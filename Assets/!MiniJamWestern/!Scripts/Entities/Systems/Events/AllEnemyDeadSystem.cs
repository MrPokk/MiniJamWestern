using System;
using BitterECS.Core;
using UnityEngine;

public class AllEnemyDeadSystem : IEcsAutoImplement, IEcsInitSystem
{
    public Priority Priority => Priority.High;

    private EcsFilter<TagEnemy> _ecsEntities;
    private EcsEvent _ecsEvent;

    public void Init()
    {
        _ecsEvent.Subscribe<IsDeadEvent>(added: OnTarget);
    }

    private void OnTarget(EcsEntity entity)
    {
        if (_ecsEntities.Count <= 0)
        {
            GFlow.IncreaseToLastDifficulty();
            Debug.Log($"[Difficulty] {GFlow.GState.CurrentDifficulty}");
        }
    }
}
