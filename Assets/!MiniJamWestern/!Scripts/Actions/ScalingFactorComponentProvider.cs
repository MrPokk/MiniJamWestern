using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct ScalingFactorComponent
{
    public Vector2 slotSize;
    [HideInInspector] public Vector3 calculatedScale;
}

public class ScalingFactorComponentProvider : ProviderEcs<ScalingFactorComponent>
{
}
