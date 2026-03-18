using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct HealthElementComponent
{
    public HealthDisplayProvider healthDisplay;
}

[RequireComponent(typeof(SpriteRenderer))]
public class HealthElementProvider : ProviderEcs<HealthElementComponent>
{
    [HideInInspector] public SpriteRenderer icon;

    protected override void Registration()
    {
        icon = GetComponent<SpriteRenderer>();

    }
}
