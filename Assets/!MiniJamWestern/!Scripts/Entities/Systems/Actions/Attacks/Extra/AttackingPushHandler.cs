using BitterECS.Core;
using UnityEngine;

public class AttackingPushHandler
{
    public static void Execute(EcsEntity _, GridComponent grid, ListActionComponent list, TargetTo target)
    {
        if (!list.Is<TagAttackPush>(out var attackAbility)) return;

        var diff = target.position - grid.currentPosition;
        var distance = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);

        if (distance <= attackAbility.distance)
        {
            if (GridInteractionHandler.TryGetEntityAt(target.position, out var entityTo))
            {
                var pushDir = new Vector2Int(Mathf.Clamp(diff.x, -1, 1), Mathf.Clamp(diff.y, -1, 1));
                if (pushDir != Vector2Int.zero)
                {
                    var newPos = target.position + pushDir;

                    if (GridInteractionHandler.IsPlacing(newPos))
                    {
                        GridInteractionHandler.MoveEntity(entityTo, newPos);
                    }
                }
            }
        }
    }
}

public class AttackingPushHandlerAuto : IEcsAutoImplement
{
    public Priority Priority => Priority.Low;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<IsAttackerTo>(added: OnAttack);

    private static void OnAttack(EcsEntity entity)
    {

    }
}
