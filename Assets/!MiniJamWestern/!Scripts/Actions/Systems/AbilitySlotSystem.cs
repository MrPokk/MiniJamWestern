using BitterECS.Core;

public class AbilitySlotSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    public bool Placing(EcsEntity ownerEntity, EcsEntity slotEntity, EcsEntity addedItemEntity)
    {
        if (ownerEntity.Has<IsNotPlace>() || slotEntity.Has<IsNotPlace>()) return false;

        if (!addedItemEntity.TryGet<TagActions>(out var action)) return false;

        addedItemEntity.Add<IsPlace>();
        addedItemEntity.Remove<IsExtract>();
        AddAbilityToOwner(ownerEntity, action);
        return true;
    }

    private static void AddAbilityToOwner(EcsEntity ownerEntity, TagActions action)
    {
        var abilityFromEvent = action.ability;
        var listComponent = ownerEntity.GetOrAdd<ListActionComponent>();
        listComponent.AddAbility(abilityFromEvent);
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
        var abilityToRemove = action.ability;
        if (!ownerEntity.Has<ListActionComponent>())
        {
            return false;
        }

        var listComponent = ownerEntity.Get<ListActionComponent>();
        listComponent.RemoveAbility(abilityToRemove);
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
