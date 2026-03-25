using BitterECS.Core;
using UnityEngine;

public class EnemySkirmisherBrainSystem : IUpdateTurn
{
    public Priority Priority => Priority.High;

    private EcsFilter<GridComponent, TagPlayer> _playerFilter;
    private EcsFilter<GridComponent, ListActionComponent, EnemyStateComponent, TagBehaviorSkirmisher> _enemies;

    public void RefreshTurn()
    {
        var player = _playerFilter.First();
        if (!player.IsAlive) return;

        var playerPos = player.Get<GridComponent>().currentPosition;

        _enemies.For((
            EcsEntity entity,
            ref GridComponent grid,
            ref ListActionComponent list,
            ref EnemyStateComponent state,
            ref TagBehaviorSkirmisher _) =>
        {
            if (state.state != EnemyState.Thinking) return;

            ref var sequence = ref entity.GetOrAdd<TagBehaviorSkirmisher>();
            var myPos = grid.currentPosition;
            var dist = EnemyBrainUtility.GetDistance(myPos, playerPos);
            VectorUtility.TryGetStepDirection(myPos, playerPos, out var dirToPlayer);

            ProcessBehaviorPhase(entity, list, ref state, ref sequence, ref grid, myPos, playerPos, dirToPlayer, dist);
        });
    }

    private void ProcessBehaviorPhase(
        EcsEntity entity,
        ListActionComponent list,
        ref EnemyStateComponent state,
        ref TagBehaviorSkirmisher sequence,
        ref GridComponent grid,
        Vector2Int myPos,
        Vector2Int playerPos,
        Vector2Int dirToPlayer,
        int dist)
    {
        var attackDist = GetAttackDistance(list);

        if (sequence.phase == SkirmisherPhase.Attack && dist > attackDist)
        {
            sequence.phase = SkirmisherPhase.Approach;
        }

        var actionTaken = false;
        var safetyLimit = 0;

        while (!actionTaken && safetyLimit < 4)
        {
            safetyLimit++;

            switch (sequence.phase)
            {
                case SkirmisherPhase.Approach:
                    actionTaken = TryApproach(entity, list, ref state, ref sequence, ref grid, myPos, dirToPlayer, dist, attackDist);
                    break;

                case SkirmisherPhase.Attack:
                    actionTaken = TryAttack(entity, list, ref state, ref sequence, playerPos, dist, attackDist);
                    break;

                case SkirmisherPhase.Retreat:
                    actionTaken = TryRetreat(entity, list, ref state, ref sequence, ref grid, myPos, dirToPlayer);
                    break;

                case SkirmisherPhase.FinalAttack:
                    actionTaken = TryFinalAttack(entity, list, ref state, ref sequence, playerPos, dist, attackDist);
                    break;
            }
        }
    }

    private bool TryApproach(
        EcsEntity entity,
        ListActionComponent list,
        ref EnemyStateComponent state,
        ref TagBehaviorSkirmisher sequence,
        ref GridComponent grid,
        Vector2Int myPos,
        Vector2Int dirToPlayer,
        int dist,
        int attackDist)
    {
        if (dist > attackDist)
        {
            var targetPos = myPos + dirToPlayer;
            if (grid.gridPresenter.IsWithinGrid(targetPos))
            {
                return EnemyBrainUtility.TryAction<TagMoveForward>(entity, list, targetPos, true, ref state);
            }
            else
            {
                // Движение невозможно из-за границ поля – переходим в Retreat, чтобы попробовать отступить
                sequence.phase = SkirmisherPhase.Retreat;
                return false;
            }
        }

        sequence.phase = SkirmisherPhase.Attack;
        return false;
    }

    private bool TryAttack(
        EcsEntity entity,
        ListActionComponent list,
        ref EnemyStateComponent state,
        ref TagBehaviorSkirmisher sequence,
        Vector2Int playerPos,
        int dist,
        int attackDist)
    {
        var success = EnemyBrainUtility.TryAction<TagAttackForward>(entity, list, playerPos, dist <= attackDist, ref state);

        sequence.phase = success ? SkirmisherPhase.Retreat : SkirmisherPhase.Approach;
        return success;
    }

    private bool TryRetreat(
        EcsEntity entity,
        ListActionComponent list,
        ref EnemyStateComponent state,
        ref TagBehaviorSkirmisher sequence,
        ref GridComponent grid,
        Vector2Int myPos,
        Vector2Int dirToPlayer)
    {
        var retreatPos = myPos - dirToPlayer;
        if (grid.gridPresenter.IsWithinGrid(retreatPos))
        {
            var success = EnemyBrainUtility.TryAction<TagMoveForward>(entity, list, retreatPos, true, ref state);
            sequence.phase = SkirmisherPhase.FinalAttack;
            return success;
        }
        else
        {
            // Отступление невозможно из-за границ – переходим сразу в FinalAttack
            sequence.phase = SkirmisherPhase.FinalAttack;
            return false;
        }
    }

    private bool TryFinalAttack(
        EcsEntity entity,
        ListActionComponent list,
        ref EnemyStateComponent state,
        ref TagBehaviorSkirmisher sequence,
        Vector2Int playerPos,
        int dist,
        int attackDist)
    {
        var success = EnemyBrainUtility.TryAction<TagAttackForward>(entity, list, playerPos, dist <= attackDist, ref state);

        sequence.phase = SkirmisherPhase.Approach;
        return success;
    }

    private int GetAttackDistance(ListActionComponent list)
    {
        if (list.abilities == null)
        {
            throw new System.Exception("Ability is null");
        }

        if (list.Is<TagAttackForward>(out var forward))
        {
            return forward.value;
        }

        throw new System.Exception("Ability is null");
    }
}
