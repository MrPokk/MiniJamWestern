using BitterECS.Core;

public class AbilitySlotSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    public void Placing(EcsEntity ownerEntity, EcsEntity addedItemEntity)
    {
        if (!addedItemEntity.TryGet<TagActions>(out var action)) return;

        ownerEntity.AddFrame<IsTargetingActionEnterEvent>(new(action.ability));
    }

    public void Extract(EcsEntity ownerEntity, EcsEntity removedItemEntity)
    {
        if (!removedItemEntity.TryGet<TagActions>(out var action)) return;

        ownerEntity.AddFrame<IsTargetingActionExitEvent>(new(action.ability));
    }
}
