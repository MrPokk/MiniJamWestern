using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct FacingComponent
{
    public Vector2Int direction;
}

public class FacingComponentProvider : ProviderEcs<FacingComponent> { }
