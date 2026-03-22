using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveConfig", menuName = "Gameplay/Enemy Wave Config")]
public class EnemyWaveConfig : ScriptableObject
{
    [System.Serializable]
    public class EnemyWaveData
    {
        public DifficultyTier difficultyTier;
        public List<TagEnemyProvider> enemyPrefabs;

        [Header("Optional: leave size at 0,0 for random screen spawn")]
        public RectInt spawnArea;
    }

    public List<EnemyWaveData> waves;
}
