using BitterECS.Core;
using BitterECS.Integration.Unity;
using UINotDependence.Core;
using UnityEngine;
using UnityEngine.EventSystems;

public class Startup : EcsUnityRoot<Startup>
{

    [Header("Debug Settings")]
    [SerializeField] private bool _useDebugWave;
    [SerializeField] private DifficultyTier _debugTier;

    [Header("Main Settings")]
    [SerializeField] private CameraObjectComponent _cameraObject;
    [SerializeField] private GridConfig _playfieldConfig;
    [SerializeField] private ComplicationSettings _complicationSettings;


    [SerializeField] private MonoGridPresenter _playfield;
    [SerializeField] private GridParentObject _gridParent;

    protected override void Bootstrap()
    {
        InitializeUI();
        InitializeGrid();
        InitializeEventSystem();
        InitializePlaying();
    }

    private void InitializeUI()
    {
        DontDestroyOnLoad(_cameraObject);
        var attackVisualSystem = EcsSystemStatic.GetSystem<AttackVisualSystem>();
        attackVisualSystem.Setup(_cameraObject);

        UIInit.Initialize("UI", _cameraObject.CameraTarget);
    }

    private void InitializePlaying()
    {
        var playerPref = new Loader<TagPlayerProvider>(PrefabObjectsPaths.PLAYER_ENTITY).Prefab();
        GridInteractionHandler.InstantiateObject(new Vector2Int(0, 1), playerPref, out _);

        new GFlow(new(_complicationSettings)).Play();

        var waveSystem = EcsSystemStatic.GetSystem<EnemyWaveSystem>();
        waveSystem.Setup(_complicationSettings);
        if (_useDebugWave)
            waveSystem.SpawnCurrentWave(_debugTier);
        else
            waveSystem.SpawnCurrentWave();
    }

    private void InitializeGrid()
    {
        _playfield = new MonoGridPresenter(_playfieldConfig);
        GridInteractionHandler.Initialize(_playfield, _gridParent.gameObject);
    }

    private void InitializeEventSystem()
    {
        new Loader<EventSystem>(SettingsPaths.EVENT_SYSTEM).New();
    }
}
