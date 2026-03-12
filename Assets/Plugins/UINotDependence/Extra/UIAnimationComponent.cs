using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

[Serializable]
public class UIAnimationPreset
{
    public float duration = 0.3f;
    public Ease easeType = Ease.OutBack;
    public Vector3 scale = Vector3.one;
    public Vector3 position = Vector3.one;
    public float alpha = 1f;
}

[RequireComponent(typeof(CanvasGroup))]
public class UIAnimationComponent : MonoBehaviour
{
    [Header("Presets")]
    [SerializeField] private UIAnimationPreset _openPreset = new() { scale = Vector3.one, alpha = 1f };
    [SerializeField] private UIAnimationPreset _closePreset = new() { scale = Vector3.zero, alpha = 0f };

    [Header("Settings")]
    [SerializeField] private bool _disableOnClose = true;

    [Header("References")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private GraphicRaycaster _raycaster;

    private Sequence _activeSequence;

    private void Awake()
    {
        EnsureReferences();
        SetState(_closePreset);
    }

    private void EnsureReferences()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        if (_raycaster == null) _raycaster = GetComponent<GraphicRaycaster>();
    }

    public void PlayOpen(Action onComplete = null)
    {
        gameObject.SetActive(true);
        ToggleRaycasts(false);
        Animate(_openPreset, () =>
        {
            ToggleRaycasts(true);
            onComplete?.Invoke();
        });
    }

    public void PlayClose(Action onComplete = null)
    {
        ToggleRaycasts(false);
        Animate(_closePreset, () =>
        {
            if (_disableOnClose) gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    public void Animate(UIAnimationPreset preset, Action onComplete)
    {
        KillCurrent();

        _activeSequence = DOTween.Sequence()
            .SetTarget(this)
            .Append(_canvasGroup.DOFade(preset.alpha, preset.duration).SetEase(preset.easeType))
            .Join(_rectTransform.DOScale(preset.scale, preset.duration).SetEase(preset.easeType))
            .Join(_rectTransform.DOAnchorPos(preset.position, preset.duration).SetEase(preset.easeType))
            .OnComplete(() => onComplete?.Invoke())
            .Play();
    }

    private void SetState(UIAnimationPreset preset)
    {
        KillCurrent();
        if (_canvasGroup) _canvasGroup.alpha = preset.alpha;
        if (_rectTransform) _rectTransform.localScale = preset.scale;
        ToggleRaycasts(preset.alpha > 0.9f);
    }

    private void ToggleRaycasts(bool enabled)
    {
        if (_canvasGroup) _canvasGroup.blocksRaycasts = enabled;
        if (_raycaster) _raycaster.enabled = enabled;
    }

    public void KillCurrent()
    {
        _activeSequence?.Kill();
    }

    private void OnDestroy() => KillCurrent();

    public static UIAnimationComponent Using(GameObject go) =>
        go.TryGetComponent(out UIAnimationComponent anim) ? anim : go.AddComponent<UIAnimationComponent>();

    public UIAnimationComponent Setup(CanvasGroup cg = null, RectTransform rt = null)
    {
        if (cg != null) _canvasGroup = cg;
        if (rt != null) _rectTransform = rt;
        return this;
    }

    public UIAnimationComponent SetPresets(UIAnimationPreset open, UIAnimationPreset close)
    {
        _openPreset = open;
        _closePreset = close;
        return this;
    }
}
