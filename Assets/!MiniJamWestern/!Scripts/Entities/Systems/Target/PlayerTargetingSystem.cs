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

public class PlayerTargetingSystemY : IEcsRunSystem
{
    public Priority Priority => Priority.High;
    private EcsFilter<TagSelector> _ecsEntities;
    private EcsFilter<TagPlayer, GridComponent> _ecsEntitiesP;

    public void Run()
    {
        foreach (var item in _ecsEntities)
        {
            var provider = item.GetProvider<TagSelectorProvider>();
            var targetTo = _ecsEntitiesP.First().GetOrAdd<TargetTo>();

            provider.transform.position = GridInteractionHandler.Instance._playfield.ConvertingPosition(targetTo.position);
        }
    }
}
