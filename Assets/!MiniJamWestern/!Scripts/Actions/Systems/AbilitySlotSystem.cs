using BitterECS.Core;

public class AbilitySlotSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    public void Placing(EcsEntity ownerEntity, EcsEntity addedItemEntity)
    {
        if (!addedItemEntity.TryGet<TagActions>(out var action)) return;

        addedItemEntity.Add<IsPlace>();
        addedItemEntity.Remove<IsExtract>();

        ownerEntity.AddFrame<IsTargetingActionEnterEvent>(new(action.ability));
    }

    public void Extract(EcsEntity ownerEntity, EcsEntity removedItemEntity)
    {
        if (!removedItemEntity.TryGet<TagActions>(out var action)) return;
        removedItemEntity.Add<IsExtract>();
        removedItemEntity.Remove<IsPlace>();
        ownerEntity.AddFrame<IsTargetingActionExitEvent>(new(action.ability));
    }
}

public struct IsExtract
{ }

public struct IsPlace
{ }
