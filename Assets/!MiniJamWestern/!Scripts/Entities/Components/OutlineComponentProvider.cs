using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct OutlineComponent
{
    public SpriteRenderer spriteRenderer;
    public Color defaultOutlineColor;

    private static readonly int s_outlineColorProperty = Shader.PropertyToID("_OutlineColor");
    private static readonly int s_outlinePixelsProperty = Shader.PropertyToID("_OutlinePixels");
    private static readonly int s_alphaThresholdProperty = Shader.PropertyToID("_AlphaThreshold");

    public readonly void SetOutlineColor(Color color)
    {
        if (spriteRenderer == null) return;
        var block = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(block);
        block.SetColor(s_outlineColorProperty, color);
        spriteRenderer.SetPropertyBlock(block);
    }

    public readonly void SetOutlineWidth(float pixels)
    {
        if (spriteRenderer == null) return;
        var block = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(block);
        block.SetFloat(s_outlinePixelsProperty, pixels);
        spriteRenderer.SetPropertyBlock(block);
    }

    public readonly void SetAlphaThreshold(float threshold)
    {
        if (spriteRenderer == null) return;
        var block = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(block);
        block.SetFloat(s_alphaThresholdProperty, threshold);
        spriteRenderer.SetPropertyBlock(block);
    }

    public readonly Color GetOutlineColor()
    {
        if (spriteRenderer == null || spriteRenderer.sharedMaterial == null)
            return Color.white;
        return spriteRenderer.sharedMaterial.GetColor(s_outlineColorProperty);
    }
}

public class OutlineComponentProvider : ProviderEcs<OutlineComponent>
{
    protected override void Registration()
    {
        Value.spriteRenderer = Value.spriteRenderer != null ? Value.spriteRenderer : GetComponentInChildren<SpriteRenderer>();
        if (Value.spriteRenderer != null && Value.spriteRenderer.sharedMaterial != null)
        {
            Value.defaultOutlineColor = Value.spriteRenderer.sharedMaterial.GetColor("_OutlineColor");
        }
    }
}
