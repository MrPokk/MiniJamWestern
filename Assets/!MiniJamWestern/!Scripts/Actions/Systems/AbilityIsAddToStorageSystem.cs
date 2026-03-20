using BitterECS.Core;
using UnityEngine;

public class AbilityIsAddToStorageSystem : IEcsInitSystem
{
    public Priority Priority => Priority.High;

    private EcsEvent _ecsEvent;
    private EcsFilter<TagInventoryStorage> _storageFilter;

    public void Init()
    {
        _ecsEvent.Subscribe<IsExtract>(added: OnTarget);
    }

    private void OnTarget(EcsEntity abilityEntity)
    {
        if (_storageFilter.Count == 0) return;
        var storageEntity = _storageFilter.First();

        var storageProvider = storageEntity.GetProvider<AbilityInventoryProvider>();
        if (storageProvider == null) return;

        var view = abilityEntity.GetProvider<AbilityViewProvider>();
        if (view == null) return;

        storageProvider.AddFirstEmpty(view);
    }
}
