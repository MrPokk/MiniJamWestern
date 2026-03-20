using BitterECS.Core;
using UnityEngine;


public class AttackingForwardHandler
{
    public static void Execute(EcsEntity entity, GridComponent grid, ListActionComponent list, TargetTo target)
    {
        if (!list.Is<TagAttackForward>(out var attackAbility)) return;

        var distance = Mathf.Abs(target.position.x - grid.currentPosition.x) +
                       Mathf.Abs(target.position.y - grid.currentPosition.y);

        if (distance <= attackAbility.distance)
        {
            if (GridInteractionHandler.TryGetEntityAt(target.position, out var entityTo))
            {
                entity.AddFrame<IsAttackerTo>(new(entityTo));
            }
        }
    }
}
