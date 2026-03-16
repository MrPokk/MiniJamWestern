using BitterECS.Core;
using UnityEngine;

public class AttackingForward : IUpdateTurn
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<GridComponent, ListActionComponent, TargetTo> _filter = new();

    public void RefreshTurn()
    {
        _filter.For((EcsEntity e, ref GridComponent gridCom, ref ListActionComponent list, ref TargetTo target) =>
        {
            if (!list.Is<TagAttackForward>(out var attackAbility))
            {
                return;
            }

            var distanceToTarget = Mathf.Abs(target.position.x - gridCom.currentPosition.x) +
                                   Mathf.Abs(target.position.y - gridCom.currentPosition.y);

            if (distanceToTarget > attackAbility.distance)
            {
                return;
            }

            var attackPos = target.position;
            Debug.Log($"Атака по клетке (радиусе): {attackPos}");

            if (!GridInteractionHandler.TryGetEntityAt(attackPos, out var entityTo))
            {
                return;
            }

            e.AddFrame<IsAttackerTo>(new(entityTo));
        });
    }
}
