using UnityEngine;

public class BalatroCardController : MonoBehaviour
{
    private Material _mat;
    private Camera _cam;

    [Header("Настройки")]
    public float smoothSpeed = 10f;
    public float maxTilt = 0.5f;

    private Vector2 _currentTilt;

    void Start()
    {
        _cam = Camera.main;
        var renderer = GetComponent<Renderer>();
        if (renderer != null) _mat = renderer.material;
    }

    void Update()
    {
        // 1. Получаем позицию мыши относительно центра карты
        Vector3 mousePos = ControllableSystem.PointerPosition;
        Vector3 cardScreenPos = _cam.WorldToScreenPoint(transform.position);

        Vector2 targetTilt = new Vector2(
            (mousePos.y - cardScreenPos.y) / Screen.height,
            (mousePos.x - cardScreenPos.x) / Screen.width
        );

        // 2. Ограничиваем наклон
        targetTilt.x = Mathf.Clamp(targetTilt.x * 2f, -maxTilt, maxTilt);
        targetTilt.y = Mathf.Clamp(targetTilt.y * 2f, -maxTilt, maxTilt);

        // 3. Плавность
        _currentTilt = Vector2.Lerp(_currentTilt, targetTilt, Time.deltaTime * smoothSpeed);

        // 4. Передаем в шейдер
        if (_mat != null)
        {
            _mat.SetVector("_Tilt", _currentTilt);
        }
    }
}
