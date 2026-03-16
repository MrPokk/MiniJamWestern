using System;
using BitterECS.Core;
using UnityEngine;

public class EnterActivationActionSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<IsTargetingActionEnterEvent>(added: OnEnterActing);

    private static void OnEnterActing(EcsEntity entity)
    {
        var abilityFromEvent = entity.Get<IsTargetingActionEnterEvent>().ability;

        var listComponent = entity.GetOrAdd<ListActionComponent>();
        listComponent.AddAbility(abilityFromEvent);
    }
}
