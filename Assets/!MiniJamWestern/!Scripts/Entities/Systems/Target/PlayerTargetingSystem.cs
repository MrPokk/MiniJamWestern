using BitterECS.Core;
using UnityEngine;

public class PlayerTargetingSystem : IUpdateTurn
{
    public Priority Priority => Priority.FIRST_TASK;

    private EcsFilter<GridComponent, TagPlayer> _playerFilter;

    public void RefreshTurn()
    {
        Targeting();
    }

    public void Targeting()
    {
        var player = _playerFilter.First();
        if (!player.IsAlive)
        {
            return;
        }

        var gridCom = player.Get<GridComponent>();
        player.GetOrAdd<TargetTo>().position = gridCom.currentPosition + Vector2Int.up;
    }
}
