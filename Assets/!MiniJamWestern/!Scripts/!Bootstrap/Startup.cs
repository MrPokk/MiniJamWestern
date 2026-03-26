using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using InGame.Script.Component_Sound;
using UINotDependence.Core;
using UnityEngine;
using UnityEngine.EventSystems;

public class Startup : EcsUnityRoot<Startup>
{
    [Header("Debug Settings")]
    [SerializeField] private bool _useDebug;
    [SerializeField] private bool _useDebugShop;
    [SerializeField] private DifficultyTier _debugTier;

    [Header("Main Settings")]
    [SerializeField] private CameraObjectComponent _cameraObject;
    [SerializeField] private GridConfig _playfieldConfig;
    [SerializeField] private ComplicationSettings _complicationSettings;

    [SerializeField] private MonoGridPresenter _playfield;
    [SerializeField] private GridParentObject _gridParent;

    private static EventSystem s_eventSystem;
    private static CameraObjectComponent s_mainCamera;
    private static bool s_isFirstStart;

    public static CameraObjectComponent MainCamera => s_mainCamera;


    protected override void Bootstrap()
    {
#if !UNITY_EDITOR
        _useDebug = false;
#endif
        InitializeUI();
        InitializeSound();
        InitializeToStartFloating();
        InitializeGrid();
        InitializeEventSystem();
        InitializePlaying();
        DebugShope();
    }

    private void InitializeSound()
    {
        DontDestroyOnLoad(SoundController.Instance);
    }

    private void InitializeToStartFloating()
    {
        if (s_isFirstStart != false && !_useDebug)
        {
            SoundController.PlayMusic(SoundType.Music);
            return;
        }

        s_isFirstStart = true;

        UIToStartFloating.OnFloatingFinished += OpenTransitionPopup;
        UIController.OpenScreen<UIToStartFloating>();
    }

    private void OpenTransitionPopup()
    {
        UIToStartFloating.OnFloatingFinished -= OpenTransitionPopup;
        UIController.OpenPopup<UIEffectTransitionPopup>();
        UIController.ClosePopup<UIEffectTransitionPopup>();
        SoundController.PlayMusic(SoundType.Music);
    }

    private void InitializeUI()
    {
        if (s_mainCamera == null)
        {
            s_mainCamera = _cameraObject;
            DontDestroyOnLoad(s_mainCamera);
        }
        else
        {
            if (_cameraObject != s_mainCamera)
            {
                Destroy(_cameraObject.gameObject);
            }
        }

        var attackVisualSystem = EcsSystemStatic.GetSystem<AttackVisualSystem>();
        attackVisualSystem.Setup(s_mainCamera);

        UIInit.Initialize("UI", s_mainCamera.CameraTarget);
    }

    private void InitializeEventSystem()
    {
        if (s_eventSystem == null)
        {
            s_eventSystem = FindFirstObjectByType<EventSystem>();
            if (s_eventSystem == null)
            {
                s_eventSystem = new Loader<EventSystem>(SettingsPaths.EVENT_SYSTEM).New();
            }
            DontDestroyOnLoad(s_eventSystem);
        }
    }

    private void DebugShope()
    {
        if (_useDebugShop)
        {
            EcsSystemStatic.GetSystem<ShopSystem>().OpenShop();
        }
    }

    private void InitializePlaying()
    {
        var playerPref = new Loader<TagPlayerProvider>(PrefabObjectsPaths.PLAYER_ENTITY).Prefab();
        GridInteractionHandler.InstantiateObject(new Vector2Int(0, 1), playerPref, out _);

        new GFlow(new(_complicationSettings)).Play();

        var waveSystem = EcsSystemStatic.GetSystem<EnemyWaveSystem>();
        waveSystem.Setup(_complicationSettings);
        if (_useDebug)
            waveSystem.SpawnCurrentWave(_debugTier);
        else
            waveSystem.SpawnCurrentWave();
    }

    private void InitializeGrid()
    {
        _playfield = new MonoGridPresenter(_playfieldConfig);
        GridInteractionHandler.Initialize(_playfield, _gridParent.gameObject);
    }
}
