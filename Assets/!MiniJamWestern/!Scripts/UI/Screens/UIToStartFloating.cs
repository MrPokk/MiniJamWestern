using System; // <-- Добавлено для Action
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UINotDependence.Core;
using InGame.Script.Component_Sound;

public class UIToStartFloating : UIScreen, IPointerClickHandler
{
    // === ДОБАВЛЕНО СОБЫТИЕ ЗАВЕРШЕНИЯ ===
    public static event Action OnFloatingFinished;

    [Header("UI Elements")]
    [Tooltip("Добавьте сюда картинки (Image) по порядку (например, 3 штуки)")]
    [SerializeField] private List<Image> _images; [Header("Animation Settings")]
    [SerializeField] private float _fadeDuration = 0.5f; [Header("Shake Settings (On Transition & Close)")]
    [SerializeField] private float _shakePositionStrength = 20f;
    [SerializeField] private float _shakeRotationStrength = 15f; [SerializeField] private int _shakeVibrato = 10;
    [SerializeField] private float _shakeScaleStrength = 0.1f;

    [Header("Auto Transition Settings")]
    [SerializeField] private float _autoTransitionDelay = 3f;

    [Header("Parallax Settings")]
    [SerializeField] private float _parallaxStrength = 25f;
    [SerializeField] private float _parallaxSmoothness = 5f;

    private bool _isClosing = false;
    private bool _isTransitioning = false;
    private int _currentStage = 0;
    private float _currentAutoTimer;

    private Vector2[] _startPositions;
    private Vector2[] _parallaxFactors;

    private static bool s_isOpenSwipingPopups;

    private void Awake()
    {
        if (s_isOpenSwipingPopups)
        {
            UIController.CloseScreen();
            return;
        }

        s_isOpenSwipingPopups = true;

        if (_images != null && _images.Count > 0)
        {
            _startPositions = new Vector2[_images.Count];
            _parallaxFactors = new Vector2[_images.Count];

            for (int i = 0; i < _images.Count; i++)
            {
                if (_images[i] != null)
                {
                    _startPositions[i] = _images[i].rectTransform.anchoredPosition;
                    _parallaxFactors[i] = new Vector2(UnityEngine.Random.Range(0.8f, 1.2f), UnityEngine.Random.Range(0.8f, 1.2f));
                }
            }
        }
    }

    private void OnDestroy()
    {
        s_isOpenSwipingPopups = false;
    }

    private void OnEnable()
    {
        ResetPopup();
    }

    private void OnDisable()
    {
        if (_images != null)
        {
            foreach (var img in _images)
            {
                if (img != null)
                {
                    img.rectTransform.DOKill();
                    img.DOKill();
                }
            }
        }
    }

    private void Update()
    {
        if (_isClosing || _images == null || _images.Count == 0 || _parallaxFactors == null) return;

        if (!_isTransitioning)
        {
            _currentAutoTimer -= Time.unscaledDeltaTime;
            if (_currentAutoTimer <= 0f)
            {
                ExecuteNextStage();
            }
        }

        var mousePos = ControllableSystem.PointerPosition;
        var screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        var offset = (mousePos - screenCenter) / screenCenter;

        ApplyParallax(offset);
    }

    private void ApplyParallax(Vector2 offset)
    {
        for (int i = 0; i < _images.Count; i++)
        {
            if (_images[i] == null) continue;
            if (i < _currentStage) continue;
            if (_isTransitioning && i == _currentStage) continue;

            var uniqueOffset = new Vector2(offset.x * _parallaxFactors[i].x, offset.y * _parallaxFactors[i].y);

            var floatOffset = new Vector2(
                Mathf.Sin(Time.unscaledTime * 1.5f + i) * 3f,
                Mathf.Cos(Time.unscaledTime * 1.2f + i) * 3f
            );

            var targetPos = _startPositions[i] + (uniqueOffset * _parallaxStrength) + floatOffset;

            _images[i].rectTransform.anchoredPosition = Vector2.Lerp(
                _images[i].rectTransform.anchoredPosition,
                targetPos,
                Time.unscaledDeltaTime * _parallaxSmoothness);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ExecuteNextStage();
    }

    private void ExecuteNextStage()
    {
        if (_isClosing || _isTransitioning || _images == null || _images.Count == 0) return;

        SoundController.PlaySoundRandomPitch(SoundType.Click);

        if (_currentStage < _images.Count - 1)
        {
            _isTransitioning = true;
            var currentImage = _images[_currentStage];
            var nextImage = _images[_currentStage + 1];

            var seq = DOTween.Sequence();
            seq.SetUpdate(true);

            seq.Join(currentImage.DOFade(0f, _fadeDuration).SetEase(Ease.InOutQuad));
            seq.Join(currentImage.rectTransform.DOShakePosition(_fadeDuration, _shakePositionStrength, _shakeVibrato, 90f, false, true));
            seq.Join(currentImage.rectTransform.DOShakeRotation(_fadeDuration, new Vector3(0, 0, _shakeRotationStrength), _shakeVibrato, 90f));
            seq.Join(currentImage.rectTransform.DOShakeScale(_fadeDuration, _shakeScaleStrength, _shakeVibrato, 90f));

            seq.Join(nextImage.DOFade(1f, _fadeDuration).SetEase(Ease.InOutQuad));

            seq.OnComplete(() =>
            {
                _isTransitioning = false;
                _currentStage++;
                _currentAutoTimer = _autoTransitionDelay;
            });

            seq.Play();
        }
        else
        {
            _isClosing = true;
            var lastImage = _images[_currentStage];

            var seq = DOTween.Sequence();
            seq.SetUpdate(true);

            seq.Join(lastImage.DOFade(0f, _fadeDuration).SetEase(Ease.InOutQuad));
            seq.Join(lastImage.rectTransform.DOShakePosition(_fadeDuration, _shakePositionStrength, _shakeVibrato, 90f, false, true));
            seq.Join(lastImage.rectTransform.DOShakeRotation(_fadeDuration, new Vector3(0, 0, _shakeRotationStrength), _shakeVibrato, 90f));
            seq.Join(lastImage.rectTransform.DOShakeScale(_fadeDuration, _shakeScaleStrength, _shakeVibrato, 90f));

            // === ВЫЗЫВАЕМ СОБЫТИЕ ПЕРЕД ЗАКРЫТИЕМ ===
            seq.OnComplete(() =>
            {
                OnFloatingFinished?.Invoke(); // Уведомляем Startup
                Close();
            });
            seq.Play();
        }
    }

    public void ResetPopup()
    {
        _isClosing = false;
        _isTransitioning = false;
        _currentStage = 0;
        _currentAutoTimer = _autoTransitionDelay;

        if (_images != null && _startPositions != null)
        {
            for (int i = 0; i < _images.Count; i++)
            {
                var img = _images[i];
                if (img == null) continue;

                img.rectTransform.DOKill();
                img.DOKill();

                if (i < _startPositions.Length)
                {
                    img.rectTransform.anchoredPosition = _startPositions[i];
                }

                img.rectTransform.localRotation = Quaternion.identity;
                img.rectTransform.localScale = Vector3.one;

                var color = img.color;
                color.a = (i == 0) ? 1f : 0f;
                img.color = color;
            }
        }
    }
}
