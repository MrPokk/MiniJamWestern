using BitterECS.Core;
using System.Collections.Generic; // Добавьте этот namespace

public class EnemyExecutionSystem : IUpdateTurn
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<GridComponent, ListActionComponent, IsIntentComponent, EnemyStateComponent> _enemies;

    private readonly List<EcsEntity> _entitiesBuffer = new();

    public void RefreshTurn()
    {
        _entitiesBuffer.Clear();
        _enemies.For((EcsEntity entity, ref GridComponent _, ref ListActionComponent _, ref IsIntentComponent _, ref EnemyStateComponent _) =>
        {
            _entitiesBuffer.Add(entity);
        });

        foreach (var entity in _entitiesBuffer)
        {
            if (!entity.IsAlive) continue;
            if (!entity.Has<IsIntentComponent>() || !entity.Has<EnemyStateComponent>()) continue;

            ref var grid = ref entity.Get<GridComponent>();
            ref var list = ref entity.Get<ListActionComponent>();
            ref var intent = ref entity.Get<IsIntentComponent>();
            ref var state = ref entity.Get<EnemyStateComponent>();

            switch (state.state)
            {
                case EnemyState.Preparing:
                    ProcessPreparation(entity, grid, ref intent, ref state);
                    break;

                case EnemyState.ReadyToExecute:
                    ProcessExecution(entity, ref grid, list, ref intent, ref state);
                    break;
            }
        }
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
