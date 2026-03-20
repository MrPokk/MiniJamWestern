using UnityEngine;

public class WateryCardController : MonoBehaviour
{
    private Material _mat;
    private Camera _cam;

    [Header("Physical Settings")]
    public float smoothSpeed = 8f;
    public float tiltSensitivity = 2f;
    public float maxTilt = 0.4f;

    private Vector2 _currentTilt;

    void Start()
    {
        _cam = Camera.main;
        if (TryGetComponent<Renderer>(out var r)) _mat = r.material;
    }

    void Update()
    {
        Vector2 pointerPos = ControllableSystem.PointerPosition;
        Vector3 cardScreenPos = _cam.WorldToScreenPoint(transform.position);

        // Вычисляем отклонение
        Vector2 targetTilt = new Vector2(
            (pointerPos.y - cardScreenPos.y) / Screen.height,
            (pointerPos.x - cardScreenPos.x) / Screen.width
        );

        targetTilt *= tiltSensitivity;

        // Ограничиваем наклон
        targetTilt.x = Mathf.Clamp(targetTilt.x, -maxTilt, maxTilt);
        targetTilt.y = Mathf.Clamp(targetTilt.y, -maxTilt, maxTilt);

        // Плавный переход (Lerp создает эффект "вязкости" воды)
        _currentTilt = Vector2.Lerp(_currentTilt, targetTilt, Time.deltaTime * smoothSpeed);

        if (_mat != null)
        {
            _mat.SetVector("_Tilt", _currentTilt);
        }
    }
}
