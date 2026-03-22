using BitterECS.Integration.Unity;

public enum EnemyState { Thinking, Preparing, ReadyToExecute }

public struct EnemyStateComponent
{
    [ReadOnly] public EnemyState state;
}

public class EnemyStateProvider : ProviderEcs<EnemyStateComponent>
{

}
