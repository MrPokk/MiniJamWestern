using System;
using BitterECS.Core;
using TMPro;
using UINotDependence.Core;
using UnityEngine;
using UnityEngine.UI;

public class UIChamberPopup : UIPopup
{
    [SerializeField] private Button _nextTurnBtn;
    [SerializeField] private TMP_Text _progressText;

    public override void Open()
    {
        _nextTurnBtn.onClick.AddListener(OnNext);

        GFlow.OnTransferProgressChanged += UpdateTransferProgress;

        UpdateTransferProgress(GFlow.GState.TransferProgress, GFlow.GState.TransferProgressMax);

        base.Open();
    }

    private void UpdateTransferProgress(int current, int max)
    {
        _progressText.text = $"{current} / {max}";
    }

    private void OnNext() => GFlow.RefreshTurn();

    public override void Close()
    {
        _nextTurnBtn.onClick.RemoveListener(OnNext);

        GFlow.OnTransferProgressChanged -= UpdateTransferProgress;

        base.Close();
    }
}
