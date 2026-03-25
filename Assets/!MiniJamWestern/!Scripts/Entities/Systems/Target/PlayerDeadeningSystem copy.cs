using System;
using BitterECS.Core;
using UnityEngine.SceneManagement;

public class PlayerKillBossSystem : IEcsInitSystem
{
    public Priority Priority => Priority.High;

    public void Init()
    {
        GFlow.OnDifficultyChanged += OnPlayerFinal;
    }

    private void OnPlayerFinal(DifficultyTier tier)
    {

    }
}
