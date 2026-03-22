using BitterECS.Core;

public class ResetIntentDisplaySystem : IEcsInitSystem
{
    public Priority Priority => Priority.High;
    private EcsEvent _ecsEvent;

    public void Init()
    {
        _ecsEvent.Subscribe<IsDeadEvent>(added: OnReset);
        _ecsEvent.Subscribe<IsIntentComponent>(removed: OnReset);
    }

    public static void OnReset(EcsEntity entity)
    {
        DrawRectUtility.Instance?.HideStaticRect(entity.GetHashCode());
        DrawRectUtility.Instance?.HideStaticFullRect(entity.GetHashCode());

        IntentVisualUtility.ClearVisuals(entity);

        if (entity.TryGet<OutlineComponent>(out var outlineComp))
        {
            outlineComp.SetOutlineColor(outlineComp.defaultOutlineColor);
        }
    }
}
