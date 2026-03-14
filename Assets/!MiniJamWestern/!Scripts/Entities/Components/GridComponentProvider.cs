using System;
using System.Numerics;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct GridComponent
{
    [ReadOnly] public MonoGridPresenter gridPresenter;
    [ReadOnly] public Vector2Int currentPosition;

    public GridComponent(Vector2Int currentPosition, MonoGridPresenter gridPresenter)
    {
        this.currentPosition = currentPosition;
        this.gridPresenter = gridPresenter;
    }
}

public class GridComponentProvider : ProviderEcs<GridComponent> { }
