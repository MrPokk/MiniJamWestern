using System.Collections.Generic;
using BitterECS.Core;
using UnityEngine;

public enum DifficultyTier
{
    Tier1_Base = 0,
    Tier1_Advanced = 1,
    Tier2_Base = 2,
    Tier2_Advanced = 3,
    Tier3_Base = 4
}

public class GState
{
    public readonly DifficultyTier CurrentDifficulty;
    public readonly int TransferProgress;
    public readonly int TransferProgressMax;

    public GState(DifficultyTier currentDifficulty, int transferProgress, int transferProgressMax)
    {
        CurrentDifficulty = currentDifficulty;
        TransferProgress = transferProgress;
        TransferProgressMax = transferProgressMax;
    }

    public GState(ComplicationSettings complicationSettings)
    {
        CurrentDifficulty = complicationSettings.difficultyStart;
        TransferProgress = complicationSettings.transferStart;
        TransferProgressMax = complicationSettings.transferMax;
    }

    public GState WithProgress(int newProgress)
    {
        return new GState(
            CurrentDifficulty,
            Mathf.Clamp(newProgress, 0, TransferProgressMax),
            TransferProgressMax
        );
    }

    public GState WithMaxProgress(int newMax)
    {
        var clampedProgress = Mathf.Clamp(TransferProgress, 0, newMax);
        return new GState(
            CurrentDifficulty,
            clampedProgress,
            newMax
        );
    }
}
