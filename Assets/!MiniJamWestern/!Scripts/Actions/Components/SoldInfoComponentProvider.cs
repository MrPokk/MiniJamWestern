using System;
using BitterECS.Integration.Unity;
using UnityEngine;

public class SoldInfoComponentProvider : ProviderEcs<SoldInfoComponent>
{

}

[Serializable]
public struct SoldInfoComponent
{
    public Sprite icon;
    [TextArea(3, 10)]
    public string title;
    [TextArea(5, 10)]
    public string description;
    public int amount;

    public Color valuable;
    public Color other;
}
