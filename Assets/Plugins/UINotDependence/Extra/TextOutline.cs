using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class TextOutline : MonoBehaviour
{
    [SerializeField] private TMP_Text _mainText;
    [SerializeField] private List<TMP_Text> _textOutlines = new();

    [Header("Settings")]
    [SerializeField] private Color _outlineColor = Color.black;

    private readonly Dictionary<TMP_Text, Vector3> _initialOffsets = new();
    private string _lastText;
    private Color _lastMainColor;
    private float _lastFontSize;
    private bool _isSyncing;

    [Header("Editor")]
    [SerializeField] private bool _isEdit;

    private void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
        RefreshOutlineTransforms();
        SyncTextAndStyle();
    }

    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
    }

    private void OnValidate()
    {
        if (_isEdit) return;
        RefreshOutlineTransforms();
        SyncTextAndStyle();
    }

    private void LateUpdate()
    {
        if (_isEdit || _mainText == null) return;

        if (_mainText.text != _lastText ||
            !Mathf.Approximately(_mainText.fontSize, _lastFontSize) ||
            _mainText.color != _lastMainColor)
        {
            SyncTextAndStyle();
        }

        UpdateOutlineTransforms();
    }

    private void OnTextChanged(Object obj)
    {
        if (_isSyncing) return;
        if (_mainText != null && obj == (Object)_mainText)
        {
            SyncTextAndStyle();
        }
    }

    public void SyncTextAndStyle()
    {
        if (_mainText == null || _isSyncing) return;

        _isSyncing = true;

        try
        {
            _mainText.ForceMeshUpdate();

            _lastText = _mainText.text;
            _lastMainColor = _mainText.color;
            _lastFontSize = _mainText.fontSize;

            var font = _mainText.font;
            var align = _mainText.alignment;
            var style = _mainText.fontStyle;
            var overflow = _mainText.overflowMode;
            var margin = _mainText.margin;
            float alpha = _mainText.alpha;

            var mainRT = _mainText.rectTransform;

            foreach (var outline in _textOutlines)
            {
                if (outline == null || outline == _mainText) continue;

                outline.enableAutoSizing = false;
                outline.fontSize = _lastFontSize;

                if (outline.text != _lastText) outline.text = _lastText;

                Color targetColor = _outlineColor;
                targetColor.a *= alpha;
                if (outline.color != targetColor) outline.color = targetColor;

                if (outline.font != font) outline.font = font;
                if (outline.alignment != align) outline.alignment = align;
                if (outline.fontStyle != style) outline.fontStyle = style;
                if (outline.overflowMode != overflow) outline.overflowMode = overflow;
                if (outline.margin != margin) outline.margin = margin;

                var outlineRT = outline.rectTransform;
                outlineRT.anchorMin = mainRT.anchorMin;
                outlineRT.anchorMax = mainRT.anchorMax;
                outlineRT.pivot = mainRT.pivot;
                outlineRT.sizeDelta = mainRT.sizeDelta;

                outline.ForceMeshUpdate();
            }
        }
        finally
        {
            _isSyncing = false;
        }
    }

    private void UpdateOutlineTransforms()
    {
        if (_mainText == null) return;

        Vector3 mainPos = _mainText.transform.localPosition;
        Vector3 mainScale = _mainText.transform.localScale;

        foreach (var outline in _textOutlines)
        {
            if (outline == null || !_initialOffsets.TryGetValue(outline, out var offset)) continue;

            outline.transform.localPosition = mainPos + offset;
            outline.transform.localScale = mainScale;
        }
    }

    public void RefreshOutlineTransforms()
    {
        _textOutlines.RemoveAll(x => x == null);
        _initialOffsets.Clear();

        if (_mainText == null) return;

        Vector3 mainPos = _mainText.transform.localPosition;

        foreach (var outline in _textOutlines)
        {
            if (outline == null || outline == _mainText) continue;
            _initialOffsets[outline] = outline.transform.localPosition - mainPos;
        }
    }

    public void SetText(string text)
    {
        if (_mainText) _mainText.text = text;
        SyncTextAndStyle();
    }
}
