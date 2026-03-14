using System.Collections.Generic;
using BitterECS.Core;

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
    public DifficultyTier currentDifficulty;

    public GState(DifficultyTier currentDifficulty)
    {
        this.currentDifficulty = currentDifficulty;
    }
}
