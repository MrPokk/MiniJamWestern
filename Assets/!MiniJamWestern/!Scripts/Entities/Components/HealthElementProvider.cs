using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HealthElementProvider : ProviderEcs<HealthElementComponent>, IHealthSlot
{
    private SpriteRenderer _renderer;
    protected override void Registration() => _renderer = GetComponent<SpriteRenderer>();

    public void SetActive(bool active) => gameObject.SetActive(active);
    public void SetSprite(Sprite sprite) => _renderer.sprite = sprite;
}

[Serializable] public struct HealthElementComponent { }
