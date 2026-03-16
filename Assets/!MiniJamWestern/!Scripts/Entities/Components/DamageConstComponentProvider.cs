using UnityEngine;
using System;
using BitterECS.Integration.Unity;

[Serializable]
public struct DamageConstComponent
{
    public int damage;
}

public class DamageConstComponentProvider : ProviderEcs<DamageConstComponent>
{
}
