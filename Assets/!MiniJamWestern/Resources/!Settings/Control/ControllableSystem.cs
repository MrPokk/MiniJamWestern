using BitterECS.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllableSystem : IEcsPreInitSystem, IEcsDestroySystem
{
    public Priority Priority => Priority.FIRST_TASK;

    private static ControlsConfig s_inputs;
    private static readonly List<SubscriptionInfo> s_subscriptions = new();

    private static Vector2 s_pointerPosition;
    public static ref Vector2 PointerPosition => ref s_pointerPosition;
    private static Camera s_camera;

    private static void UpdateCamera()
    {
        if (s_camera == null || !s_camera.gameObject.activeInHierarchy)
            s_camera = Camera.main;
    }

    public static Vector3 GetPointerPositionWorld()
    {
        UpdateCamera();
        return s_camera.ScreenToWorldPoint(new Vector3(s_pointerPosition.x, s_pointerPosition.y, s_camera.nearClipPlane));
    }

    public static ControlsConfig Inputs => s_inputs;

    public void PreInit()
    {
        UnsubscribeAll();

        s_inputs ??= new ControlsConfig();
        s_inputs.Enable();

        SubscribePerformed(Inputs.UI.Point, OnPointerPositionChanged);
    }

    private void OnPointerPositionChanged(InputAction.CallbackContext context)
    {
        s_pointerPosition = context.ReadValue<Vector2>();
    }

    public void Destroy()
    {
        UnsubscribeAll();
        s_inputs?.Disable();
    }

    public static void SubscribePerformed(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        if (action == null || callback == null)
            return;

        if (IsAlreadySubscribed(action, callback, SubscriptionType.Performed))
            return;

        action.performed += callback;
        s_subscriptions.Add(new SubscriptionInfo(action, callback, SubscriptionType.Performed));
    }

    public static void SubscribeStarted(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        if (action == null || callback == null)
            return;

        if (IsAlreadySubscribed(action, callback, SubscriptionType.Started))
            return;

        action.started += callback;
        s_subscriptions.Add(new SubscriptionInfo(action, callback, SubscriptionType.Started));
    }

    public static void SubscribeCanceled(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        if (action == null || callback == null)
            return;

        if (IsAlreadySubscribed(action, callback, SubscriptionType.Canceled))
            return;

        action.canceled += callback;
        s_subscriptions.Add(new SubscriptionInfo(action, callback, SubscriptionType.Canceled));
    }

    private static bool IsAlreadySubscribed(InputAction action, Action<InputAction.CallbackContext> callback, SubscriptionType type)
    {
        foreach (var sub in s_subscriptions)
        {
            if (sub.Action == action && sub.Callback == callback && sub.Type == type)
                return true;
        }
        return false;
    }

    public static void UnsubscribeAll()
    {
        foreach (var subscription in s_subscriptions)
        {
            subscription.Unsubscribe();
        }
        s_subscriptions.Clear();
    }

    public static void Unsubscribe(
        InputAction action,
        Action<InputAction.CallbackContext> callback,
        SubscriptionType type)
    {
        if (action == null || callback == null)
            return;

        for (var i = s_subscriptions.Count - 1; i >= 0; i--)
        {
            var subscription = s_subscriptions[i];
            if (subscription.Action == action &&
                subscription.Callback == callback &&
                subscription.Type == type)
            {
                subscription.Unsubscribe();
                s_subscriptions.RemoveAt(i);
                break;
            }
        }
    }

    public static void UnsubscribeStarted(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        Unsubscribe(action, callback, SubscriptionType.Started);
    }

    public static void UnsubscribePerformed(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        Unsubscribe(action, callback, SubscriptionType.Performed);
    }

    public static void UnsubscribeCanceled(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        Unsubscribe(action, callback, SubscriptionType.Canceled);
    }

    private class SubscriptionInfo
    {
        public InputAction Action { get; }
        public Action<InputAction.CallbackContext> Callback { get; }
        public SubscriptionType Type { get; }

        public SubscriptionInfo(InputAction action, Action<InputAction.CallbackContext> callback, SubscriptionType type)
        {
            Action = action;
            Callback = callback;
            Type = type;
        }

        public void Unsubscribe()
        {
            switch (Type)
            {
                case SubscriptionType.Performed:
                    Action.performed -= Callback;
                    break;
                case SubscriptionType.Started:
                    Action.started -= Callback;
                    break;
                case SubscriptionType.Canceled:
                    Action.canceled -= Callback;
                    break;
            }
        }
    }

    public enum SubscriptionType
    {
        Performed,
        Started,
        Canceled
    }
}
