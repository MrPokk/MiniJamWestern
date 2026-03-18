using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class DrawColliderGizmos : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (!TryGetComponent<BoxCollider2D>(out var boxCollider)) return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 size = boxCollider.size;
        Vector3 center = boxCollider.offset;

        var gizmoColor = Color.green;
        var fillColor = gizmoColor;
        fillColor.a = 0.2f;
        Gizmos.color = fillColor;
        Gizmos.DrawCube(center, size);

        var edgeColor = gizmoColor;
        edgeColor.a = 1.0f;
        Gizmos.color = edgeColor;
        Gizmos.DrawWireCube(center, size);
    }
}
