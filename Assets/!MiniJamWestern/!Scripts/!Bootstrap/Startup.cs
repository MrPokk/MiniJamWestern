using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UINotDependence.Core;
using UnityEngine;
using UnityEngine.EventSystems;

public class Startup : EcsUnityRoot<Startup>
{
    private CameraObject _cameraObject;
    [SerializeField] private GridConfig _playfieldConfig;
    [SerializeField] private ComplicationSettings _complicationSettings;

    [SerializeField] private MonoGridPresenter _playfield;
    [SerializeField] private GridParentObject _gridParent;

    protected override void Bootstrap()
    {
        InitializeCamera();
        InitializeUI();
        InitializeGrid();
        InitializeEventSystem();
        InitializePlaying();
    }

    private void InitializeCamera()
    {
        DontDestroyOnLoad(_cameraObject = new Loader<CameraObject>(EnvironmentPaths.CAMERA_OBJECT).New());
    }

    private void InitializeUI()
    {
        UIInit.Initialize("UI", _cameraObject.CameraTarget);
    }

    private void InitializePlaying()
    {
        var goblinPref = new Loader<TagEnemyProvider>(PrefabObjectsPaths.GOBLIN_ENTITY).Prefab();
        var playerPref = new Loader<TagPlayerProvider>(PrefabObjectsPaths.PLAYER_ENTITY).Prefab();
        GridInteractionHandler.InstantiateObject(new Vector2Int(0, 4), goblinPref, out _);
        GridInteractionHandler.InstantiateObject(new Vector2Int(0, 2), playerPref, out _);

        new GFlow(new(_complicationSettings)).Play();
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
