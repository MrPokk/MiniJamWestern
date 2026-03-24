using UnityEngine;

public class BalatroTiltEffect : MonoBehaviour
{
    [Header("Режим работы")]
    [SerializeField] private bool _tiltTowardsPointer = true; // ГАЛОЧКА: true — к указателю, false — от него

    [Header("Настройки наклона")]
    [SerializeField] private float _maxTiltAngle = 20f;
    [SerializeField] private float _smoothSpeed = 10f;

    [Header("Чувствительность")]
    [SerializeField] private float _tiltSensitivity = 1.5f;

    private Quaternion _targetRotation = Quaternion.identity;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        ApplyBalatroTilt();
    }

    private void ApplyBalatroTilt()
    {
        Vector2 pointerPos = ControllableSystem.PointerPosition;
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(transform.position);

        // Вычисляем относительное смещение (-1 до 1)
        var distX = (pointerPos.x - screenPos.x) / Screen.width;
        var distY = (pointerPos.y - screenPos.y) / Screen.height;

        // Определяем направление наклона (инверсия)
        // Если _tiltTowardsPointer = true, то инвертируем базовую логику
        var directionMultiplier = _tiltTowardsPointer ? -1f : 1f;

        // Базовая математика:
        // Поворот вокруг Y зависит от смещения по X
        // Поворот вокруг X зависит от смещения по Y
        var tiltY = distX * _maxTiltAngle * _tiltSensitivity * directionMultiplier;
        var tiltX = -distY * _maxTiltAngle * _tiltSensitivity * directionMultiplier;

        // Ограничиваем углы
        tiltX = Mathf.Clamp(tiltX, -_maxTiltAngle, _maxTiltAngle);
        tiltY = Mathf.Clamp(tiltY, -_maxTiltAngle, _maxTiltAngle);

        // Создаем целевой поворот
        _targetRotation = Quaternion.Euler(tiltX, tiltY, 0);

        // Плавно применяем
        transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime * _smoothSpeed);
    }
}
