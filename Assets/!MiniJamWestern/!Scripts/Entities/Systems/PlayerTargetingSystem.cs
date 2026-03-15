using BitterECS.Core;
using UnityEngine;

public class PlayerTargetingSystem : IUpdateTurn
{
    public Priority Priority => Priority.High;

    private EcsFilter<GridComponent, TagPlayer> _playerFilter;

    public void RefreshTurn()
    {
        var player = _playerFilter.First();
        if (!player.IsAlive)
        {
            return;
        }

        var gridCom = player.Get<GridComponent>();
        player.GetOrAdd<TargetToMove>().position = gridCom.currentPosition + Vector2Int.up;
    }
}
