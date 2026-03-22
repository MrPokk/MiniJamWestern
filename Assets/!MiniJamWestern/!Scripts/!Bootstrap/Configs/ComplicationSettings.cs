using UnityEngine;

[CreateAssetMenu(fileName = "ComplicationSettings", menuName = "Gameplay/Complication Settings")]
public class ComplicationSettings : ScriptableObject
{
    [Header("Difficulty Progression (Setting)")]
    public DifficultyTier difficultyStart;

    [Header("Transfer Progression (Setting)")]
    public int transferStart;
    public int transferMax;

    [Header("Enemy Wave Progression (Setting)")]
    public EnemyWaveConfig enemyWaveConfig;
}
