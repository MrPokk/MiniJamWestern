using BitterECS.Core;

public class ActionTransferProgressSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.Medium;


    private static void OnTarget(EcsEntity entity)
    {
        if (!entity.Has<TagPlayer>())
            return;

        GFlow.AddTransferProgress(1);
    }
}
