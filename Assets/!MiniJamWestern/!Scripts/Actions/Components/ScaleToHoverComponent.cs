using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ScaleToHoverComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Hover Settings")]
    [SerializeField] private float _hoverScaleMultiplier = 1.1f;
    [SerializeField] private float _hoverDuration = 0.2f;
    [SerializeField] private Ease _hoverEase = Ease.OutBack;

    [Header("Click Settings (Push)")]
    [SerializeField] private Vector3 _punchAmount = new Vector3(-0.1f, -0.1f, -0.1f);
    [SerializeField] private float _punchDuration = 0.2f;
    [SerializeField] private int _vibrato = 10;
    [SerializeField] private float _elasticity = 1f;

    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateScale(_originalScale * _hoverScaleMultiplier);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateScale(_originalScale);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOPunchScale(_punchAmount, _punchDuration, _vibrato, _elasticity)
            .SetUpdate(true)
            .Play();
    }

    private void AnimateScale(Vector3 target)
    {
        transform.DOKill();
        transform.DOScale(target, _hoverDuration)
            .SetEase(_hoverEase)
            .SetUpdate(true)
            .Play();
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}
