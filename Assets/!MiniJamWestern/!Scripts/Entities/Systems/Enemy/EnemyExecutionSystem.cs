using BitterECS.Core;

public class EnemyExecutionSystem : IUpdateTurn
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<GridComponent, ListActionComponent, IsIntentComponent, EnemyStateComponent> _enemies;

    public void RefreshTurn()
    {
        _enemies.For((EcsEntity entity, ref GridComponent grid, ref ListActionComponent list, ref IsIntentComponent intent, ref EnemyStateComponent state) =>
        {
            if (!entity.IsAlive) return;

            switch (state.state)
            {
                case EnemyState.Preparing:
                    ProcessPreparation(entity, grid, ref intent, ref state);
                    break;

                case EnemyState.ReadyToExecute:
                    ProcessExecution(entity, ref grid, list, ref intent, ref state);
                    break;
            }
        });
    }

    private void ProcessPreparation(EcsEntity entity, GridComponent grid, ref IsIntentComponent intent, ref EnemyStateComponent state)
    {
        if (VectorUtility.TryGetStepDirection(grid.currentPosition, intent.targetPosition, out var dir))
        {
            entity.GetOrAdd<FacingComponent>().direction = dir;
        }

        state.state = EnemyState.ReadyToExecute;
    }

    private void ProcessExecution(EcsEntity entity, ref GridComponent grid, ListActionComponent list, ref IsIntentComponent intent, ref EnemyStateComponent state)
    {
        ref var target = ref entity.GetOrAdd<TargetTo>();
        target.position = intent.targetPosition;

        AbilityLogicRouter.Execute(entity, intent.chosenAbility, ref grid, list, ref target);

        state.state = EnemyState.Thinking;
        entity.Remove<IsIntentComponent>();
    }
}
