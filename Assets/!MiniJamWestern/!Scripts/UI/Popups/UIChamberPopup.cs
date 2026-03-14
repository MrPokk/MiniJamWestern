using UINotDependence.Core;
using UnityEngine;
using UnityEngine.UI;

public class UIChamberPopup : UIPopup
{
    [SerializeField] private Button _nextTurn;

    public override void Open()
    {
        _nextTurn.onClick.AddListener(OnNext);

        base.Open();
    }

    private void OnNext()
    {
        Debug.Log("Next turn button clicked");

        GFlow.RefreshTurn();
    }

    public override void Close()
    {
        _nextTurn.onClick.RemoveListener(OnNext);
    }
}
