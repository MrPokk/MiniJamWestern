using System;
using BitterECS.Core;
using UINotDependence.Core;
using UnityEngine;

public class GFlow
{
    public static GState GState;
    public static event Action<int, int> OnTransferProgressChanged;

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
    }

    public static void AddTransferProgress(int amount) =>
        SetTransferProgress(GState.TransferProgress + amount);

    public static void SetTransferProgress(int value)
    {
        if (GState == null)
            return;

        var newProgress = Mathf.Clamp(value, 0, GState.TransferProgressMax);
        if (newProgress == GState.TransferProgress)
            return;

        GState = GState.WithProgress(newProgress);
        OnTransferProgressChanged?.Invoke(newProgress, GState.TransferProgressMax);
    }
}
