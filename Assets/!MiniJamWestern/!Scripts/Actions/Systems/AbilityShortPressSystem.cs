using BitterECS.Core;
using UnityEngine;

public class AbilityShortPressSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.Low;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<ShortPressAbilityEvent>(added: OnShortPress);

    private static void OnShortPress(EcsEntity abilityEntity)
    {
        Debug.Log("Short press on ability");
    }
}
