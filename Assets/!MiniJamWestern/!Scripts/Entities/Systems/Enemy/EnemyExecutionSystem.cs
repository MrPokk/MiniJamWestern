using BitterECS.Core;

public class EnemyExecutionSystem : IUpdateTurn
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<GridComponent, ListActionComponent, IsIntentComponent> _enemiesWithIntent;

    public void RefreshTurn()
    {
        _enemiesWithIntent.For((EcsEntity entity, ref GridComponent grid, ref ListActionComponent list, ref IsIntentComponent intent) =>
        {
            ref var target = ref entity.GetOrAdd<TargetTo>();
            target.position = intent.targetPosition;

            AbilityLogicRouter.Execute(entity, intent.chosenAbility, ref grid, list, ref target);

            entity.Remove<IsIntentComponent>();
        });
    }
}
