using BitterECS.Core;
using UnityEngine;

public class EnemyWaveSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;
    private ComplicationSettings _settings;
    public void Setup(ComplicationSettings settings) => _settings = settings;

    public void SpawnCurrentWave()
    {
        var currentTier = GFlow.GState.CurrentDifficulty;
        var waveData = _settings.enemyWaveConfig.waves.Find(w => w.difficultyTier == currentTier);

        if (waveData == null || waveData.enemyPrefabs.Count == 0) return;

        foreach (var prefab in waveData.enemyPrefabs)
        {
            Vector2Int spawnPos;
            bool found;

            if (waveData.spawnArea.width > 0 && waveData.spawnArea.height > 0)
            {
                var min = waveData.spawnArea.min;
                var max = waveData.spawnArea.max;
                found = GridInteractionHandler.TryGetRandomEmptyPointInArea(min, max, out spawnPos);
            }
            else
            {
                found = GridInteractionHandler.TryGetRandomEmptyPoint(out spawnPos);
            }

            if (found)
            {
                GridInteractionHandler.InstantiateObject(spawnPos, prefab, out _);
            }
            else
            {
                Debug.LogWarning($"[EnemyWaveSystem] No empty space found for prefab {prefab.name}");
            }
        }
    }
}
