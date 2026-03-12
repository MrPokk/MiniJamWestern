using System;
using System.Collections.Generic;
using UnityEngine;
namespace UINotDependence.Core
{
    public class UIInit
    {
        public static void Initialize(string prefabPath = "UI")
        {
            if (UIController.Instance.IsInitialized) return;

            var allBinders = new Dictionary<Type, WindowBinder>();
            var allPrefabs = Resources.LoadAll<GameObject>(prefabPath);

            foreach (var prefab in allPrefabs)
            {
                if (prefab.TryGetComponent<WindowBinder>(out var binder))
                {
                    var type = binder.GetType();
                    allBinders.TryAdd(type, binder);
                }
            }

            UIFactory.Create(allBinders);
        }
    }
}
