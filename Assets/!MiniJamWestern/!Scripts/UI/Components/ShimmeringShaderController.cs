using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Graphic))]
public class ShimmeringShaderController : MonoBehaviour
{
    private Graphic _graphic;
    private Material _materialInstance;

    private static readonly int Color1Prop = Shader.PropertyToID("_Color1");
    private static readonly int Color2Prop = Shader.PropertyToID("_Color2");

    [SerializeField] private Color _color1 = Color.white;
    [SerializeField] private Color _color2 = Color.black;

    public Color Color1
    {
        get => _color1;
        set => SetColor1(value);
    }

    public Color Color2
    {
        get => _color2;
        set => SetColor2(value);
    }

    private void Awake()
    {
        _graphic = GetComponent<Graphic>();

        if (_graphic.material != null)
        {
            _materialInstance = new Material(_graphic.material);
            _graphic.material = _materialInstance;
            UpdateShader();
        }
    }

    public void SetColor1(Color color)
    {
        _color1 = color;
        UpdateShader();
    }

    public void SetColor2(Color color)
    {
        _color2 = color;
        UpdateShader();
    }

    public void SetColors(Color color1, Color color2)
    {
        _color1 = color1;
        _color2 = color2;
        UpdateShader();
    }

    private void UpdateShader()
    {
        if (_materialInstance != null)
        {
            _materialInstance.SetColor(Color1Prop, _color1);
            _materialInstance.SetColor(Color2Prop, _color2);
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            UpdateShader();
        }
    }

    private void OnDestroy()
    {
        if (_materialInstance != null)
        {
            Destroy(_materialInstance);
        }
    }
}
