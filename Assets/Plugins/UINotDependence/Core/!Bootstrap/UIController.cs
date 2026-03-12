using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UINotDependence.Core
{
    public class UIController : MonoBehaviour
    {
        private static UIController _instance;
        private WindowsContainer _uiContainer;
        private bool _isInitialized;

        public static UIController Instance => _instance ??= FindOrCreateInstance();
        public bool IsInitialized => _isInitialized;

        private static UIController FindOrCreateInstance()
        {
            var found = FindFirstObjectByType<UIController>();
            if (found) return found;

            var go = new GameObject(nameof(UIController));
            return go.AddComponent<UIController>();
        }

        public void Initialize(WindowsContainer windowsContainer)
        {
            if (_isInitialized) return;

            _uiContainer = windowsContainer;
            _instance = this;
            _isInitialized = true;
        }

        private void OnDestroy()
        {
            if (_instance != this) return;
            CloseAllPopupsInstance();
            CloseScreenInstance();
            _instance = null;
        }

        public static IWindowBinder GetCurrentScreen => Instance._uiContainer.OpenedScreenBinder;
        public static IReadOnlyList<IWindowBinder> GetCurrentPopups => Instance._uiContainer.OpenedBinders.Values.ToList();

        public static bool TryGetOpenedPopup<T>(out T popup) where T : UIPopup => Instance.TryGetOpenedPopupInstance(out popup);
        public static T OpenScreen<T>() where T : UIScreen => Instance.OpenScreenInstance<T>();
        public static void CloseScreen() => Instance.CloseScreenInstance();
        public static T OpenPopup<T>() where T : UIPopup => Instance.OpenPopupInstance<T>();
        public static void ChangePopup<T>() where T : UIPopup => Instance.ChangePopupInstance<T>();
        public static void ClosePopup<T>() where T : UIPopup => Instance.ClosePopupInstance<T>();
        public static void CloseAllPopups() => Instance.CloseAllPopupsInstance();

        private bool TryGetOpenedPopupInstance<T>(out T popup) where T : UIPopup
        {
            if (_uiContainer.OpenedBinders.TryGetValue(typeof(T), out var binder))
            {
                popup = binder as T;
                return true;
            }
            popup = null;
            return false;
        }

        private T OpenScreenInstance<T>() where T : UIScreen
        {
            if (_uiContainer.OpenedScreenBinder != null && _uiContainer.OpenedScreenBinder.GetType() == typeof(T))
                return _uiContainer.OpenedScreenBinder as T;

            CloseScreenInstance();

            var binder = CreateAndBindWindow<T>(isScreen: true) ?? throw new Exception($"Can't open screen {typeof(T)}");
            _uiContainer.OpenedScreenBinder = binder;
            binder.Open();
            return binder as T;
        }

        private void CloseScreenInstance()
        {
            _uiContainer?.OpenedScreenBinder?.Close();
            if (_uiContainer != null) _uiContainer.OpenedScreenBinder = null;
        }

        private T OpenPopupInstance<T>() where T : UIPopup
        {
            CloseExistingPopup<T>();
            var binder = CreateAndBindWindow<T>(isScreen: false);
            var isOpened = _uiContainer.TryAddOpenedBinder(typeof(T), binder);
            if (isOpened == false) throw new Exception($"Can't open popup {typeof(T)}");
            if (binder == null) throw new Exception($"Can't open popup {typeof(T)}");
            binder.Open();
            return binder as T;
        }

        private void ClosePopupInstance<T>() where T : UIPopup
        {
            if (TryFindPopupInContainer<T>(out var existingPopup))
            {
                existingPopup.Close();
                _uiContainer.TryRemoveOpenedBinder(typeof(T));
            }
            else if (_uiContainer.OpenedBinders.TryGetValue(typeof(T), out var binder))
            {
                binder.Close();
                _uiContainer.TryRemoveOpenedBinder(typeof(T));
            }
        }

        private void CloseAllPopupsInstance()
        {
            if (_uiContainer == null) return;

            foreach (var binder in _uiContainer.OpenedBinders.Values.ToList())
            {
                binder?.Close();
            }
            _uiContainer.ClearOpenedBinders();
        }

        private void ChangePopupInstance<T>() where T : UIPopup
        {
            if (_uiContainer.OpenedBinders.ContainsKey(typeof(T)))
                ClosePopupInstance<T>();
            else
                OpenPopupInstance<T>();
        }

        private bool TryFindPopupInContainer<T>(out T popup) where T : UIPopup
        {
            popup = _uiContainer?.PopupsContainer?.GetComponentsInChildren<T>()?.FirstOrDefault();
            return popup != null;
        }

        private void CloseExistingPopup<T>() where T : UIPopup
        {
            if (TryFindPopupInContainer<T>(out var existingPopup))
            {
                existingPopup.Close();
                _uiContainer.TryRemoveOpenedBinder(typeof(T));
            }
        }

        private IWindowBinder CreateAndBindWindow<T>(bool isScreen) where T : WindowBinder
        {
            if (_uiContainer == null || !_uiContainer.TryGetBinder(typeof(T), out var binderPrefab))
            {
                Debug.LogError($"WindowBinder of type {typeof(T)} not found");
                return null;
            }

            var parent = isScreen ? _uiContainer.ScreensContainer : _uiContainer.PopupsContainer;
            var windowObject = Instantiate(binderPrefab.gameObject, parent);
            return windowObject.GetComponent<IWindowBinder>();
        }
    }
}
