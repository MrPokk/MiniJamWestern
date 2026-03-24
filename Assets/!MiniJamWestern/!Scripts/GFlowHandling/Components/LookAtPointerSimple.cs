using UnityEngine;

public class LookAtPointerSimple : MonoBehaviour
{
    public float angleOffset = 0f;
    public bool smooth = false;
    public float speed = 10f;

    private void Update()
    {
        Vector3 mousePos = ControllableSystem.GetPointerPositionWorld();

        Vector2 direction = (Vector2)mousePos - (Vector2)transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRot = Quaternion.Euler(0, 0, angle + angleOffset);

        if (smooth)
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, speed * Time.deltaTime);
        else
            transform.rotation = targetRot;
    }
}
