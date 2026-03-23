using System;
using BitterECS.Core;
using UnityEngine;
using DG.Tweening;

public class CardData
{
    public string id = Guid.NewGuid().ToString();
    public AbilityViewProvider prefab;

    public CardData(AbilityViewProvider prefab)
    {
        this.prefab = prefab;
    }
}

public class HandControllerAbility : HandController<CardData, AbilityViewProvider>
{
    [Header("Animation")]
    public float animDuration = 0.3f;

    protected override void AnimateEntry(AbilityViewProvider view)
    {
        var tr = view.transform;
        tr.DOKill();
        tr.localScale = Vector3.one * 0.5f;
        tr.DOScale(Vector3.one, animDuration).SetEase(Ease.OutBack);
    }

    public void ReturnToHand(AbilityViewProvider view)
    {
        view.transform.SetParent(_containerRect);
        view.transform.DOLocalMove(Vector3.zero, animDuration).SetEase(Ease.OutQuad);
        OnChangedInternal();
    }

    public bool TryExtract(AbilityViewProvider view)
    {
        return ExtractView(view, out _);
    }
}

public class HandStackControllerAbility : HandStackController<CardData, AbilityViewProvider>
{
    public void AddCard(AbilityViewProvider prefab)
    {
        Add(new CardData(prefab), prefab);
    }
}
