using BitterECS.Core;
using UnityEngine;

public class MovingToTurn : IUpdateTurn
{
    public Priority Priority => Priority.High;

    private EcsFilter<GridComponent> _gridComponentFilter;

    public void RefreshTurn()
    {
        _gridComponentFilter.For((EcsEntity e, ref GridComponent gridCom) =>
        {
            var targetIndex = gridCom.currentPosition + Vector2Int.up;

            if (!GridInteractionHandler.IsPlacing(targetIndex))
            {
                Debug.LogWarning("Moving to turn:" + targetIndex);
            }

            GridInteractionHandler.Moving(gridCom.currentPosition, targetIndex);
        });
    }
}
