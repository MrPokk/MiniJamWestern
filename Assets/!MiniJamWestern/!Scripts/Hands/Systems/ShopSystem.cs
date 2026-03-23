using UnityEngine;
using DG.Tweening;
using UINotDependence.Core;

public class ShopSystem : MonoBehaviour
{
    public UIShopPopup shopPopup;
    public GameObject cardPrefab;

    public void OpenShop()
    {
        UIController.OpenPopup<UIShopPopup>();
        shopPopup.Clear();
        SpawnCards();
    }

    private void SpawnCards()
    {
        for (int i = 0; i < 3; i++)
        {
            CreateRandomCard(i);
        }
    }

    private void CreateRandomCard(int index)
    {
        var go = Instantiate(cardPrefab);
        var card = go.GetComponent<ShopCard>();

        int rand = Random.Range(0, 3);
        if (rand == 0) card.AssignHeal(5);
        else if (rand == 1) card.AssignPerk(null, "Speed", "Move faster");
        else card.AssignActions(new TagActions(), new TagActions());

        card.OnSelected = OnCardSelected;

        shopPopup.AddCard(card);

        card.visual.localScale = Vector3.zero;
        card.visual.DOScale(Vector3.one, 0.5f)
            .SetDelay(index * 0.2f)
            .SetEase(Ease.OutBack);
    }

    private void OnCardSelected(ShopCard card)
    {
        Debug.Log("Bought: " + card.titleLabel.text);
        CloseShop();
    }

    public void CloseShop()
    {
        UIController.ClosePopup<UIShopPopup>();
    }
}
