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

        // ДОБАВЛЕНО: Проверка и удержание в границах экрана
        KeepInBounds();
    }

    // --- МЕТОД ОГРАНИЧЕНИЯ ПЕРЕМЕЩЕНИЯ ---
    private void KeepInBounds()
    {
        if (_rectTransform == null || _canvas == null) return;

        var canvasRect = _canvas.GetComponent<RectTransform>();
        if (canvasRect == null) return;

        // Получаем мировые координаты 4 углов Канваса (экрана)
        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);

        // Получаем мировые координаты 4 углов самого Окна
        Vector3[] panelCorners = new Vector3[4];
        _rectTransform.GetWorldCorners(panelCorners);

        Vector3 offset = Vector3.zero;

        // Сравниваем левую [0] и правую [2] границы
        if (panelCorners[0].x < canvasCorners[0].x)
            offset.x = canvasCorners[0].x - panelCorners[0].x; // Вылезли слева -> толкаем вправо
        else if (panelCorners[2].x > canvasCorners[2].x)
            offset.x = canvasCorners[2].x - panelCorners[2].x; // Вылезли справа -> толкаем влево

        // Сравниваем нижнюю[0] и верхнюю [2] границы
        if (panelCorners[0].y < canvasCorners[0].y)
            offset.y = canvasCorners[0].y - panelCorners[0].y; // Вылезли снизу -> толкаем вверх
        else if (panelCorners[2].y > canvasCorners[2].y)
            offset.y = canvasCorners[2].y - panelCorners[2].y; // Вылезли сверху -> толкаем вниз

        // Если окно вышло за пределы экрана, применяем компенсацию
        if (offset != Vector3.zero)
        {
            _rectTransform.position += offset;
        }
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
