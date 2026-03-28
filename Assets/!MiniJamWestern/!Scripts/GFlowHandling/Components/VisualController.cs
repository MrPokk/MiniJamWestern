using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Visual
{
    public SpriteRenderer renderer;
    public Material altMaterial;
    [HideInInspector] public Material originalMaterial;
}

public class VisualController : MonoBehaviour
{
    public List<Visual> visuals;

    private void Awake()
    {
        foreach (var v in visuals)
        {
            if (v.renderer != null)
            {
                v.originalMaterial = v.renderer.sharedMaterial;
            }
        }
    }

    public void UpdateMaterialsByEnum<T>(T value) where T : Enum
    {
        var isEven = value.IsEveryThird();

        foreach (var v in visuals)
        {
            if (v.renderer == null) continue;

            if (isEven)
            {
                if (v.altMaterial != null)
                    v.renderer.material = v.altMaterial;
            }
            else
            {
                v.renderer.material = v.originalMaterial;
            }
        }
    }
}
