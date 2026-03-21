using BitterECS.Core;
using UnityEngine;

namespace BitterECS.Integration.Unity
{
    [DefaultExecutionOrder(int.MinValue)]
    [DisallowMultipleComponent]
    public abstract class EcsUnityRoot<T> : MonoBehaviour where T : EcsUnityRoot<T>
    {
        private static T s_instance;
        public static T Instance
        {
            get
            {
                if (s_instance == null)
                {
                    var instanceFind = FindFirstObjectByType<T>();
                    if (instanceFind == null)
                    {
                        s_instance = new GameObject($"[{typeof(T).Name}]").AddComponent<T>();
                    }
                    else
                    {
                        s_instance = instanceFind;
                    }
                }
                return s_instance;
            }
        }

        protected virtual void Bootstrap() { }
        protected virtual void PostBootstrap() { }

        protected virtual void Awake()
        {
            EcsComponentTypes.Warmup();
            EcsSystemStatic.Load();
            EcsSystemStatic.Run<IEcsPreInitSystem>(system => system.PreInit());
            Bootstrap();
        }

        protected virtual void Start()
        {
            EcsSystemStatic.Run<IEcsInitSystem>(system => system.Init());
            PostBootstrap();
        }

        protected virtual void Update()
        {
            EcsSystemStatic.Run<IEcsRunSystem>(system => system.Run());
        }

        protected virtual void FixedUpdate()
        {
            EcsSystemStatic.Run<IEcsFixedRunSystem>(system => system.FixedRun());
        }

        protected virtual void LateUpdate()
        {
            EcsSystemStatic.Run<IEcsPostRunSystem>(system => system.PostRun());
        }

        protected virtual void OnDestroy()
        {
            EcsSystemStatic.Run<IEcsDestroySystem>(system => system.Destroy());
            EcsSystemStatic.Run<IEcsPostDestroySystem>(system => system.PostDestroy());

            EcsWorldStatic.Dispose();
            EcsSystemStatic.Dispose();
        }
    }
}
