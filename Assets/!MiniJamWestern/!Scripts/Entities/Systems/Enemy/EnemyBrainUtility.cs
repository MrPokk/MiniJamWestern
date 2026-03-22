using BitterECS.Core;
using UnityEngine;

public static class EnemyBrainUtility
{
    public static bool TryAction<T>(EcsEntity entity, ListActionComponent list, Vector2Int target, bool condition, ref EnemyStateComponent state)
        where T : IActionAbility
    {
        if (condition && list.Is<T>(out var ability, out var abEntity))
        {
            SetIntent(entity, ability, abEntity, target, ref state);
            return true;
        }
        return false;
    }

    public static void SetIntent(EcsEntity entity, IActionAbility ability, EcsEntity abEntity, Vector2Int target, ref EnemyStateComponent state)
    {
        ref var intent = ref entity.GetOrAdd<IsIntentComponent>();
        intent.chosenAbility = ability;
        intent.abilityEntity = abEntity;
        intent.targetPosition = target;
        state.state = EnemyState.Preparing;
    }

    public static int GetDistance(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
}
