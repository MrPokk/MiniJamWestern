using System;
using BitterECS.Core;
using UINotDependence.Core;
using UnityEngine;

public class GFlow
{
    public static GState GState;
    public static event Action<int, int> OnTransferProgressChanged;
    public static event Action<DifficultyTier> OnDifficultyChanged;

    public GFlow(GState gStat)
    {
        GState = gStat;
    }

    public void Play()
    {
        UIController.OpenPopup<UIChamberPopup>();
    }

    public static void RefreshTurn()
    {
        EcsSystemStatic.Run<IUpdateTurn>(s => s.RefreshTurn());

        ResetTransferProgress();
    }

    public static void ResetTransferProgress()
    {
        if (GState == null) return;
        SetTransferProgress(GState.TransferProgressMax);
    }

    public static void AddTransferProgressMax(int amount) =>
        SetTransferProgressMax(GState.TransferProgressMax + amount);

    public static void AddTransferProgress(int amount) =>
        SetTransferProgress(GState.TransferProgress + amount);

    public static void MinusTransferProgressMax(int amount) =>
        SetTransferProgressMax(GState.TransferProgressMax - amount);

    public static void MinusTransferProgress(int amount) =>
        SetTransferProgress(GState.TransferProgress - amount);

    public static void IncreaseToLastDifficulty()
    {
        if (GState == null) return;

        var values = (DifficultyTier[])Enum.GetValues(typeof(DifficultyTier));
        var lastDifficulty = values[^1];

        var current = GState.CurrentDifficulty;
        while (current < lastDifficulty)
        {
            current++;
            GState = GState.WithDifficulty(current);
            OnDifficultyChanged?.Invoke(current);
        }
    }

    public static void SetTransferProgress(int value)
    {
        if (GState == null) return;

        var newProgress = Mathf.Clamp(value, 0, GState.TransferProgressMax);

        GState = GState.WithProgress(newProgress);
        OnTransferProgressChanged?.Invoke(newProgress, GState.TransferProgressMax);

        if (newProgress <= 0)
        {
            RefreshTurn();
        }
    }

    public static void SetTransferProgressMax(int value)
    {
        if (GState == null)
            return;

        if (value < 0) value = 0;
        if (value == GState.TransferProgressMax)
            return;

        GState = GState.WithMaxProgress(value);
        OnTransferProgressChanged?.Invoke(GState.TransferProgress, GState.TransferProgressMax);
    }
}
