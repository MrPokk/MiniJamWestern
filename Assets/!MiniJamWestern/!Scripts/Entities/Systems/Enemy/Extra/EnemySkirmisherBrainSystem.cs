using BitterECS.Core;
using UnityEngine;

public class EnemySkirmisherBrainSystem : IUpdateTurn
{
    public Priority Priority => Priority.High;

    private EcsFilter<GridComponent, TagPlayer> _playerFilter;
    private EcsFilter<GridComponent, ListActionComponent, EnemyStateComponent, TagBehaviorSkirmisher> _enemies;

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
            ref TagBehaviorSkirmisher _) =>
        {
            if (state.state != EnemyState.Thinking) return;

            var myPos = grid.currentPosition;
            var dist = EnemyBrainUtility.GetDistance(myPos, playerPos);
            VectorUtility.TryGetStepDirection(myPos, playerPos, out var dir);

            if (EnemyBrainUtility.TryAction<TagAttackTwoSides>(e, list, playerPos, dist == 2, ref state)) return;
            if (EnemyBrainUtility.TryAction<TagMoveForward>(e, list, myPos + dir, dist > 2, ref state)) return;

            var diff = myPos - playerPos;
            var runPos = myPos + new Vector2Int(Mathf.Clamp(diff.x, -1, 1), Mathf.Clamp(diff.y, -1, 1));
            if (EnemyBrainUtility.TryAction<TagMoveForward>(e, list, runPos, dist < 2, ref state)) return;
        });
    }
}
