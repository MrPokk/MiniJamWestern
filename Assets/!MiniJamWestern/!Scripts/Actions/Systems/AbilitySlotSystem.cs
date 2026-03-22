using BitterECS.Core;

public class AbilitySlotSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    public bool Placing(EcsEntity ownerInventory, EcsEntity slotEntity, EcsEntity itemEntity)
    {
        if (ownerInventory.Has<IsNotPlace>() || slotEntity.Has<IsNotPlace>()) return false;

        if (!itemEntity.TryGet<TagActions>(out var action)) return false;

        if (slotEntity.TryGet<AbilitySlotLimitComponent>(out var limit))
        {
            if (!limit.IsAllowed(action)) return false;
        }

        itemEntity.Add<IsPlace>();
        itemEntity.Remove<IsExtract>();
        AddAbilityToOwner(ownerInventory, action);

        return true;
    }

    private static void AddAbilityToOwner(EcsEntity ownerEntity, TagActions action)
    {
        var listComponent = ownerEntity.GetOrAdd<ListActionComponent>();
        listComponent.AddAbility(action, ownerEntity);
    }

    public bool Extract(EcsEntity ownerEntity, EcsEntity slotEntity, EcsEntity removedItemEntity)
    {
        if (ownerEntity.Has<IsNotExtract>() || slotEntity.Has<IsNotExtract>()) return false;

        if (!removedItemEntity.TryGet<TagActions>(out var action)) return false;

        removedItemEntity.Add<IsExtract>();
        removedItemEntity.Remove<IsPlace>();
        return RemoveAbilityToOwner(ownerEntity, action);
    }

    private static bool RemoveAbilityToOwner(EcsEntity ownerEntity, TagActions action)
    {
        if (!ownerEntity.Has<ListActionComponent>())
        {
            return false;
        }

        var listComponent = ownerEntity.Get<ListActionComponent>();
        listComponent.RemoveAbility(action);
        if (listComponent.abilities.Count == 0)
        {
            ownerEntity.Remove<ListActionComponent>();
        }

        return true;
    }
}

public struct IsExtract
{ }

public struct IsPlace
{ }
