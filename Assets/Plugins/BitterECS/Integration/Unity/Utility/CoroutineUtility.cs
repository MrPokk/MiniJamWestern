using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BitterECS.Integration.Unity
{
    public sealed class CoroutineUtility : MonoBehaviour
    {
        private static CoroutineUtility s_instance;
        public static CoroutineUtility Instance
        {
            get
            {
                if (s_instance == null)
                {
                    var instanceFind = FindFirstObjectByType<CoroutineUtility>();
                    if (instanceFind == null)
                    {
                        s_instance = new GameObject("[CoroutineUtility]").AddComponent<CoroutineUtility>();
                    }
                    else
                    {
                        s_instance = instanceFind;
                    }
                }
                return s_instance;
            }
        }

        private readonly Dictionary<int, ActiveCoroutine> _activeCoroutines = new();
        private int _nextCoroutineId = 1;

        private event Action<int> OnCoroutineStopped;
        private event Action OnAllCoroutinesStopped;

        public static CoroutineHandle Run(IEnumerator coroutine)
        {
            if (coroutine == null)
            {
                Debug.LogWarning("Attempted to start a null coroutine.");
                return CoroutineHandle.Invalid;
            }

            return Instance.StartNewCoroutine(coroutine);
        }

        public static CoroutineHandle RunAfter(IEnumerator coroutine, CoroutineHandle previousHandle)
        {
            if (coroutine == null)
            {
                Debug.LogWarning("Attempted to start a null coroutine.");
                return CoroutineHandle.Invalid;
            }

            return Instance.StartCoroutineAfter(previousHandle, coroutine);
        }

        public static void Stop(CoroutineHandle handle)
        {
            if (!handle.IsValid)
            {
                return;
            }
            Instance.StopCoroutineInternal(handle);
        }

        public static void StopAll()
        {
            Instance.StopAllCoroutinesInternal();
        }

        public static void SubscribeToStop(Action<CoroutineHandle> callback)
        {
            if (callback == null)
            {
                return;
            }
            Instance.OnCoroutineStopped += (id) => callback(new CoroutineHandle(id));
        }

        public static void SubscribeToAllStop(Action callback)
        {
            if (callback == null)
            {
                return;
            }
            Instance.OnAllCoroutinesStopped += callback;
        }

        public static bool IsCoroutineActive(CoroutineHandle handle)
        {
            var id = handle.Id;
            if (id == -1) return false;

            return Instance._activeCoroutines.ContainsKey(id);
        }

        public static bool HasActiveCoroutines() => Instance._activeCoroutines.Count > 0;

        private CoroutineHandle StartCoroutineAfter(CoroutineHandle previousHandle, IEnumerator coroutine)
        {
            if (!previousHandle.IsValid || !IsCoroutineActive(previousHandle))
            {
                return StartNewCoroutine(coroutine);
            }

            var id = _nextCoroutineId++;
            var handle = new CoroutineHandle(id);

            Action<CoroutineHandle> onPreviousStopped = null;
            onPreviousStopped = (stoppedHandle) =>
            {
                if (stoppedHandle == previousHandle)
                {
                    var unityCoroutine = StartCoroutine(RunCoroutineWrapper(id, coroutine));
                    _activeCoroutines.Add(id, new ActiveCoroutine(unityCoroutine, handle));

                    OnCoroutineStopped -= (stopId) => onPreviousStopped?.Invoke(new CoroutineHandle(stopId));
                }
            };

            SubscribeToStop(onPreviousStopped);

            return handle;
        }

        private CoroutineHandle StartNewCoroutine(IEnumerator coroutine)
        {
            var id = _nextCoroutineId++;
            var handle = new CoroutineHandle(id);

            var unityCoroutine = StartCoroutine(RunCoroutineWrapper(id, coroutine));
            _activeCoroutines.Add(id, new ActiveCoroutine(unityCoroutine, handle));

            return handle;
        }

        private IEnumerator RunCoroutineWrapper(int id, IEnumerator coroutine)
        {
            try
            {
                yield return coroutine;
            }
            finally
            {
                if (_activeCoroutines.Remove(id, out var activeCoroutine))
                {
                    OnCoroutineStopped?.Invoke(id);
                    if (_activeCoroutines.Count == 0)
                    {
                        OnAllCoroutinesStopped?.Invoke();
                    }
                }
            }
        }

        private void StopCoroutineInternal(CoroutineHandle handle)
        {
            if (!_activeCoroutines.Remove(handle.Id, out var activeCoroutine))
            {
                return;
            }

            StopCoroutine(activeCoroutine.UnityCoroutine);
            OnCoroutineStopped?.Invoke(handle.Id);

            if (_activeCoroutines.Count == 0)
            {
                OnAllCoroutinesStopped?.Invoke();
            }
        }

        private void StopAllCoroutinesInternal()
        {
            foreach (var entry in _activeCoroutines.Values)
            {
                StopCoroutine(entry.UnityCoroutine);
                OnCoroutineStopped?.Invoke(entry.Handle.Id);
            }

            _activeCoroutines.Clear();
            OnAllCoroutinesStopped?.Invoke();
        }

        private readonly struct ActiveCoroutine
        {
            public readonly Coroutine UnityCoroutine;
            public readonly CoroutineHandle Handle;

            public ActiveCoroutine(Coroutine unityCoroutine, CoroutineHandle handle)
            {
                UnityCoroutine = unityCoroutine;
                Handle = handle;
            }
        }
    }


    public readonly struct CoroutineHandle : IEquatable<CoroutineHandle>
    {
        private readonly int _valuePlusOne;
        public int Id => _valuePlusOne - 1;

        public static readonly CoroutineHandle Invalid = new(-1);

        public bool IsValid => Id != -1 && CoroutineUtility.IsCoroutineActive(this);

        public CoroutineHandle(int id)
        {
            _valuePlusOne = id + 1;
        }

        public bool Equals(CoroutineHandle other) => Id == other.Id;
        public override bool Equals(object obj) => obj is CoroutineHandle other && Equals(other);
        public override int GetHashCode() => Id;
        public static bool operator ==(CoroutineHandle a, CoroutineHandle b) => a.Equals(b);
        public static bool operator !=(CoroutineHandle a, CoroutineHandle b) => !a.Equals(b);
    }
}
