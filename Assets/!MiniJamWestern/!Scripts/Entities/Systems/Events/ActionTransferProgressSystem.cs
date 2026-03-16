using BitterECS.Core;

public class ActionTransferProgressSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.Medium;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<IsAttackerTo>(added: OnTarget);

    private static void OnTarget(EcsEntity entity)
    {
        if (!entity.Has<TagPlayer>())
            return;

        GFlow.AddTransferProgress(1);
    }
}
