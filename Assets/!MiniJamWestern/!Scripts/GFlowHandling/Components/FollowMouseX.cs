using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouseX : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private bool useWorldPosition = true;
    [SerializeField] private bool invertDirection = false;

    [Header("Local X Limits")]
    [SerializeField] private float minLocalX = -0.05f;
    [SerializeField] private float maxLocalX = 0.05f;

    private Camera mainCamera;
    private float targetLocalX;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector3 mouseScreenPos = ControllableSystem.PointerPosition;

        float targetX;
        if (useWorldPosition)
        {
            mouseScreenPos.z = -mainCamera.transform.position.z;
            var mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            targetX = mouseWorldPos.x + offsetX;
        }
        else
        {
            targetX = mouseScreenPos.x + offsetX;
        }

        if (invertDirection)
        {
            float currentWorldX = transform.position.x;
            targetX = 2f * currentWorldX - targetX;
        }

        var localPos = transform.localPosition;
        targetLocalX = transform.parent != null
            ? transform.parent.InverseTransformPoint(new Vector3(targetX, 0, 0)).x
            : targetX;

        targetLocalX = Mathf.Clamp(targetLocalX, minLocalX, maxLocalX);

        localPos.x = Mathf.Lerp(localPos.x, targetLocalX, Time.deltaTime * speed);
        transform.localPosition = localPos;
    }
}
