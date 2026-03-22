using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class DrawRectUtility : MonoBehaviour
{
    public static DrawRectUtility Instance { get; private set; }

    // Главный LineRenderer для отрисовки мышью и старых систем
    private LineRenderer _lineRend;
    private Vector2 _initialMousePosition;
    private bool _isDrawing;

    [Header("Resolution Settings")]
    [Tooltip("Reference Screen Height (in pixels) for resolution calculations")]
    [SerializeField] private float _referenceScreenHeight = 1080f;
    [SerializeField] private int _orderLayer = 0; [Header("Visual Settings")]
    [SerializeField] private float _lineWidthPixels = 1.0f;
    [SerializeField] private Color _lineColor = Color.white;

    // ПУЛ ОБЪЕКТОВ ДЛЯ МНОЖЕСТВЕННЫХ РАМОК
    private Dictionary<int, LineRenderer> _activeRects = new Dictionary<int, LineRenderer>();
    private Queue<LineRenderer> _rectPool = new Queue<LineRenderer>();

    public float LineWidth
    {
        get => _lineWidthPixels;
        set => _lineWidthPixels = value;
    }

    public Color LineColor
    {
        get => _lineColor;
        set => _lineColor = value;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _lineRend = GetComponent<LineRenderer>();
        _lineRend.positionCount = 0;
        _lineRend.loop = true;
        _lineRend.useWorldSpace = true;
        _lineRend.sortingOrder = _orderLayer;
    }

    private void OnEnable()
    {
        ControllableSystem.SubscribeStarted(ControllableSystem.Inputs.UI.Click, OnDrawStarted);
        ControllableSystem.SubscribeCanceled(ControllableSystem.Inputs.UI.Click, OnDrawEnded);
    }

    private void OnDisable()
    {
        if (ControllableSystem.Inputs != null)
        {
            ControllableSystem.UnsubscribeStarted(ControllableSystem.Inputs.UI.Click, OnDrawStarted);
            ControllableSystem.UnsubscribeCanceled(ControllableSystem.Inputs.UI.Click, OnDrawEnded);
        }
    }

    private float GetWorldHeightAtPosition(Vector3 worldPosition)
    {
        var cam = Camera.main;
        if (cam == null) return 1f;

        if (cam.orthographic)
        {
            return cam.orthographicSize * 2.0f;
        }

        var distance = Vector3.Distance(cam.transform.position, worldPosition);
        return 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }

    // ==========================================
    // СТАРЫЕ МЕТОДЫ (Используют базовый LineRenderer)
    // ==========================================
    public void DrawStaticRect(Vector3 center, float sizeInPixels, Color color)
    {
        DrawStaticRect(center, sizeInPixels);
        _lineRend.startColor = color;
        _lineRend.endColor = color;
    }

    public void DrawStaticRect(Vector3 center, float sizeInPixels)
    {
        SetLineRendererPoints(_lineRend, center, sizeInPixels, _lineColor);
    }

    public void HideRect()
    {
        if (_lineRend != null) _lineRend.positionCount = 0;
    }

    // ==========================================
    // НОВЫЕ МЕТОДЫ ДЛЯ МНОЖЕСТВЕННЫХ РАМОК (Пул)
    // ==========================================

    // Получает свободный LineRenderer из пула или создает новый
    private LineRenderer GetOrCreateLineRenderer(int id)
    {
        if (_activeRects.TryGetValue(id, out var lr)) return lr;

        if (_rectPool.Count > 0)
        {
            lr = _rectPool.Dequeue();
        }
        else
        {
            var go = new GameObject($"DrawRect_{id}");
            go.transform.SetParent(transform); // Делаем дочерним для порядка в иерархии
            lr = go.AddComponent<LineRenderer>();
            lr.loop = true;
            lr.useWorldSpace = true;
            lr.sortingOrder = _orderLayer;
            // Копируем материал из главного LineRenderer
            lr.material = _lineRend.sharedMaterial != null ? _lineRend.sharedMaterial : new Material(Shader.Find("Sprites/Default"));
        }

        lr.gameObject.SetActive(true);
        _activeRects[id] = lr;
        return lr;
    }

    public void DrawStaticRect(int id, Vector3 center, float sizeInPixels, Color color)
    {
        var lr = GetOrCreateLineRenderer(id);
        SetLineRendererPoints(lr, center, sizeInPixels, color);
    }

    public void HideStaticRect(int id)
    {
        if (_activeRects.TryGetValue(id, out var lr))
        {
            lr.positionCount = 0;
            lr.gameObject.SetActive(false);
            _activeRects.Remove(id);
            _rectPool.Enqueue(lr); // Возвращаем в пул
        }
    }

    // Общая математика для установки точек
    private void SetLineRendererPoints(LineRenderer lr, Vector3 center, float sizeInPixels, Color color)
    {
        var worldHeight = GetWorldHeightAtPosition(center);
        var logicalScale = worldHeight / _referenceScreenHeight;
        var minPhysicalScale = worldHeight / Screen.height;
        var worldLineWidth = Mathf.Max(_lineWidthPixels * logicalScale, minPhysicalScale);

        var worldSize = sizeInPixels * logicalScale;
        var half = worldSize * 0.5f;

        lr.startWidth = worldLineWidth;
        lr.endWidth = worldLineWidth;
        lr.startColor = color;
        lr.endColor = color;

        lr.positionCount = 4;
        lr.SetPosition(0, new Vector3(center.x - half, center.y + half, center.z));
        lr.SetPosition(1, new Vector3(center.x - half, center.y - half, center.z));
        lr.SetPosition(2, new Vector3(center.x + half, center.y - half, center.z));
        lr.SetPosition(3, new Vector3(center.x + half, center.y + half, center.z));
    }

    // ==========================================
    // ЛОГИКА ОТРИСОВКИ МЫШЬЮ
    // ==========================================
    private void OnDrawStarted(InputAction.CallbackContext context)
    {
        _isDrawing = true;
        _initialMousePosition = ControllableSystem.GetPointerPositionWorld();
    }

    private void OnDrawEnded(InputAction.CallbackContext context)
    {
        _isDrawing = false;
        HideRect();
    }

    private void Update()
    {
        if (_isDrawing)
        {
            Vector3 currentPos = ControllableSystem.GetPointerPositionWorld();
            Vector3 center = ((Vector3)_initialMousePosition + currentPos) * 0.5f;

            var worldHeight = GetWorldHeightAtPosition(center);
            var logicalScale = worldHeight / _referenceScreenHeight;
            var minPhysicalScale = worldHeight / Screen.height;
            var worldLineWidth = Mathf.Max(_lineWidthPixels * logicalScale, minPhysicalScale);

            _lineRend.startWidth = worldLineWidth;
            _lineRend.endWidth = worldLineWidth;
            _lineRend.startColor = _lineColor;
            _lineRend.endColor = _lineColor;

            _lineRend.positionCount = 4;
            _lineRend.SetPosition(0, new Vector2(_initialMousePosition.x, _initialMousePosition.y));
            _lineRend.SetPosition(1, new Vector2(_initialMousePosition.x, currentPos.y));
            _lineRend.SetPosition(2, new Vector2(currentPos.x, currentPos.y));
            _lineRend.SetPosition(3, new Vector2(currentPos.x, _initialMousePosition.y));
        }
    }
}
