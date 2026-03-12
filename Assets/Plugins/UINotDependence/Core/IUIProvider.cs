using System;
using UnityEngine;

namespace UINotDependence.Core
{
    public interface IUIProvider
    {
        public GameObject UIObject { get; }

        void AddListener(Action actionClick);
        void RemoveListener(Action actionClick);
        void SetSelectNeighbors(GameObject upNeighbour,
                                 GameObject downNeighbour,
                                 GameObject leftNeighbour,
                                 GameObject rightNeighbour);
        void Refresh();
    }
}
