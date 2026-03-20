using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

public class AbilitySlotViewSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.High;

    public bool Placing(AbilityViewProvider item, AbilitySlotProvider slotProvider)
    {
        var slotEntity = slotProvider.Entity;
        ref var abilitySlot = ref slotEntity.Get<AbilitySlotComponent>();

        if (abilitySlot.itemEntity.IsAlive)
        {
            Debug.Log("Slot already occupied by another item.");
            return false;
        }

        item.Entity.Remove<IsDraggingAbility>();
        abilitySlot.itemEntity = item.Entity;

        item.transform.SetParent(slotProvider.transform);
        item.transform.localPosition = Vector3.zero;

        if (slotEntity.Has<ScalingFactorComponent>())
        {
            var scalingData = slotEntity.Get<ScalingFactorComponent>();

            var itemBaseSize = Vector2.one;
            if (item.Entity.Has<ScalingFactorComponent>())
            {
                itemBaseSize = item.Entity.Get<ScalingFactorComponent>().slotSize;
            }

            var scaleX = scalingData.slotSize.x / itemBaseSize.x;
            var scaleY = scalingData.slotSize.y / itemBaseSize.y;

            item.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }

        return true;
    }

    public bool Extract(AbilitySlotProvider slotProvider)
    {
        ref var abilitySlot = ref slotProvider.Entity.Get<AbilitySlotComponent>();
        if (!abilitySlot.itemEntity.IsAlive)
            return false;

        var itemEntity = abilitySlot.itemEntity;
        var provider = itemEntity.GetProvider<ProviderEcs>();

        if (provider != null)
        {
            provider.transform.SetParent(null);
            provider.transform.localScale = Vector3.one;
        }

        abilitySlot.itemEntity = EcsEntity.Null;
        return true;
    }
}
