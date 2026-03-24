using System.Linq;
using BitterECS.Core;

public class ShopHealthPurchaseSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<TagPlayer, HealthComponent, GridComponent> _playerFilter;

    public void ProcessPurchase(ShopCard card)
    {
        if (card.Type == ShopCard.CardType.HEAL)
        {
            var player = _playerFilter.First();

            ref var healthComponent = ref player.Get<HealthComponent>();

            var healthAmount = card.HealthAmount;

            healthComponent.SetHealth(healthComponent.GetCurrentHealth() + healthAmount);
            player.AddFrame<PlayerUpdateHealthUIEvent>();
        }
    }
}
