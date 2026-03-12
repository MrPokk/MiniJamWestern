using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UINotDependence.Core
{
    public static class UIFactory
    {
        public static UIController Create(Dictionary<Type, WindowBinder> binders)
        {
            var rootManager = UIController.Instance;

            if (rootManager.IsInitialized) return rootManager;

            var canvas = CreateCanvas(rootManager.transform);
            var screens = CreateUIContainer("UIScreens", canvas);
            var popups = CreateUIContainer("UIPopups", canvas);

            rootManager.Initialize(new WindowsContainer(popups, screens, binders));
            Object.DontDestroyOnLoad(rootManager.gameObject);

            return rootManager;
        }

        private static Transform CreateCanvas(Transform parent)
        {
            var canvasObject = new GameObject("UIRootCanvas");
            canvasObject.transform.SetParent(parent);

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(960, 540);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            return canvasObject.transform;
        }

        private static Transform CreateUIContainer(string name, Transform parent)
        {
            var container = new GameObject(name).AddComponent<RectTransform>();
            container.SetParent(parent);
            SetupFullStretch(container);
            return container;
        }

        private static void SetupFullStretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
        }
    }
}
