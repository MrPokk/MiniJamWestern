using System;
using BitterECS.Core;
using BitterECS.Integration.Unity;
using UnityEngine;

public static class GridInteractionHandler
{
    private static GridInteractionHandlerInstance _instance;
    public static GridInteractionHandlerInstance Instance => _instance ?? throw new ArgumentNullException("GridInteractionHandlerInstance is not initialized.");

    public static void Initialize(MonoGridPresenter playfield, GameObject playfieldGameObject) => _instance = new GridInteractionHandlerInstance(playfield, playfieldGameObject);

    public static ProviderEcs Extraction(Vector2Int index) => Instance.Extraction(index);
    public static ProviderEcs Extraction(Vector3 indexWorld) => Instance.Extraction(indexWorld);
    public static bool Placing(Vector2Int index, ProviderEcs entityObject) => Instance.Placing(index, entityObject);
    public static bool Placing(Vector3 indexWorld, ProviderEcs entityObject) => Instance.Placing(indexWorld, entityObject);
    public static bool Moving(Vector2Int fromIndex, Vector2Int toIndex) => Instance.Moving(fromIndex, toIndex);
    public static bool Moving(Vector3 fromWorldPosition, Vector3 toWorldPosition) => Instance.Moving(fromWorldPosition, toWorldPosition);
    public static bool IsPlacing(Vector2Int index) => Instance.IsPlacing(index);
    public static bool IsPlacing(Vector3 indexWorld) => Instance.IsPlacing(indexWorld);
    public static void InstantiateObject(Vector2Int index, ProviderEcs prefab, out ProviderEcs instantiateObject) => Instance.InstantiateObject(index, prefab, out instantiateObject);
    public static void InstantiateObject(Vector3 indexWorld, ProviderEcs prefab, out ProviderEcs instantiateObject) => Instance.InstantiateObject(indexWorld, prefab, out instantiateObject);
    public static bool TryGetEntityAt(Vector2Int index, out EcsEntity entity) => Instance.TryGetEntityAt(index, out entity);
    public static bool TryGetEntityAt(Vector3 worldPosition, out EcsEntity entity) => Instance.TryGetEntityAt(worldPosition, out entity);
    public static bool MoveEntity(EcsEntity entity, Vector2Int toIndex) => Instance.MoveEntity(entity, toIndex);
    public static bool MoveEntity(EcsEntity entity, Vector3 toWorldPosition) => Instance.MoveEntity(entity, toWorldPosition);

    public class GridInteractionHandlerInstance
    {
        public readonly MonoGridPresenter _playfield;
        private readonly GameObject _playfieldGameObject;

        public GridInteractionHandlerInstance(MonoGridPresenter playfield, GameObject playfieldGameObject)
        {
            _playfield = playfield;
            _playfieldGameObject = playfieldGameObject;
        }

        public ProviderEcs Extraction(Vector2Int index)
        {
            var objectExtract = _playfield.GetGameObject(index);
            if (objectExtract == null) return null;

            _playfield.ExtractGameObject(index);
            objectExtract.Entity.AddFrame<IsExtractionEvent>();

            return objectExtract;
        }

        public ProviderEcs Extraction(Vector3 indexWorld) => Extraction(_playfield.ConvertingPosition(indexWorld));

        public bool Placing(Vector2Int index, ProviderEcs entityObject)
        {
            if (entityObject == null) return false;

            var isSet = _playfield.TrySetGameObject(index, entityObject);
            if (!isSet) return false;

            entityObject.Entity.Add<GridComponent>(new(index, _playfield));
            entityObject.Entity.AddFrame<IsPlacingEvent>();

            return true;
        }

        public bool Placing(Vector3 indexWorld, ProviderEcs entityObject) => Placing(_playfield.ConvertingPosition(indexWorld), entityObject);

        public bool Moving(Vector2Int fromIndex, Vector2Int toIndex)
        {
            var entityObject = _playfield.GetGameObject(fromIndex);
            if (entityObject == null) return false;

            var isMoved = _playfield.MoveGameObject(fromIndex, toIndex);
            if (!isMoved) return false;

            entityObject.Entity.Add<GridComponent>(new(toIndex, _playfield));
            entityObject.Entity.AddFrame<IsExtractionEvent>();
            entityObject.Entity.AddFrame<IsPlacingEvent>();

            return true;
        }

        public bool Moving(Vector3 fromWorldPosition, Vector3 toWorldPosition) => Moving(_playfield.ConvertingPosition(fromWorldPosition), _playfield.ConvertingPosition(toWorldPosition));

        public bool IsPlacing(Vector2Int index) => _playfield.HasNotGameObject(index) && _playfield.IsWithinGrid(index);

        public bool IsPlacing(Vector3 indexWorld) => IsPlacing(_playfield.ConvertingPosition(indexWorld));

        public void InstantiateObject(Vector2Int index, ProviderEcs prefab, out ProviderEcs instantiateObject)
        {
            var isSet = _playfield.InitializeGameObject(index, prefab, out instantiateObject, _playfieldGameObject.transform);
            if (!isSet) return;

            instantiateObject.Entity.Add<GridComponent>(new(index, _playfield));
            instantiateObject.Entity.AddFrame<IsInstantiateEvent>();
            instantiateObject.Entity.AddFrame<IsActivatingEvent>();
        }

        public void InstantiateObject(Vector3 indexWorld, ProviderEcs prefab, out ProviderEcs instantiateObject) => InstantiateObject(_playfield.ConvertingPosition(indexWorld), prefab, out instantiateObject);

        public bool TryGetEntityAt(Vector2Int index, out EcsEntity entity)
        {
            var provider = _playfield.GetGameObject(index);
            if (provider != null)
            {
                entity = provider.Entity;
                return true;
            }
            entity = default;
            return false;
        }

        public bool TryGetEntityAt(Vector3 worldPosition, out EcsEntity entity) => TryGetEntityAt(_playfield.ConvertingPosition(worldPosition), out entity);

        public bool MoveEntity(EcsEntity entity, Vector2Int toIndex)
        {
            if (!entity.Has<GridComponent>())
            {
                Debug.LogWarning("MoveEntity: entity has no GridComponent");
                return false;
            }

            var fromIndex = entity.Get<GridComponent>().currentPosition;
            return Moving(fromIndex, toIndex);
        }

        public bool MoveEntity(EcsEntity entity, Vector3 toWorldPosition) => MoveEntity(entity, _playfield.ConvertingPosition(toWorldPosition));
    }
}
