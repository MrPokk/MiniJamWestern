using DG.Tweening;
using UnityEngine;

public static class UIAnimationPresets
{
    // --- POPUP (Масштабирование с отскоком) ---
    public static UIAnimationPreset PopupOpen => new()
    {
        duration = 0.4f,
        easeType = Ease.OutBack,
        scale = Vector3.one,
        alpha = 1f
    };

    public static UIAnimationPreset PopupClose => new()
    {
        duration = 0.25f,
        easeType = Ease.InBack,
        scale = Vector3.zero,
        alpha = 0f
    };

    // --- SCREEN / FADE (Плавное появление) ---
    public static UIAnimationPreset FadeIn => new()
    {
        duration = 0.3f,
        easeType = Ease.OutCubic,
        scale = Vector3.one,
        alpha = 1f
    };

    public static UIAnimationPreset FadeOut => new()
    {
        duration = 0.25f,
        easeType = Ease.InCubic,
        scale = Vector3.one, // Масштаб не меняем
        alpha = 0f
    };

    // --- SLIDE (Выезд справа) ---
    // Внимание: position зависит от ширины экрана, 800 — это примерный оффсет
    public static UIAnimationPreset SlideRightIn => new()
    {
        duration = 0.4f,
        easeType = Ease.OutCubic,
        position = Vector3.zero,
        alpha = 1f
    };

    public static UIAnimationPreset SlideRightOut => new()
    {
        duration = 0.3f,
        easeType = Ease.InCubic,
        position = new Vector3(800, 0, 0),
        alpha = 0f
    };

    // --- BOUNCE (Сильный отскок) ---
    public static UIAnimationPreset BounceIn => new()
    {
        duration = 0.6f,
        easeType = Ease.OutBounce,
        scale = Vector3.one,
        alpha = 1f
    };
}
