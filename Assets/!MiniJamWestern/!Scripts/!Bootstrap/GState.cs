using System.Collections.Generic;
using BitterECS.Core;
using UnityEngine;

public enum DifficultyTier : int
{
    Tier1_Base,
    Tier1_Advanced,
    Tier2_Base,
    Tier2_Advanced,
    Tier3_Base,
    Tier4_Advanced,
    Tier5_Advanced,
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

    public GState WithDifficulty(DifficultyTier newDifficulty)
    {
        return new GState(newDifficulty, TransferProgress, TransferProgressMax);
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
