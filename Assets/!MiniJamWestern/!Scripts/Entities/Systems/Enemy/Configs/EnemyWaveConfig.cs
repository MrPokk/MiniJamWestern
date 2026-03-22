using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveConfig", menuName = "Gameplay/Enemy Wave Config")]
public class EnemyWaveConfig : ScriptableObject
{
    [Serializable]
    public class EnemySpawnGroup
    {
        public string groupName;
        public List<TagEnemyProvider> enemyPrefabs;

        [Header("Zero size = random grid spawn")]
        public RectInt spawnArea;
    }

    [Serializable]
    public class EnemyWaveData
    {
        public DifficultyTier difficultyTier;
        public List<EnemySpawnGroup> enemyGroups;
    }

    public List<EnemyWaveData> waves;
}
