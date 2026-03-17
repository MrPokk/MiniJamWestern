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

public class EnemyAttackingForward : IUpdateTurn
{
    public Priority Priority => Priority.Medium;
    private EcsFilter<TagEnemy, GridComponent, ListActionComponent, TargetTo> _filter;

    public void RefreshTurn()
    {
        _filter.For((EcsEntity e, ref TagEnemy enemy, ref GridComponent grid, ref ListActionComponent list, ref TargetTo target) =>
        {
            AttackingForwardHandler.Execute(e, grid, list, target);
        });
    }
}
