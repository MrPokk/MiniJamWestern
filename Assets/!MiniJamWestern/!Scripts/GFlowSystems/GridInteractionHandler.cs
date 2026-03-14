using BitterECS.Integration.Unity;
using UnityEngine;

public class GridInteractionHandler
{
    private static MonoGridPresenter Playfield => Startup.Instance.playfield;
    private static GameObject PlayfieldGameObject => Startup.Instance.gridParent;

    public static ProviderEcs Extraction(Vector2Int index)
    {
        var objectExtract = Playfield.GetGameObject(index);
        if (objectExtract == null) return null;

        Playfield.ExtractGameObject(index);

        objectExtract.Entity.AddFrame<IsExtractionEvent>();

        return objectExtract;
    }

    public static ProviderEcs Extraction(Vector3 indexWorld)
    {
        return Extraction(Playfield.ConvertingPosition(indexWorld));
    }

    public static bool Placing(Vector2Int index, ProviderEcs entityObject)
    {
        if (entityObject == null) return false;

        var isSet = Playfield.TrySetGameObject(index, entityObject);
        if (!isSet) return false;

        entityObject.Entity.Add<GridComponent>(new(index, Playfield));
        entityObject.Entity.AddFrame<IsPlacingEvent>();

        return true;
    }

    public static bool Placing(Vector3 indexWorld, ProviderEcs entityObject)
    {
        return Placing(Playfield.ConvertingPosition(indexWorld), entityObject);
    }

    public static bool Moving(Vector2Int fromIndex, Vector2Int toIndex)
    {
        var entityObject = Playfield.GetGameObject(fromIndex);
        if (entityObject == null) return false;

        var isMoved = Playfield.MoveGameObject(fromIndex, toIndex);
        if (!isMoved) return false;

        entityObject.Entity.Add<GridComponent>(new(toIndex, Playfield));
        entityObject.Entity.AddFrame<IsExtractionEvent>();
        entityObject.Entity.AddFrame<IsPlacingEvent>();

        return true;
    }

    public static bool Moving(Vector3 fromWorldPosition, Vector3 toWorldPosition)
    {
        return Moving(Playfield.ConvertingPosition(fromWorldPosition), Playfield.ConvertingPosition(toWorldPosition));
    }

    public static bool IsPlacing(Vector2Int index)
    {
        return Playfield.HasNotGameObject(index) && Playfield.IsWithinGrid(index);
    }

    public static bool IsPlacing(Vector3 indexWorld)
    {
        return IsPlacing(Playfield.ConvertingPosition(indexWorld));
    }

    public static void InstantiateObject(Vector2Int index, ProviderEcs prefab, out ProviderEcs instantiateObject)
    {
        var isSet = Playfield.InitializeGameObject(index, prefab, out instantiateObject, PlayfieldGameObject.transform);
        if (!isSet) return;

        instantiateObject.Entity.Add<GridComponent>(new(index, Playfield));

        if (instantiateObject is ProviderEcs _)
        {
            instantiateObject.Entity.AddFrame<IsInstantiateEvent>();
            instantiateObject.Entity.AddFrame<IsActivatingEvent>();
        }
    }

    public static void InstantiateObject(Vector3 indexWorld, ProviderEcs prefab, out ProviderEcs instantiateObject)
    {
        InstantiateObject(Playfield.ConvertingPosition(indexWorld), prefab, out instantiateObject);
    }
}
