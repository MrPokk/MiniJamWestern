using System;
using BitterECS.Integration.Unity;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIHealthElementProvider : ProviderEcs<UIHealthElementComponent>, IDisplay
{
    private Image _image;
    protected override void Registration() => _image = GetComponent<Image>();

    public void SetActive(bool active) => gameObject.SetActive(active);
    public void SetSprite(Sprite sprite) => _image.sprite = sprite;
}

[Serializable] public struct UIHealthElementComponent { }
