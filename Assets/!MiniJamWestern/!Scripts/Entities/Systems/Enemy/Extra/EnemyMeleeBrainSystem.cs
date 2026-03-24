using BitterECS.Core;
using UnityEngine;

public class EnemyMeleeBrainSystem : IUpdateTurn
{
    public Priority Priority => Priority.High;

    private EcsFilter<GridComponent, TagPlayer> _playerFilter;
    private EcsFilter<GridComponent, ListActionComponent, EnemyStateComponent, TagBehaviorMelee> _enemies;

    public void RefreshTurn()
    {
        var player = _playerFilter.First();
        if (!player.IsAlive) return;

        var playerPos = player.Get<GridComponent>().currentPosition;

        _enemies.For((
            EcsEntity e,
            ref GridComponent grid,
            ref ListActionComponent list,
            ref EnemyStateComponent state,
            ref TagBehaviorMelee _) =>
        {
            if (state.state != EnemyState.Thinking) return;

            var myPos = grid.currentPosition;
            var dist = EnemyBrainUtility.GetDistance(myPos, playerPos);
            VectorUtility.TryGetStepDirection(myPos, playerPos, out var dir);

            list.Is<TagAttackForward>(out var attackForward);
            var distTarget = attackForward.value;

            if (EnemyBrainUtility.TryAction<TagAttackForward>(e, list, playerPos, dist <= distTarget, ref state)) return;
            if (EnemyBrainUtility.TryAction<TagMoveForward>(e, list, myPos + dir, true, ref state)) return;
        });
    }
}
