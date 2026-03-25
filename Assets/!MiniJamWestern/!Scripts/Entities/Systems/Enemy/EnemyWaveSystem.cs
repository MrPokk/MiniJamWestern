using BitterECS.Core;
using UnityEngine;

public class EnemyWaveSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;
    private ComplicationSettings _settings;

    public void Setup(ComplicationSettings settings) => _settings = settings;

    public void SpawnCurrentWave(DifficultyTier? overrideTier = null)
    {
        var targetTier = overrideTier ?? GFlow.GState.CurrentDifficulty;

        var waveData = _settings.enemyWaveConfig.waves.Find(w => w.difficultyTier == targetTier);
        if (waveData == null || waveData.enemyGroups == null)
        {
            Debug.LogWarning($"[EnemyWaveSystem] Wave data for {targetTier} not found!");
            return;
        }

        // IncreaseToDifficulty(targetTier);

        foreach (var group in waveData.enemyGroups)
        {
            SpawnGroup(group);
        }
    }

    private static void IncreaseToDifficulty(DifficultyTier targetTier)
    {
        Debug.Log($"Spawning wave for tier: {targetTier}");
        if (targetTier == DifficultyTier.Tier1_Advanced)
        {
            Debug.Log("Add Transfer to");
            GFlow.AddTransferProgressMax(1);
            GFlow.AddTransferProgress(1);
        }
    }

    private void SpawnGroup(EnemyWaveConfig.EnemySpawnGroup group)
    {
        if (group.enemyPrefabs == null) return;

        foreach (var prefab in group.enemyPrefabs)
        {
            if (prefab == null) continue;

            Vector2Int spawnPos;
            bool found;

            if (group.spawnArea.width > 0 && group.spawnArea.height > 0)
            {
                found = GridInteractionHandler.TryGetRandomEmptyPointInArea(
                    group.spawnArea.min,
                    group.spawnArea.max,
                    out spawnPos);
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
                Debug.LogWarning($"[EnemyWaveSystem] No space for {prefab.name} in group {group.groupName}");
            }
        }
    }
}
