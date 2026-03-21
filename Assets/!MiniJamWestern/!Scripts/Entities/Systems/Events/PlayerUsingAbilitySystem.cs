using BitterECS.Core;
using UnityEngine;

public class PlayerUsingAbilitySystem : IEcsInitSystem
{
    public Priority Priority => Priority.Low;
    private EcsEvent _ecsEvent;
    private EcsFilter<TagPlayer, GridComponent> _playerFilter;
    private EcsFilter<TagInventoryEffects> _effectsFilter;

    public void Init() => _ecsEvent.Subscribe<ShortPressAbilityEvent>(OnShortPress);

    private void OnShortPress(EcsEntity abilityEntity)
    {

        if (!TryGetContext(abilityEntity, out var player, out var action, out var mainList)) return;

        EcsSystemStatic.GetSystem<PlayerTargetingSystem>().Targeting();


        var playerFilter = _playerFilter.First();
        Debug.Log($"{playerFilter.Id} {playerFilter.Has<TagPlayer>()} {playerFilter.Has<GridComponent>()} ");
        ref var grid = ref playerFilter.GetOrAdd<GridComponent>();
        ref var target = ref playerFilter.GetOrAdd<TargetTo>();

        AbilityLogicRouter.Execute(playerFilter, action.ability, ref grid, mainList, ref target);
        ExecuteEffects(playerFilter, action.ability, ref grid, ref target);

        GFlow.MinusTransferProgress(1);
    }

    private void ExecuteEffects(EcsEntity player, IActionAbility primary, ref GridComponent grid, ref TargetTo target)
    {
        foreach (var invEntity in _effectsFilter)
        {
            var inv = invEntity.GetProvider<AbilityInventoryProvider>();
            if (inv == null) continue;

            foreach (var slot in inv.Value.listSlot)
            {
                if (slot.Entity.TryGet<AbilitySlotLimitComponent>(out var limit) && !limit.IsAllowed(primary)) continue;
                if (!slot.Value.itemEntity.TryGet<TagActions>(out var action) || action.ability == null) continue;

                var tempList = new ListActionComponent { abilities = new() { action } };
                AbilityLogicRouter.Execute(player, action.ability, ref grid, tempList, ref target);
            }
        }
    }

    private bool TryGetContext(EcsEntity entity, out EcsEntity player, out TagActions action, out ListActionComponent list)
    {
        player = _playerFilter.First();
        action = default;
        list = default;
        var view = entity.GetProvider<AbilityViewProvider>();
        var slot = view?.GetComponentInParent<AbilitySlotProvider>();
        var owner = slot?.Value.abilityInventory.Entity ?? default;
        return GFlow.GState.TransferProgress > 0 && player.IsAlive && entity.TryGet(out action) && owner.TryGet(out list);
    }
}
