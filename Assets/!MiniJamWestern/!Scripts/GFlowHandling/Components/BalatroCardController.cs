using UnityEngine;
using UnityEngine.UI;

public class BalatroCardController : MonoBehaviour
{
    private Material _mat;
    private Camera _cam;
    private RectTransform _rectTransform;
    private Canvas _canvas;

    [Header("Настройки")]
    public float smoothSpeed = 10f;
    public float maxTilt = 0.5f;

    private Vector2 _currentTilt;

    void Start()
    {
        _cam = Camera.main;
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        var renderer = GetComponent<Renderer>();
        var rendererImage = GetComponent<Image>();

        if (renderer != null)
        {
            _mat = renderer.material;
        }
        else if (rendererImage != null)
        {
            rendererImage.material = new Material(rendererImage.material);
            _mat = rendererImage.material;
        }
    }

    void Update()
    {
        Vector3 mousePos = ControllableSystem.PointerPosition;
        Vector2 cardScreenPos;

        if (_rectTransform != null)
        {
            Camera uiCam = null;
            if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                uiCam = _canvas.worldCamera != null ? _canvas.worldCamera : _cam;
            }

            Vector3 worldCenter = _rectTransform.TransformPoint(_rectTransform.rect.center);
            cardScreenPos = RectTransformUtility.WorldToScreenPoint(uiCam, worldCenter);
        }
        else
        {
            cardScreenPos = _cam.WorldToScreenPoint(transform.position);
        }

        Vector2 targetTilt = new Vector2(
            (mousePos.y - cardScreenPos.y) / Screen.height,
            (mousePos.x - cardScreenPos.x) / Screen.width
        );

        targetTilt.x = Mathf.Clamp(targetTilt.x * 2f, -maxTilt, maxTilt);
        targetTilt.y = Mathf.Clamp(targetTilt.y * 2f, -maxTilt, maxTilt);

        _currentTilt = Vector2.Lerp(_currentTilt, targetTilt, Time.deltaTime * smoothSpeed);

        if (_mat != null)
        {
            _mat.SetVector("_Tilt", _currentTilt);
        }
    }
}
