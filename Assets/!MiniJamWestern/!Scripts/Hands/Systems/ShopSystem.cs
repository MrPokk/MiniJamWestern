using UnityEngine;
using DG.Tweening;
using UINotDependence.Core;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

public class ShopSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    private EcsFilter<TagPlayerMoney, MoneyComponent> _ecsEntities;

    private UIShopPopup _shopPopup;
    private ShopCard _cardPrefab;

    public void OpenShop()
    {
        _cardPrefab = new Loader<ShopCard>(UiPrefabsPaths.UICARD_ELEMENT).Prefab();

        _shopPopup = UIController.OpenPopup<UIShopPopup>();
        _shopPopup.Clear();
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

        listCard[2].AssignHeal(3, 1);

        EnsureAffordableCard(listCard);
    }

    private void EnsureAffordableCard(List<ShopCard> cards)
    {
        var playerMoney = GetPlayerMoney();
        var hasAffordable = false;

        foreach (var card in cards)
        {
            if (card.Price <= playerMoney)
            {
                hasAffordable = true;
                break;
            }
        }

        if (!hasAffordable && cards.Count > 0)
        {
            var randomCard = cards[Random.Range(0, cards.Count)];
            randomCard.SetPrice(playerMoney);
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

        return card;
    }

    private void OnCardSelected(ShopCard card)
    {
        var playerMoney = GetPlayerMoney();

        if (playerMoney >= card.Price)
        {
            SpendMoney(card.Price);
            CloseShop();
        }
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
        UIController.ClosePopup<UIShopPopup>();
    }
}
