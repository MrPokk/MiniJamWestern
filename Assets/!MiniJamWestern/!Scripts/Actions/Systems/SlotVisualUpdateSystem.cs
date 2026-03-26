using BitterECS.Core;
using UnityEngine;

public class SlotVisualUpdateSystem : IEcsRunSystem
{
    public Priority Priority => Priority.Low;

    private const float Amplitude = 0.5f;
    private const float Frequency = 0.3f;
    private const float PhaseOffset = 0.3f;

    private EcsFilter<AbilityInventory, TagInventoryEffects> _ecsEntities;

    public void Run()
    {
        var time = Time.time;

        _ecsEntities.For((EcsEntity entity, ref AbilityInventory abilityInventory, ref TagInventoryEffects _) =>
        {
            if (abilityInventory.listSlot == null) return;

            for (var i = 0; i < abilityInventory.listSlot.Count; i++)
            {
                var slot = abilityInventory.listSlot[i];
                if (slot == null || !slot.gameObject.activeSelf) continue;

                var slotComp = slot.Entity.Get<AbilitySlotComponent>();

                var xOffset = Mathf.Sin(time * Frequency + i * PhaseOffset) * Amplitude;

                var targetPos = slotComp.initialLocalPos;
                targetPos.x += xOffset;

                slot.transform.localPosition = targetPos;
            }
        });
    }
}
