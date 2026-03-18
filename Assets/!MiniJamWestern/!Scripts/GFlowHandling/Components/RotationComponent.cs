using UnityEngine;

public class RotationComponent : MonoBehaviour
{
    public bool isRandomRotation = true;
    public bool isLookAtCamera = true;

    [Header("X Axis Range")]
    public float minRandomX = 0f;
    public float maxRandomX = 0f;

    [Header("Y Axis Range")]
    public float minRandomY = -60f;
    public float maxRandomY = 60f;

    [Header("Z Axis Range")]
    public float minRandomZ = 0f;
    public float maxRandomZ = 0f;

    private void Start()
    {
        ApplyRotation();
    }

    private void ApplyRotation()
    {
        if (Camera.main != null && isLookAtCamera)
        {
            transform.LookAt(Camera.main.transform.position);
        }

        if (!isRandomRotation)
        {
            return;
        }

        var randomX = Random.Range(minRandomX, maxRandomX);
        var randomY = Random.Range(minRandomY, maxRandomY);
        var randomZ = Random.Range(minRandomZ, maxRandomZ);

        transform.Rotate(randomX, randomY, randomZ);
    }
}
