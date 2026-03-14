using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using InGame.Script.Component_Sound;
using UINotDependence.Core;

public class GFlow
{
    public static GState GState;

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
}
