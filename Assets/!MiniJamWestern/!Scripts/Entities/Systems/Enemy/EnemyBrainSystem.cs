using BitterECS.Core;
using UnityEngine;

public class EnemyBrainSystem : IUpdateTurn
{
    public Priority Priority => Priority.High;

    private EcsFilter<GridComponent, TagPlayer> _playerFilter;

    private EcsFilter<GridComponent, ListActionComponent, TagEnemy, TagBehaviorMelee> _meleeEnemies;
    private EcsFilter<GridComponent, ListActionComponent, TagEnemy, TagBehaviorSkirmisher> _skirmisherEnemies;

    public void RefreshTurn()
    {
        var player = _playerFilter.First();
        if (!player.IsAlive) return;

        var playerPos = player.Get<GridComponent>().currentPosition;

        _meleeEnemies.For((
            EcsEntity entity,
            ref GridComponent grid,
            ref ListActionComponent list,
            ref TagEnemy _,
            ref TagBehaviorMelee _) =>
        {
            PlanMeleeBehavior(entity, grid, list, playerPos);
        });

        _skirmisherEnemies.For((
            EcsEntity entity,
            ref GridComponent grid,
            ref ListActionComponent list,
            ref TagEnemy _,
            ref TagBehaviorSkirmisher _) =>
        {
            PlanSkirmisherBehavior(entity, grid, list, playerPos);
        });
    }

    private void PlanMeleeBehavior(EcsEntity entity, GridComponent grid, ListActionComponent list, Vector2Int playerPos)
    {
        var distance = Mathf.Abs(playerPos.x - grid.currentPosition.x) + Mathf.Abs(playerPos.y - grid.currentPosition.y);

        if (distance <= 1)
        {
            if (list.Is<TagAttackForward>(out var attack))
            {
                SetIntent(entity, attack, playerPos);
                return;
            }
        }
        else
        {
            if (list.Is<TagMoveForward>(out var move))
            {
                SetIntent(entity, move, playerPos);
                return;
            }
        }
    }

    private void PlanSkirmisherBehavior(EcsEntity entity, GridComponent grid, ListActionComponent list, Vector2Int playerPos)
    {
        var distance = Mathf.Abs(playerPos.x - grid.currentPosition.x) + Mathf.Abs(playerPos.y - grid.currentPosition.y);
        var optimalDistance = 2;

        if (distance == optimalDistance)
        {
            if (list.Is<TagAttackTwoSides>(out var attack))
            {
                SetIntent(entity, attack, playerPos);
                return;
            }
        }
        else if (distance > optimalDistance)
        {
            if (list.Is<TagMoveForward>(out var moveForward))
            {
                SetIntent(entity, moveForward, playerPos);
                return;
            }
        }
        else if (distance < optimalDistance)
        {
            if (list.Is<TagMoveForward>(out var moveAway))
            {
                var directionAway = grid.currentPosition - playerPos;
                var escapeTarget = grid.currentPosition + new Vector2Int(Mathf.Clamp(directionAway.x, -1, 1), Mathf.Clamp(directionAway.y, -1, 1));
                SetIntent(entity, moveAway, escapeTarget);
                return;
            }
        }
    }

    private void SetIntent(EcsEntity entity, IActionAbility ability, Vector2Int targetPos)
    {
        ref var intent = ref entity.GetOrAdd<IsIntentComponent>();
        intent.chosenAbility = ability;
        intent.targetPosition = targetPos;
    }
}
