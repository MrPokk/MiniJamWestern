using EasyTextEffects;
using TMPro;
using UINotDependence.Core;
using UnityEngine;

public class UITooltipPopup : UIPopup
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _abilityText;

    [Header("Positioning")]
    [SerializeField] private Vector2 _offset = new(10f, -10f);

    private Canvas _rootCanvas;

    private void Awake()
    {
        _rootCanvas = GetComponentInParent<Canvas>();
    }

    public void Bind(SoldInfoComponent soldInfoComponent, int dynamicValue)
    {
        InitNameText(soldInfoComponent);
        InitDescriptionText(soldInfoComponent, dynamicValue);
        InitAbilityText(soldInfoComponent);
    }

    private void InitNameText(SoldInfoComponent soldInfoComponent)
    {
        if (_nameText)
        {
            var title = soldInfoComponent.title;
            title = title.Replace("\r\n", " ").Replace("\n", " ");
            _nameText.text = title;
            RefreshTextEffect(_nameText);
        }
    }

    private void InitDescriptionText(SoldInfoComponent soldInfoComponent, int dynamicValue)
    {
        if (_descriptionText)
        {
            var finalDescription = AbilityUIConverter.GetFinalText(soldInfoComponent, dynamicValue);
            _descriptionText.text = finalDescription;
            RefreshTextEffect(_descriptionText);
        }
    }

    private void InitAbilityText(SoldInfoComponent soldInfoComponent)
    {
        if (_abilityText)
        {
            var amountText = soldInfoComponent.amount > 0 ? $"Amount: {soldInfoComponent.amount}" : "";
            _abilityText.text = amountText;
            _abilityText.gameObject.SetActive(!string.IsNullOrEmpty(amountText));
            if (_abilityText.gameObject.activeSelf)
                RefreshTextEffect(_abilityText);
        }
    }

    private void RefreshTextEffect(TMP_Text textComponent)
    {
        if (textComponent == null) return;
        if (textComponent.TryGetComponent<TextEffect>(out var textEffect)) textEffect.Refresh();
    }

    public override void Open()
    {
        if (_rootCanvas != null)
        {
            Vector3 pointerPos = ControllableSystem.PointerPosition;
            var scaleFactor = _rootCanvas.scaleFactor;
            var scaledOffset = (Vector3)_offset * scaleFactor;
            transform.position = pointerPos + scaledOffset;
        }

        UIAnimationComponent
            .Using(gameObject)
            .SetPresets(UIAnimationPresets.PopupOpen,
                        UIAnimationPresets.PopupClose)
            .PlayOpen();

        base.Open();
    }

    private void Update()
    {
        if (!gameObject || _rootCanvas == null)
            return;

        Vector3 pointerPos = ControllableSystem.PointerPosition;

        var scaleFactor = _rootCanvas.scaleFactor;
        var scaledOffset = (Vector3)_offset * scaleFactor;

        transform.position = pointerPos + scaledOffset;
    }

    public override void Close()
    {
        base.Close();
    }
}
