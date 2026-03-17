using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public struct AbilityComponent
{
    public EcsEntity owner;
}

[RequireComponent(typeof(BoxCollider2D))]
public class AbilityViewProvider : ProviderEcs<AbilityComponent>,
    IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Color _gizmoColor = Color.green;
    private Camera _camera;
    private Collider2D _collider;

    private void Start()
    {
        _camera = Camera.main;
        _collider = GetComponent<Collider2D>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Entity.AddFrame(new PointerDownAbilityEvent { pressTime = Time.time });
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Entity.AddFrame<PointerUpAbilityEvent>();
    }

    public Camera GetCamera() => _camera;

    public void EnableCollider(bool enable)
    {
        if (_collider == null) return;

        _collider.enabled = enable;
    }

    private void OnDrawGizmos()
    {
        if (!TryGetComponent<BoxCollider2D>(out var boxCollider)) return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 size = boxCollider.size;
        Vector3 center = boxCollider.offset;

        Color fillColor = _gizmoColor;
        fillColor.a = 0.2f;
        Gizmos.color = fillColor;
        Gizmos.DrawCube(center, size);

        Color edgeColor = _gizmoColor;
        edgeColor.a = 1.0f;
        Gizmos.color = edgeColor;
        Gizmos.DrawWireCube(center, size);
    }
}
