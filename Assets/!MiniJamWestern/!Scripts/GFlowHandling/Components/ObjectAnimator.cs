using UnityEngine;

public class ObjectAnimator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private bool rotationEnabled = false;
    [SerializeField] private Vector3 rotationRange = new Vector3(0, 0, 15f);
    [SerializeField] private float rotationSpeed = 1f;

    [Header("Scale Settings")]
    [SerializeField] private bool scaleEnabled = false;
    [SerializeField] private Vector3 minScale = new Vector3(0.8f, 0.8f, 1f);
    [SerializeField] private Vector3 maxScale = new Vector3(1.2f, 1.2f, 1f);
    [SerializeField] private float scaleSpeed = 1f;

    private Vector3 initialRotation;

    private void Start()
    {
        initialRotation = transform.localEulerAngles;
    }

    private void Update()
    {
        UpdateRotation();
        UpdateScale();
    }

    private void UpdateRotation()
    {
        if (!rotationEnabled) return;
        float t = Mathf.Sin(Time.time * rotationSpeed);
        transform.localEulerAngles = initialRotation + (rotationRange * t);
    }

    private void UpdateScale()
    {
        if (!scaleEnabled) return;
        float t = (Mathf.Sin(Time.time * scaleSpeed) + 1f) / 2f;
        transform.localScale = Vector3.Lerp(minScale, maxScale, t);
    }
}
