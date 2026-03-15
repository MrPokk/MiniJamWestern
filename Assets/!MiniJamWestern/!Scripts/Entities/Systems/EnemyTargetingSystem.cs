using BitterECS.Core;
using UnityEngine;

public class EnemyTargetingSystem : IUpdateTurn
{
    public Priority Priority => Priority.High;

    private EcsFilter<GridComponent, TagPlayer> _playerFilter;
    private EcsFilter<TagEnemy> _enemyFilter;

    public void RefreshTurn()
    {
        var playerPosition = Vector2Int.zero;

        var player = _playerFilter.First();
        if (!player.IsAlive)
        {
            return;
        }

        _enemyFilter.For((EcsEntity e, ref TagEnemy tagEnemy) =>
        {
            e.GetOrAdd<TargetToMove>().position = player.Get<GridComponent>().currentPosition;
        });
    }
}
