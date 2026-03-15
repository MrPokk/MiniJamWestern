using UnityEngine;

public class CameraObject : MonoBehaviour
{
    public Camera CameraTarget { get; private set; }

    private void Awake()
    {
        CameraTarget = GetComponentInChildren<Camera>();
    }
}
