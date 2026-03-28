using UnityEngine;
using DG.Tweening;
using UINotDependence.Core;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using InGame.Script.Component_Sound;

public class ShopSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    private const int AmountRegeneration = 3;
    private const int AmountAddMaxCount = 1;
    private EcsFilter<TagPlayerMoney, MoneyComponent> _ecsEntities;
    private EcsFilter<TagPlayer, GridComponent, HealthComponent> _playerFilter;
    private EcsFilter<TagInventoryEffects> _effectsFilter;

    private UIShopPopup _shopPopup;
    private ShopCard _cardPrefab;

    public void OpenShop()
    {
        _cardPrefab = new Loader<ShopCard>(UiPrefabsPaths.UICARD_ELEMENT).Prefab();
        _shopPopup = UIController.OpenPopup<UIShopPopup>();
        _shopPopup.Clear();

        SoundController.PlaySoundRandomPitch(SoundType.ShopCardsOpen);

        SpawnCards();
    }

    private void SpawnCards()
    {
        var listCard = new List<ShopCard>();
        for (var i = 0; i < 3; i++)
        {
            var card = CreateCard();
            card.transform.localScale = Vector3.zero;
            listCard.Add(card);
        }

        var ownedAbilitiesTitles = new HashSet<string>();
        foreach (var invEntity in _effectsFilter)
        {
            var inv = invEntity.GetProvider<AbilityInventoryProvider>();
            if (inv == null) continue;

            foreach (var slot in inv.Value.listSlot)
            {
                var itemEnt = slot.Value.itemEntity;
                if (itemEnt.TryGet<SoldInfoComponent>(out var soldInfo))
                {
                    if (!itemEnt.TryGet<MultiAbility>(out _))
                        ownedAbilitiesTitles.Add(soldInfo.title);
                }
            }
        }

        var attackPaths = new List<string>();
        var generalPaths = new List<string>();

        foreach (var path in ActionsExtraPaths.AllPaths)
        {
            var providerPrefab = new Loader<TagActionsProvider>(path).Prefab();
            if (providerPrefab == null) continue;

            if (providerPrefab.Entity.TryGet<SoldInfoComponent>(out var soldInfo))
            {
                if (ownedAbilitiesTitles.Contains(soldInfo.title)) continue;
            }

            bool isAttack = false;
            if (providerPrefab.Entity.TryGet<TagActions>(out var tagActions))
            {
                if (tagActions.ability is IAttackAbility) isAttack = true;
            }

            if (isAttack) attackPaths.Add(path);
            else generalPaths.Add(path);
        }

        if (attackPaths.Count > 0)
        {
            AssignUniqueAction(listCard[0], attackPaths);
            generalPaths.AddRange(attackPaths);
            AssignUniqueAction(listCard[1], generalPaths);
        }
        else
        {
            AssignUniqueAction(listCard[0], generalPaths);
            AssignUniqueAction(listCard[1], generalPaths);
        }

        var player = _playerFilter.First();
        ref var healthComp = ref player.Get<HealthComponent>();
        var currentHealth = healthComp.GetCurrentHealth();
        var currentHealthMax = healthComp.GetMaxHealth();

        Debug.Log($"{currentHealth} / {currentHealthMax}");
        if (currentHealth >= currentHealthMax && currentHealth >= AmountRegeneration)
            listCard[2].AssignMaxHealth(AmountAddMaxCount, 3);
        else
            listCard[2].AssignHeal(AmountRegeneration, 3);

        EnsureAffordableCard(listCard);

        var playerMoney = GetPlayerMoney();
        foreach (var card in listCard)
        {
            if (playerMoney < card.Price) card.SetAffordable();
        }
    }

    private void EnsureAffordableCard(List<ShopCard> cards)
    {
        var playerMoney = GetPlayerMoney();
        var hasAffordable = false;

        foreach (var card in cards)
        {
            if (card.Price <= playerMoney && card.Type != ShopCard.CardType.HEAL && card.Type != ShopCard.CardType.MAX_HEALTH)
            {
                hasAffordable = true;
                break;
            }
        }

        if (!hasAffordable)
        {
            var actionCards = cards.FindAll(c => c.Type == ShopCard.CardType.ACTION);
            if (actionCards.Count > 0)
            {
                var randomCard = actionCards[UnityEngine.Random.Range(0, actionCards.Count)];
                randomCard.SetPrice(playerMoney);
            }
        }
    }

    private void AssignUniqueAction(ShopCard card, List<string> paths)
    {
        if (paths.Count == 0) return;
        var randomIndex = UnityEngine.Random.Range(0, paths.Count);
        var selectedPath = paths[randomIndex];
        paths.RemoveAt(randomIndex);
        var provider = new Loader<TagActionsProvider>(selectedPath).Prefab();
        card.AssignAction(provider);
    }

    private ShopCard CreateCard()
    {
        var card = UnityEngine.Object.Instantiate(_cardPrefab);
        card.onSelected = OnCardSelected;
        _shopPopup.AddCard(card);
        card.transform.localScale = Vector3.zero;
        return card;
    }

    private void OnCardSelected(ShopCard card)
    {
        if (GetPlayerMoney() < card.Price) return;
        EcsSystemStatic.GetSystem<ShopAbilityPurchaseSystem>().ProcessPurchase(card);
        EcsSystemStatic.GetSystem<ShopHealthPurchaseSystem>().ProcessPurchase(card);
        SoundController.PlaySoundRandomPitch(SoundType.TakeCards);
        SpendMoney(card.Price);
        CloseShop();
    }

    private int GetPlayerMoney() => _ecsEntities.First().Get<MoneyComponent>().GetCurrentMoney();

    private void SpendMoney(int amount)
    {
        var ecsEntity = _ecsEntities.First();
        ref var moneyComponent = ref ecsEntity.Get<MoneyComponent>();
        moneyComponent.SetMoney(moneyComponent.GetCurrentMoney() - amount);
        ecsEntity.AddFrame<PlayerUpdateMoneyUIEvent>();
    }

    public void CloseShop()
    {
        SoundController.PlaySoundRandomPitch(SoundType.ShopCardsClose);
        UIController.ClosePopup<UIShopPopup>();
    }
}
