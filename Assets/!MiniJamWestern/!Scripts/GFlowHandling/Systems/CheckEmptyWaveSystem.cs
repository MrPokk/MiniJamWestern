using System.Collections;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UINotDependence.Core;
using UnityEngine;

public class CheckEmptyWaveSystem : IEcsRunSystem
{
    public Priority Priority => Priority.High;

    private EcsFilter<TagPlayer, GridComponent> _player;
    private EcsFilter<TagEnemy, GridComponent> _enemies;
    private CoroutineHandle _isTransitioning;

    public void Run()
    {
        if (_enemies.Count > 0) return;
        if (_isTransitioning.IsValid) return;

        _isTransitioning = CoroutineUtility.Run(WaveTransitionSequence());
    }

    private IEnumerator WaveTransitionSequence()
    {
        var playerEntity = _player.First();
        var currentPos = playerEntity.Get<GridComponent>().currentPosition;

        var maxY = GetMaxY(currentPos);

        UIController.OpenPopup<UIEffectTransitionPopup>();
        yield return new WaitForSeconds(0.5f);

        var minY = GetMinY(currentPos);
        var midY = GetMidY(minY, maxY);
        GridInteractionHandler.Moving(currentPos, new Vector2Int(currentPos.x, midY));

        GFlow.IncreaseToDifficulty();
        EcsSystemStatic.GetSystem<EnemyWaveSystem>().SpawnCurrentWave();

        UIController.ClosePopup<UIEffectTransitionPopup>();

        EcsSystemStatic.GetSystem<ShopSystem>().OpenShop();


        _isTransitioning = default;
    }

    private int GetMaxY(Vector2Int pos)
    {
        var playfield = GridInteractionHandler.Instance.Playfield;
        var maxY = pos.y;

        while (playfield.IsWithinGrid(new Vector2Int(pos.x, maxY + 1)))
        {
            maxY++;
        }

        return maxY;
    }
    private int GetMinY(Vector2Int pos)
    {
        var playfield = GridInteractionHandler.Instance.Playfield;
        var minY = pos.y;

        while (playfield.IsWithinGrid(new Vector2Int(pos.x, minY - 1)))
        {
            minY--;
        }

        return minY;
    }
    private int GetMidY(int minY, int maxY)
    {
        return (minY + maxY) / 2;
    }
}
