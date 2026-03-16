using System;
using BitterECS.Core;
using UINotDependence.Core;
using UnityEngine;
using UnityEngine.UI;

public class UIChamberPopup : UIPopup
{
    [SerializeField] private Button _nextTurnBtn;

    public override void Open()
    {
        _nextTurnBtn.onClick.AddListener(OnNext);

        base.Open();
    }

    private void OnNext() => GFlow.RefreshTurn();


    public override void Close()
    {
        _nextTurnBtn.onClick.RemoveListener(OnNext);

        base.Close();
    }
}
