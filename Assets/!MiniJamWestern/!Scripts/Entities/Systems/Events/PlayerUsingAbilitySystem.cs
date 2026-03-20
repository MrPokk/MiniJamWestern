using BitterECS.Core;
using UnityEngine;

public class PlayerUsingAbilitySystem : IEcsInitSystem
{
    public Priority Priority => Priority.Low;

    private EcsEvent _ecsEvent;
    private EcsFilter<TagPlayer> _ecsEntities;

    public void Init()
    {
        _ecsEvent.Subscribe<ShortPressAbilityEvent>(added: OnShortPress);
    }

    private void OnShortPress(EcsEntity abilityEntity)
    {
        var view = abilityEntity.GetProvider<AbilityViewProvider>();
        if (view == null) return;

        var slot = view.GetComponentInParent<AbilitySlotProvider>();
        if (slot == null) return;

        var ownerEntity = slot.Value.abilityInventory.Entity;

        if (!ownerEntity.Has<TagInventoryUsing>()) return;
        if (!abilityEntity.TryGet<TagActions>(out var action)) return;
        if (GFlow.GState.TransferProgress <= 0) return;


        EcsSystemStatic.GetSystem<PlayerTargetingSystem>().Targeting();

        var player = _ecsEntities.First();
        if (!player.IsAlive) return;

        if (!player.Has<GridComponent>() ||
            !player.Has<TargetTo>() ||
            !ownerEntity.Has<ListActionComponent>())
        {
            Debug.LogWarning("Missing required components for Ability Execution");
            return;
        }


        ref var grid = ref player.Get<GridComponent>();
        ref var target = ref player.Get<TargetTo>();
        var list = ownerEntity.Get<ListActionComponent>();

        AbilityLogicRouter.Execute(player, action.ability, ref grid, list, ref target);

        GFlow.MinusTransferProgress(1);
    }
}
