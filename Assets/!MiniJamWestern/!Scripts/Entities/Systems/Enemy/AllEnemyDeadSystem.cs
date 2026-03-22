using System;
using BitterECS.Core;
using UnityEngine;

public class AllEnemyDeadSystem : IEcsAutoImplement, IEcsInitSystem
{
    public Priority Priority => Priority.Medium;

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
            Debug.Log("All enemies are dead!");
            GFlow.IncreaseToLastDifficulty();

            //TODO: Effect to show all enemies are dead
            EcsSystemStatic.GetSystem<EnemyWaveSystem>().SpawnCurrentWave();
        }
    }
}
