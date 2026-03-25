using UnityEngine;
using DG.Tweening;

public class WateryCardController : MonoBehaviour
{
    private Material _mat;
    private Camera _cam;

    [Header("Physical Settings")]
    public float smoothSpeed = 8f;
    public float tiltSensitivity = 2f;
    public float maxTilt = 0.4f;

    [Header("Watery Noise Settings")]
    public float noiseSpeed = 0.3f;
    public float noiseStrength = 0.1f;

    private Vector2 _currentTilt;
    private float _noiseSeedX;
    private float _noiseSeedY;

    void Start()
    {
        _cam = Camera.main;
        if (TryGetComponent<Renderer>(out var r)) _mat = r.material;

        _noiseSeedX = Random.value * 100f;
        _noiseSeedY = Random.value * 100f;
    }

    void Update()
    {
        var pointerPos = ControllableSystem.PointerPosition;
        var cardScreenPos = _cam.WorldToScreenPoint(transform.position);

        var targetTilt = new Vector2(
            (pointerPos.y - cardScreenPos.y) / Screen.height,
            (pointerPos.x - cardScreenPos.x) / Screen.width
        );

        targetTilt *= tiltSensitivity;
        targetTilt.x = Mathf.Clamp(targetTilt.x, -maxTilt, maxTilt);
        targetTilt.y = Mathf.Clamp(targetTilt.y, -maxTilt, maxTilt);

        _currentTilt = Vector2.Lerp(_currentTilt, targetTilt, Time.deltaTime * smoothSpeed);

        if (_mat != null)
        {
            var nX = Mathf.PerlinNoise(_noiseSeedX, Time.time * noiseSpeed);
            var nY = Mathf.PerlinNoise(_noiseSeedY, Time.time * noiseSpeed);

            var finalTilt = new Vector2(
                _currentTilt.x + (nX * noiseStrength),
                _currentTilt.y + (nY * noiseStrength)
            );

            _mat.SetVector("_Tilt", finalTilt);
        }
    }

    public void Flash(Color color, float duration, float maxIntensity = 1f)
    {
        if (_mat == null) return;

        DOTween.Kill(_mat);

        _mat.SetColor("_FlashColor", color);

        Sequence sequence = DOTween.Sequence();
        sequence.SetId(_mat);

        sequence.Append(DOTween.To(() => _mat.GetFloat("_FlashIntensity"), x => _mat.SetFloat("_FlashIntensity", x), maxIntensity, duration * 0.5f));
        sequence.Append(DOTween.To(() => _mat.GetFloat("_FlashIntensity"), x => _mat.SetFloat("_FlashIntensity", x), 0f, duration * 0.5f));
        sequence.OnComplete(() => _mat.SetColor("_FlashColor", Color.white));

        sequence.Play();
    }
}
