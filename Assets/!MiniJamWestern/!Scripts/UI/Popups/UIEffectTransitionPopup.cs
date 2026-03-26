using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UINotDependence.Core;
using BitterECS.Integration.Unity;

public class UIEffectTransitionPopup : UIPopup
{
    [SerializeField] private Image transitionImage;
    [SerializeField] private float transitionDuration = 0.5f;
    public float TransitionDuration => transitionDuration;

    private Material _transitionMaterial;
    private CoroutineHandle _transitionHandle;

    private readonly int _progressID = Shader.PropertyToID("_Progress");
    private readonly int _bgThresholdID = Shader.PropertyToID("_BackgroundThreshold");
    private readonly int _colorThresholdID = Shader.PropertyToID("_ColorThreshold");
    private readonly int _seedID = Shader.PropertyToID("_Seed");

    private void Awake()
    {
        if (transitionImage != null)
        {
            _transitionMaterial = new Material(transitionImage.material);
            transitionImage.material = _transitionMaterial;
        }
    }

    public override void Open()
    {
        base.Open();

        if (_transitionHandle.IsValid) CoroutineUtility.Stop(_transitionHandle);

        if (_transitionMaterial != null)
        {
            _transitionMaterial.SetFloat(_seedID, Random.value);
            _transitionHandle = CoroutineUtility.Run(TransitionRoutine(0f, 0.5f));
        }
    }

    public override void Close()
    {
        if (_transitionHandle.IsValid) CoroutineUtility.Stop(_transitionHandle);

        if (_transitionMaterial != null)
        {
            _transitionHandle = CoroutineUtility.Run(TransitionRoutine(0.5f, 1.0f, () => base.Close()));
        }
        else
        {
            base.Close();
        }
    }

    private IEnumerator TransitionRoutine(float start, float end, System.Action onComplete = null)
    {
        var time = 0f;
        UpdateShaderParameters(start);

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            var progress = Mathf.Lerp(start, end, Mathf.Clamp01(time / transitionDuration));
            UpdateShaderParameters(progress);
            yield return null;
        }

        UpdateShaderParameters(end);
        _transitionHandle = CoroutineHandle.Invalid;
        onComplete?.Invoke();
    }

    private void UpdateShaderParameters(float value)
    {
        _transitionMaterial.SetFloat(_progressID, value);
        _transitionMaterial.SetFloat(_bgThresholdID, Mathf.Abs(1.0f - value * 2.0f) - 0.5f);
        _transitionMaterial.SetFloat(_colorThresholdID, Mathf.Min(1.0f, Mathf.Abs(-4.0f + value * 8.0f)) * 0.48f);
    }
}
