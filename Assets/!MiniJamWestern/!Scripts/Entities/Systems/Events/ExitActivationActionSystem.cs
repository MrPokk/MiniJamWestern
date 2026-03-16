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

        if (!entity.Has<ListActionComponent>())
        {
            return;
        }

        var listComponent = entity.Get<ListActionComponent>();

        listComponent.RemoveAbility(abilityToRemove);

        if (listComponent.abilities.Count == 0)
        {
            entity.Remove<ListActionComponent>();
        }
    }
}
