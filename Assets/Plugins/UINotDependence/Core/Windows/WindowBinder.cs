using UnityEngine;

namespace UINotDependence.Core
{
    public abstract class WindowBinder : MonoBehaviour, IWindowBinder
    {
        public virtual void Open()
        {
            if (!this || !gameObject) return;
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            if (!this || !gameObject) return;
            gameObject.SetActive(false);

            if (this && gameObject && !gameObject.activeSelf)
            {
                Destroy(gameObject);
            }
        }
    }
}
