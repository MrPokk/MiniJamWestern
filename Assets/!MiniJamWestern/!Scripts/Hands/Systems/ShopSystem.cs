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

    private UIShopPopup _shopPopup;
    private ShopCard _cardPrefab;
    public void OpenShop()
    {

        _cardPrefab = new Loader<ShopCard>(CardsPaths.UICARD_ELEMENT).Prefab();

        _shopPopup = UIController.OpenPopup<UIShopPopup>();
        _shopPopup.Clear();
        SpawnCards();
    }

    private void SpawnCards()
    {
        var listCard = new List<ShopCard>();
        for (var i = 0; i < 3; i++)
        {
            listCard.Add(CreateCard(i));
        }

        var availablePaths = new List<string>(ActionsExtraPaths.AllPaths);

        AssignUniqueAction(listCard[0], availablePaths);
        AssignUniqueAction(listCard[1], availablePaths);

        listCard[2].AssignHeal(25);
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

    private ShopCard CreateCard(int index)
    {
        var card = Object.Instantiate(_cardPrefab);
        card.onSelected = OnCardSelected;

        _shopPopup.AddCard(card);

        card.visual.localScale = Vector3.zero;
        card.visual.DOScale(Vector3.one, 0.5f)
            .SetDelay(index * 0.2f)
            .SetEase(Ease.OutBack);

        return card;
    }

    private void OnCardSelected(ShopCard card)
    {
        CloseShop();
    }

    public void CloseShop()
    {
        UIController.ClosePopup<UIShopPopup>();
    }
}
