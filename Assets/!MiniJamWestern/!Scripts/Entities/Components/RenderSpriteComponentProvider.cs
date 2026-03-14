
using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct RenderSpriteComponent
{
    public SpriteRenderer renderer;
}

public class RenderSpriteComponentProvider : ProviderEcs<RenderSpriteComponent>
{

}
