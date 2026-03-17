using BitterECS.Core;
using UnityEngine;

public class AbilityShortPressSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.Low;

    private EcsEvent _ecsEvent = new EcsEvent()
        .Subscribe<ShortPressAbilityEvent>(added: OnShortPress);

    private static void OnShortPress(EcsEntity abilityEntity)
    {
        var view = abilityEntity.GetProvider<AbilityViewProvider>();
        if (view == null) return;

        var slot = view.GetComponentInParent<AbilitySlotProvider>();
        if (slot == null) return;

        var ownerEntity = slot.Value.abilityInventory.Entity;
        if (!ownerEntity.Has<TagPlayer>())
        {
            return;
        }

        if (!abilityEntity.TryGet<TagActions>(out var action)) return;
        if (GFlow.GState.TransferProgress <= 0)
        {
            return;
        }

        EcsSystemStatic.GetSystem<PlayerTargetingSystem>().Targeting();
        AbilityLogicRouter.Execute(ownerEntity, action.ability);
        GFlow.MinusTransferProgress(1);
    }
}
