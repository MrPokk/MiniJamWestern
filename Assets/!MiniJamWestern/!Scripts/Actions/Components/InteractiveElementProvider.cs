using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class InteractiveElementProvider : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
