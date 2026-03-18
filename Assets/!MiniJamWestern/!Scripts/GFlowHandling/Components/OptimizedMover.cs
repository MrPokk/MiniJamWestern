using UnityEngine;
using DG.Tweening;

public class OptimizedMover : MonoBehaviour
{
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float duration = 2f;
    [SerializeField] private Ease easeType = Ease.Linear;
    [SerializeField] private bool isRelative = true;
    [SerializeField] private LoopType loopType = LoopType.Yoyo;

    private Tween moveTween;

    private void Start()
    {
        InitializeTween();
    }

    private void InitializeTween()
    {
        moveTween = transform.DOMove(targetPosition, duration)
            .SetEase(easeType)
            .SetLoops(-1, loopType)
            .SetRelative(isRelative)
            .SetAutoKill(false)
            .SetLink(gameObject)
            .SetUpdate(UpdateType.Normal, true);
    }

    private void OnEnable()
    {
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Play();
        }
    }

    private void OnDisable()
    {
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Pause();
        }
    }

    private void OnDestroy()
    {
        if (moveTween != null)
        {
            moveTween.Kill();
        }
    }
}
