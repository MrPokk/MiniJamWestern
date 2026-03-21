using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;

[Serializable]
public struct TagRotation : IActionAbility, IRotationAbility
{
    public void Execute(EcsEntity actor, ref GridComponent grid, ListActionComponent list, ref TargetTo target)
    {
        RotationHandler.Execute(actor, grid, ref target);
    }
}

public class TagRotationProvider : ProviderEcs<TagRotation> { }
