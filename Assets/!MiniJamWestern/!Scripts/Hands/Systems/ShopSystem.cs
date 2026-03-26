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
    private EcsFilter<TagPlayer, HealthComponent> _playerFilter;

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
            listCard.Add(CreateCard());
        }

        var availablePaths = new List<string>(ActionsExtraPaths.AllPaths);

        AssignUniqueAction(listCard[0], availablePaths);
        AssignUniqueAction(listCard[1], availablePaths);

        var player = _playerFilter.First();
        var healthComp = player.Get<HealthComponent>();

        var currentHealth = healthComp.GetCurrentHealth();
        var currentHealthMax = healthComp.GetMaxHealth();
        if (currentHealth >= currentHealthMax
            && currentHealth >= AmountRegeneration)
        {
            listCard[2].AssignMaxHealth(AmountAddMaxCount, 3);
        }
        else
        {
            listCard[2].AssignHeal(AmountRegeneration, 3);
        }

        EnsureAffordableCard(listCard);

        var playerMoney = GetPlayerMoney();
        foreach (var card in listCard)
        {
            if (playerMoney < card.Price)
                card.SetAffordable();
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
                var randomCard = actionCards[Random.Range(0, actionCards.Count)];
                randomCard.SetPrice(playerMoney);
            }
        }
    }

    private void AssignUniqueAction(ShopCard card, List<string> paths)
    {
        if (paths.Count == 0) throw new Exception("No more unique actions available!");

        var randomIndex = Random.Range(0, paths.Count);
        var selectedPath = paths[randomIndex];

        paths.RemoveAt(randomIndex);

        var provider = new Loader<TagActionsProvider>(selectedPath).Prefab();
        card.AssignAction(provider);
    }

    private ShopCard CreateCard()
    {
        var card = Object.Instantiate(_cardPrefab);
        card.onSelected = OnCardSelected;

        _shopPopup.AddCard(card);
        SoundController.PlaySoundRandomPitch(SoundType.GiveCards); // Уже было реализовано

        return card;
    }

    private void OnCardSelected(ShopCard card)
    {
        var playerMoney = GetPlayerMoney();

        if (playerMoney < card.Price)
        {
            return;
        }

        EcsSystemStatic.GetSystem<ShopAbilityPurchaseSystem>().ProcessPurchase(card);
        EcsSystemStatic.GetSystem<ShopHealthPurchaseSystem>().ProcessPurchase(card);
        SoundController.PlaySoundRandomPitch(SoundType.TakeCards); // Уже было реализовано

        SpendMoney(card.Price);
        CloseShop();
    }

    private int GetPlayerMoney()
    {
        var money = _ecsEntities.First().Get<MoneyComponent>().GetCurrentMoney();
        return money;
    }

    private void SpendMoney(int amount)
    {
        var ecsEntity = _ecsEntities.First();
        ref var moneyComponent = ref ecsEntity.Get<MoneyComponent>();
        var deltaMoney = moneyComponent.GetCurrentMoney() - amount;
        moneyComponent.SetMoney(deltaMoney);
        ecsEntity.AddFrame<PlayerUpdateMoneyUIEvent>();
    }

    public void CloseShop()
    {
        SoundController.PlaySoundRandomPitch(SoundType.ShopCardsClose);

        UIController.ClosePopup<UIShopPopup>();
    }
}
