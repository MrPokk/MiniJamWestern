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
    [TextArea(1, 3)]
    public string title;
    [TextArea(1, 5)]
    public string description;
    public int amount;
}
