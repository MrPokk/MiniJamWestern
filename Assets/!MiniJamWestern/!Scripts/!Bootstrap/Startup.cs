using BitterECS.Core;
using BitterECS.Integration.Unity;
using UINotDependence.Core;
using UnityEngine;
using UnityEngine.EventSystems;

public class Startup : EcsUnityRoot<Startup>
{
    [SerializeField] private GridConfig _playfieldConfig;
    [SerializeField] public GameObject gridParent;

    public MonoGridPresenter playfield;

    protected override void Bootstrap()
    {
        InitializeUI();
        InitializeGrid();
        InitializeEventSystem();
        InitializePlaying();
    }

    private void InitializeUI()
    {
        UIInit.Initialize();
    }

    private void InitializePlaying()
    {
        var playerPref = new Loader<TagPlayerProvider>(PrefabObjectsPaths.PLAYER_ENTITY).Prefab();
        GridInteractionHandler.InstantiateObject(new Vector2Int(0, 0), playerPref, out _);

        new GFlow(new(DifficultyTier.Tier1_Base)).Play();
    }

    private void InitializeGrid()
    {
        playfield = new MonoGridPresenter(_playfieldConfig);
    }

    private void InitializeEventSystem()
    {
        new Loader<EventSystem>(SettingsPaths.EVENT_SYSTEM).New();
    }
}

public class Test : IEcsInitSystem
{
    public Priority Priority => Priority.High;

    private EcsFilter<TagPlayer> _ecsEntities;
    public async void Init()
    {
        _ecsEntities.For((EcsEntity e, ref TagPlayer tag) =>
        {

        });
    }
}

