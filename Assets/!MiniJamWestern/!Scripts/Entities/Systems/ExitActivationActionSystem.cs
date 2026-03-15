using System;
using BitterECS.Core;
using UnityEngine;

public class ExitActivationActionSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    private EcsEvent _ecsEvent = new EcsEvent()
    .Subscribe<IsTargetingActionExitEvent>(added: OnExitActing);

    private static void OnExitActing(EcsEntity entity)
    {
        var abilityToRemove = entity.Get<IsTargetingActionExitEvent>().ability;

        if (!entity.Has<IsActionComponent>())
        {
            return;
        }

        var currentComponent = entity.Get<IsActionComponent>();
        if (currentComponent.ability == abilityToRemove)
        {
            entity.Remove<IsActionComponent>();
        }
    }
}
