using UnityEngine;

namespace UINotDependence.Core
{
    public class UIPopup : WindowBinder
    {
        [SerializeField] private GameObject _btnCloseObject;
        [SerializeField] private GameObject _btnAlternativeCloseObject;

        private IUIProvider _btnClose;
        private IUIProvider _btnAlternativeClose;

        private void Awake()
        {
            if (_btnCloseObject != null) _btnClose = _btnCloseObject.GetComponent<IUIProvider>();
            if (_btnAlternativeCloseObject != null) _btnAlternativeClose = _btnAlternativeCloseObject.GetComponent<IUIProvider>();
        }

        public override void Open()
        {
            AddListeners();
            base.Open();
        }

        public override void Close()
        {
            RemoveListeners();
            base.Close();
        }

        private void AddListeners()
        {
            _btnClose?.AddListener(OnCloseClicked);
            _btnAlternativeClose?.AddListener(OnCloseClicked);
        }

        private void RemoveListeners()
        {
            _btnClose?.RemoveListener(OnCloseClicked);
            _btnAlternativeClose?.RemoveListener(OnCloseClicked);
        }

        private void OnCloseClicked() => Close();
    }
}
