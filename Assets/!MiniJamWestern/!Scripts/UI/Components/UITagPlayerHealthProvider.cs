
using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct UITagPlayerHealth
{

}

[RequireComponent(typeof(HealthDisplayProvider))]
public class UITagPlayerHealthProvider : ProviderEcs<UITagPlayerHealth>
{
    private void Start()
    {
        Entity.AddFrame<PlayerUpdateHealthUIEvent>();
    }
}
