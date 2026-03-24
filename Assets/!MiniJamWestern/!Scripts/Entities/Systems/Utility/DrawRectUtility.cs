using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class DrawRectUtility : MonoBehaviour
{
    public static DrawRectUtility Instance { get; private set; }

    private LineRenderer _lineRend;
    // ИСПРАВЛЕНИЕ: Используем Vector3, чтобы не терять глубину (Z) при клике в 3D пространстве
    private Vector3 _initialMousePosition;
    private bool _isDrawing;

    [Header("Resolution Settings")]
    [SerializeField] private float _referenceScreenHeight = 1080f;
    [SerializeField] private int _orderLayer = 0;

    [Header("Visual Settings")]
    [SerializeField] private float _lineWidthPixels = 1.0f;
    [SerializeField] private Color _lineColor = Color.white;
    [SerializeField] private Sprite _pixelSprite;

    private Dictionary<int, LineRenderer> _activeRects = new Dictionary<int, LineRenderer>();
    private Queue<LineRenderer> _rectPool = new Queue<LineRenderer>();

    private Dictionary<int, SpriteRenderer> _activeFills = new Dictionary<int, SpriteRenderer>();
    private Queue<SpriteRenderer> _fillPool = new Queue<SpriteRenderer>();

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

        // ИСПРАВЛЕНИЕ: Интерактивная рамка всегда рисуется поверх всего (большой offset)
        _lineRend.sortingOrder = _orderLayer + 2000;

        if (_pixelSprite == null)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            _pixelSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        }
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

    public void DrawStaticRect(int id, Vector3 center, float sizeInPixels, Color color)
    {
        var lr = GetOrCreateLineRenderer(id);
        SetLineRendererPoints(lr, id, center, sizeInPixels, color); // Передали ID
    }

    public void HideStaticRect(int id)
    {
        if (_activeRects.TryGetValue(id, out var lr))
        {
            lr.positionCount = 0;
            lr.gameObject.SetActive(false);
            _activeRects.Remove(id);
            _rectPool.Enqueue(lr);
        }
    }

    public void DrawStaticFullRect(int id, Vector3 center, float sizeInPixels, Color color)
    {
        var sr = GetOrCreateSpriteRenderer(id);

        var worldHeight = GetWorldHeightAtPosition(center);
        var logicalScale = worldHeight / _referenceScreenHeight;
        var worldSize = sizeInPixels * logicalScale;

        // ИСПРАВЛЕНИЕ Z-FIGHTING: 
        // Создаем уникальное микро-смещение по Z на основе ID, чтобы они физически не сливались.
        float uniqueZOffset = 0.01f - ((Mathf.Abs(id) % 1000) * 0.0001f);

        sr.transform.position = new Vector3(center.x, center.y, center.z + uniqueZOffset);
        sr.transform.localScale = new Vector3(worldSize, worldSize, 1f);
        sr.color = color;
    }

    public void HideStaticFullRect(int id)
    {
        if (_activeFills.TryGetValue(id, out var sr))
        {
            sr.gameObject.SetActive(false);
            _activeFills.Remove(id);
            _fillPool.Enqueue(sr);
        }
    }

    private LineRenderer GetOrCreateLineRenderer(int id)
    {
        if (_activeRects.TryGetValue(id, out var lr)) return lr;

        if (_rectPool.Count > 0)
        {
            lr = _rectPool.Dequeue();
        }
        else
        {
            var go = new GameObject($"Outline_{id}");
            go.transform.SetParent(transform);
            lr = go.AddComponent<LineRenderer>();
            lr.loop = true;
            lr.useWorldSpace = true;
            lr.material = _lineRend.sharedMaterial;
        }

        // ИСПРАВЛЕНИЕ Z-FIGHTING: Разные ID получают разный sortingOrder. 
        // +1000 гарантирует, что контуры всегда поверх заливки (Fill).
        lr.sortingOrder = _orderLayer + 1000 + (Mathf.Abs(id) % 1000);

        lr.gameObject.SetActive(true);
        _activeRects[id] = lr;
        return lr;
    }

    private SpriteRenderer GetOrCreateSpriteRenderer(int id)
    {
        if (_activeFills.TryGetValue(id, out var sr)) return sr;

        if (_fillPool.Count > 0)
        {
            sr = _fillPool.Dequeue();
        }
        else
        {
            var go = new GameObject($"Fill_{id}");
            go.transform.SetParent(transform);
            sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _pixelSprite;
        }

        // ИСПРАВЛЕНИЕ Z-FIGHTING: Каждый уникальный прямоугольник имеет свой приоритет отрисовки
        sr.sortingOrder = _orderLayer + (Mathf.Abs(id) % 1000);

        sr.gameObject.SetActive(true);
        _activeFills[id] = sr;
        return sr;
    }

    private float GetWorldHeightAtPosition(Vector3 worldPosition)
    {
        var cam = Camera.main;
        if (cam == null) return 1f;
        if (cam.orthographic) return cam.orthographicSize * 2.0f;
        var distance = Vector3.Distance(cam.transform.position, worldPosition);
        return 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }

    // ИСПРАВЛЕНИЕ: Добавлен параметр id
    private void SetLineRendererPoints(LineRenderer lr, int id, Vector3 center, float sizeInPixels, Color color)
    {
        var worldHeight = GetWorldHeightAtPosition(center);
        var logicalScale = worldHeight / _referenceScreenHeight;
        var minPhysicalScale = worldHeight / Screen.height;
        var worldLineWidth = Mathf.Max(_lineWidthPixels * logicalScale, minPhysicalScale);
        var worldSize = sizeInPixels * logicalScale;
        var half = worldSize * 0.5f;

        // Микро-смещение по Z для обводок, чтобы они не конфликтовали друг с другом
        float uniqueZOffset = -((Mathf.Abs(id) % 1000) * 0.0001f);
        float zPos = center.z + uniqueZOffset;

        lr.startWidth = lr.endWidth = worldLineWidth;
        lr.startColor = lr.endColor = color;
        lr.positionCount = 4;

        lr.SetPosition(0, new Vector3(center.x - half, center.y + half, zPos));
        lr.SetPosition(1, new Vector3(center.x - half, center.y - half, zPos));
        lr.SetPosition(2, new Vector3(center.x + half, center.y - half, zPos));
        lr.SetPosition(3, new Vector3(center.x + half, center.y + half, zPos));
    }

    private void OnDrawStarted(InputAction.CallbackContext context)
    {
        _isDrawing = true;
        _initialMousePosition = ControllableSystem.GetPointerPositionWorld();
    }

    private void OnDrawEnded(InputAction.CallbackContext context)
    {
        _isDrawing = false;
        if (_lineRend != null) _lineRend.positionCount = 0;
    }

    private void Update()
    {
        if (_isDrawing)
        {
            Vector3 currentPos = ControllableSystem.GetPointerPositionWorld();
            Vector3 center = (_initialMousePosition + currentPos) * 0.5f;

            var worldHeight = GetWorldHeightAtPosition(center);
            var logicalScale = worldHeight / _referenceScreenHeight;
            var minPhysicalScale = worldHeight / Screen.height;
            var worldLineWidth = Mathf.Max(_lineWidthPixels * logicalScale, minPhysicalScale);

            _lineRend.startWidth = worldLineWidth;
            _lineRend.endWidth = worldLineWidth;
            _lineRend.startColor = _lineColor;
            _lineRend.endColor = _lineColor;

            _lineRend.positionCount = 4;

            // ИСПРАВЛЕНИЕ: Используем Vector3, чтобы сохранять Z позицию мыши
            _lineRend.SetPosition(0, new Vector3(_initialMousePosition.x, _initialMousePosition.y, _initialMousePosition.z));
            _lineRend.SetPosition(1, new Vector3(_initialMousePosition.x, currentPos.y, _initialMousePosition.z));
            _lineRend.SetPosition(2, new Vector3(currentPos.x, currentPos.y, currentPos.z));
            _lineRend.SetPosition(3, new Vector3(currentPos.x, _initialMousePosition.y, currentPos.z));
        }
    }
}
