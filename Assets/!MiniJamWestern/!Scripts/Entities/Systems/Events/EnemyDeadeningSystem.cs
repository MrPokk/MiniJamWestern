using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;

public class EnemyDeadeningSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;
    private EcsEvent _ecsEvent = new EcsEvent().SubscribeWhereEntity<IsDeadEvent>(e => e.Has<TagEnemy>(), added: OnDead);
    private static void OnDead(EcsEntity entity)
    {
        var container = entity.GetProvider<ProviderEcs>().GetComponent<ContainerActionsProvider>();
        container.ExtractAll();

        entity.Destroy();
    }
}
