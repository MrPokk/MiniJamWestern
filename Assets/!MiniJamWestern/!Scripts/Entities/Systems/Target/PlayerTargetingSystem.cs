using BitterECS.Core;
using UnityEngine;

public class PlayerTargetingSystem : IUpdateTurn, IEcsInitSystem
{
    public Priority Priority => Priority.FIRST_TASK;
    private EcsFilter<GridComponent, TagPlayer> _playerFilter;

    public void RefreshTurn() => Targeting();
    public void Init()
    {
        Targeting();
    }

    public void Targeting()
    {
        var player = _playerFilter.First();
        if (!player.IsAlive) return;

        var gridCom = player.Get<GridComponent>();
        var facingDir = player.TryGet<FacingComponent>(out var f) ? f.direction : Vector2Int.up;

        player.GetOrAdd<TargetTo>().position = gridCom.currentPosition + facingDir;
    }
}
