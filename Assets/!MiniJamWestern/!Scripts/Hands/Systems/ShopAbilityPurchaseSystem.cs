using System.Linq;
using BitterECS.Core;
using UnityEngine;
using DG.Tweening;

public class ShopAbilityPurchaseSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<TagInventoryEffects> _storageFilter;

    public void ProcessPurchase(ShopCard card)
    {
        if (card.Type == ShopCard.CardType.ACTION)
        {
            if (_storageFilter.Count == 0) return;

            var storageEntity = _storageFilter.First();
            var storageProvider = storageEntity.GetProvider<AbilityInventoryProvider>();

            if (card.AbilityView != null)
            {
                var abilityView = card.AbilityView;
                var instantiateAbility = Object.Instantiate(abilityView);

                storageProvider.AddFirstEmpty(instantiateAbility);

                var tempScale = instantiateAbility.transform.localScale;
                instantiateAbility.transform.localScale = Vector3.zero;
                instantiateAbility.transform.DOScale(tempScale, 0.3f)
                .SetEase(Ease.OutBack)
                .Play();
            }
        }
    }
}
