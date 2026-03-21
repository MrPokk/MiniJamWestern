using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct SetColorComponent
{
    public Color color;
}


public class SetColorComponentProvider : ProviderEcs<SetColorComponent>
{

}
