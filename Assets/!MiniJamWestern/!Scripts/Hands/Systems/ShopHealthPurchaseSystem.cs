using System.Linq;
using BitterECS.Core;

public class ShopHealthPurchaseSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.Medium;

    private EcsFilter<TagPlayer, HealthComponent, GridComponent> _playerFilter;

    public void ProcessPurchase(ShopCard card)
    {
        var player = _playerFilter.First();
        ref var healthComponent = ref player.Get<HealthComponent>();
        var healthAmount = card.HealthAmount;

        if (card.Type == ShopCard.CardType.HEAL)
        {
            healthComponent.SetHealth(healthComponent.GetCurrentHealth() + healthAmount);
            player.AddFrame<PlayerUpdateHealthUIEvent>();
        }
        else if (card.Type == ShopCard.CardType.MAX_HEALTH)
        {
            healthComponent.SetMaxHealth(healthComponent.GetMaxHealth() + healthAmount);
            healthComponent.SetHealth(healthComponent.GetCurrentHealth() + healthAmount);
            player.AddFrame<PlayerUpdateHealthUIEvent>();
        }
    }
}
