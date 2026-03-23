using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    // Синглтон для доступа из любого места: TransitionManager.Instance.ChangeScene("Level2");
    public static TransitionManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image transitionImage; // Ссылка на компонент Image

    [Header("Transition Settings")][SerializeField] private float transitionDuration = 0.5f;

    private Material _transitionMaterial;
    private Coroutine _currentTransition;

    // Кэшируем ID параметров (для оптимизации производительности)
    private readonly int _progressID = Shader.PropertyToID("_Progress");
    private readonly int _bgThresholdID = Shader.PropertyToID("_BackgroundThreshold");
    private readonly int _colorThresholdID = Shader.PropertyToID("_ColorThreshold");
    private readonly int _seedID = Shader.PropertyToID("_Seed");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Делаем объект Persistent (как Autoload в Godot)

            // Создаем копию материала, чтобы не изменять ассет напрямую
            _transitionMaterial = new Material(transitionImage.material);
            transitionImage.material = _transitionMaterial;

            transitionImage.gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeScene(string sceneName)
    {
        if (_currentTransition != null)
        {
            StopCoroutine(_currentTransition);
        }

        _currentTransition = StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        transitionImage.gameObject.SetActive(true);
        _transitionMaterial.SetFloat(_seedID, Random.value);

        // --- Фаза 1: Закрытие экрана (от 0 до 0.5) ---
        float time = 0f;
        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / transitionDuration);
            float progress = Mathf.Lerp(0f, 0.5f, t);

            UpdateShaderParameters(progress);
            yield return null;
        }

        // --- Загрузка сцены ---
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null; // Ждем окончания загрузки
        }

        // --- Фаза 2: Открытие экрана (от 0.5 до 1.0) ---
        time = 0f;
        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / transitionDuration);
            float progress = Mathf.Lerp(0.5f, 1.0f, t);

            UpdateShaderParameters(progress);
            yield return null;
        }

        transitionImage.gameObject.SetActive(false);
        _currentTransition = null;
    }

    private void UpdateShaderParameters(float value)
    {
        _transitionMaterial.SetFloat(_progressID, value);

        // Точная копия математики из GDScript
        _transitionMaterial.SetFloat(_bgThresholdID, Mathf.Abs(1.0f - value * 2.0f) - 0.5f);
        _transitionMaterial.SetFloat(_colorThresholdID, Mathf.Min(1.0f, Mathf.Abs(-4.0f + value * 8.0f)) * 0.48f);
    }
}
