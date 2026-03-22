using BitterECS.Core;
using UnityEngine;

public class EnemyAttackTargetingSystem : IUpdateTurn
{
    public Priority Priority => Priority.FIRST_TASK;

    private EcsFilter<GridComponent, TagPlayer> _playerFilter = new();
    private EcsFilter<GridComponent, TagEnemy> _enemyFilter = new();

    public void RefreshTurn()
    {
        var player = _playerFilter.First();
        if (!player.IsAlive) return;

        var playerPos = player.Get<GridComponent>().currentPosition;

        _enemyFilter.For((EcsEntity e, ref GridComponent gridCom, ref TagEnemy tagEnemy) =>
        {
            if (e.TryGet<IsIntentComponent>(out _)) return;

            e.GetOrAdd<TargetTo>().position = playerPos;
        });
    }
}
