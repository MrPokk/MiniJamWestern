using System;
using BitterECS.Core;
using UINotDependence.Core;
using UnityEngine;
using UnityEngine.UI;

public class UIChamberPopup : UIPopup
{
    [SerializeField] private Button _nextTurnBtn;
    [SerializeField] private Slider _swapSl;

    public override void Open()
    {
        _nextTurnBtn.onClick.AddListener(OnNext);
        GFlow.OnTransferProgressChanged += UpdateSlider;

        _swapSl.maxValue = GFlow.GState.TransferProgressMax;
        _swapSl.value = GFlow.GState.TransferProgress;

        base.Open();
    }

    private void OnNext() => GFlow.RefreshTurn();

    private void UpdateSlider(int current, int max)
    {
        if (_swapSl == null)
        {
            return;
        }

        _swapSl.maxValue = max;
        _swapSl.value = current;
    }

    public override void Close()
    {
        _nextTurnBtn.onClick.RemoveListener(OnNext);
        GFlow.OnTransferProgressChanged -= UpdateSlider;

        base.Close();
    }
}
