using System;
using BitterECS.Core;
using InGame.Script.Component_Sound;

public class SoundToEventSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.Low;

    private EcsEvent _isHover = new EcsEvent()
    .Subscribe<IsHover>(added: OnSoundHover);

    private EcsEvent _isClick = new EcsEvent()
    .Subscribe<ShortPressAbilityEvent>(added: OnSoundClick);

    private EcsEvent _isDamage = new EcsEvent()
    .Subscribe<IsDamagedEvent>(added: OnSoundDamage);


    private EcsEvent _isMove = new EcsEvent()
    .Subscribe<IsMovingEvent>(added: OnSoundMove);

    private static void OnSoundMove(EcsEntity entity)
    {
        SoundController.PlaySoundRandomPitch(SoundType.Walking);
    }

    private static void OnSoundDamage(EcsEntity entity)
    {
        SoundController.PlaySoundRandomPitch(SoundType.DamageHit);
    }

    private static void OnSoundClick(EcsEntity entity)
    {
        if (!entity.Has<IsNotClick>())
            SoundController.PlaySoundRandomPitch(SoundType.Hover);
    }

    private static void OnSoundHover(EcsEntity entity)
    {
        SoundController.PlaySoundRandomPitch(SoundType.Hover);
    }
}
