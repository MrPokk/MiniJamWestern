
using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct SpriteComponent
{
    public Sprite sprite;
}

public class SpriteComponentProvider : ProviderEcs<SpriteComponent>
{

}
