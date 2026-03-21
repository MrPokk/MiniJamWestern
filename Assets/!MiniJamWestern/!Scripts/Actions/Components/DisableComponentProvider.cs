using UnityEngine;

public class DisableComponentProvider : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
}
