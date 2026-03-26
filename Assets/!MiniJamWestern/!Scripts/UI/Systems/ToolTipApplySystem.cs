using BitterECS.Core;
using UINotDependence.Core;
using UnityEngine;

public class ToolTipApplySystem : IEcsAutoImplement, IEcsRunSystem
{
    public Priority Priority => Priority.Medium;

    private const float TooltipDelay = 0.3f;

    private EcsEntity? _pendingEntity = null;
    private float _pendingTime = 0f;

    private static ToolTipApplySystem _instance;

    private EcsEvent _ecsEventEnter = new EcsEvent()
        .Subscribe<IsHover>(added: OnPointerEnter, removed: OnPointerExit);

    public ToolTipApplySystem()
    {
        _instance = this;
    }

    private static void OnPointerEnter(EcsEntity entity)
    {
        if (entity.Has<IsNotHover>()) return;
        _instance.ScheduleTooltip(entity);
    }

    private static void OnPointerExit(EcsEntity entity)
    {
        if (entity.Has<IsNotHover>()) return;

        _instance.CancelScheduledTooltip();
        UIController.ClosePopup<UITooltipPopup>();
    }

    private void ScheduleTooltip(EcsEntity entity)
    {
        if (_pendingEntity.HasValue && _pendingEntity.Value == entity)
            return;

        CancelScheduledTooltip();
        UIController.ClosePopup<UITooltipPopup>();

        _pendingEntity = entity;
        _pendingTime = Time.time + TooltipDelay;
    }

    private void CancelScheduledTooltip()
    {
        _pendingEntity = null;
    }

    public void Run()
    {
        if (_pendingEntity.HasValue && Time.time >= _pendingTime)
        {
            var entity = _pendingEntity.Value;
            if (entity.IsAlive && entity.Has<SoldInfoComponent>())
            {
                ref var soldInfo = ref entity.Get<SoldInfoComponent>();
                var popup = UIController.OpenPopup<UITooltipPopup>();
                popup.Bind(soldInfo, soldInfo.amount);
            }

            _pendingEntity = null;
        }
    }

}
