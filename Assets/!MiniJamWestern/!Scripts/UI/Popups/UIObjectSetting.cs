using UINotDependence.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(RectTransform))]
public class UIObjectSetting : UIPopup, IBeginDragHandler, IDragHandler
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    private const string MusicParam = "MusicVolume";
    private const string SfxParam = "SfxVolume";

    private RectTransform _rectTransform;
    private Canvas _canvas;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        if (musicSlider != null)
        {
            musicSlider.minValue = 0.0001f;
            musicSlider.maxValue = 1f;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0.0001f;
            sfxSlider.maxValue = 1f;
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        }
    }

    private void Start()
    {
        if (audioMixer != null)
        {
            if (musicSlider != null && audioMixer.GetFloat(MusicParam, out var musicDb))
            {
                var linearValue = Mathf.Pow(10, musicDb / 20f);
                musicSlider.SetValueWithoutNotify(linearValue);
            }

            if (sfxSlider != null && audioMixer.GetFloat(SfxParam, out var sfxDb))
            {
                var linearValue = Mathf.Pow(10, sfxDb / 20f);
                sfxSlider.SetValueWithoutNotify(linearValue);
            }
        }
    }

    public override void Open()
    {
        UIAnimationComponent
          .Using(gameObject)
          .SetPresets(UIAnimationPresets.FadeIn,
                      UIAnimationPresets.FadeOut)
          .PlayOpen();
    }

    public override void Close()
    {
        UIAnimationComponent
          .Using(gameObject)
          .SetPresets(UIAnimationPresets.FadeIn,
                      UIAnimationPresets.FadeOut)
          .PlayClose();
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData)
    {
        if (_rectTransform == null) return;

        var scaleFactor = _canvas != null ? _canvas.scaleFactor : 1f;
        _rectTransform.anchoredPosition += eventData.delta / scaleFactor;
    }

    private void SetMusicVolume(float value)
    {
        if (audioMixer == null) return;

        var volumeInDb = Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(MusicParam, volumeInDb);
    }

    private void SetSfxVolume(float value)
    {
        if (audioMixer == null) return;

        var volumeInDb = Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(SfxParam, volumeInDb);
    }

    private void OnDestroy()
    {
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);
    }
}
