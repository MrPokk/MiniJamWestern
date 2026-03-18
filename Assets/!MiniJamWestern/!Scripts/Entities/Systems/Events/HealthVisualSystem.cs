using BitterECS.Core;

public struct UpdateHealthUIEvent { }

public class HealthVisualSystem : IEcsAutoImplement
{
    public Priority Priority => Priority.Medium;

    private EcsEvent _ecsEvent = new EcsEvent()
        .SubscribeAny<UpdateHealthUIEvent, IsDamagedEvent>(added: OnUpdateUI);

    private static void OnUpdateUI(EcsEntity entity)
    {
        // Убеждаемся, что у сущности есть и здоровье, и дисплей
        if (!entity.Has<HealthComponent>() || !entity.Has<HealthDisplay>())
            return;

        var health = entity.Get<HealthComponent>();
        var display = entity.Get<HealthDisplay>();

        int currentHealth = health.GetCurrentHealth();

        // ВАЖНО: Замените GetMaxHealth() на ваш метод/переменную из HealthComponent
        // Если у вас нет максимального хп, и вы хотите просто выключать пустые сердечки, 
        // то используйте: int maxHealth = currentHealth;
        int maxHealth = (int)health.GetMaxHealth();

        for (int i = 0; i < display.listSlot.Count; i++)
        {
            var element = display.listSlot[i];

            if (element == null)
                continue;

            // Если индекс слота меньше максимального ХП — он должен быть включен
            if (i < maxHealth)
            {
                element.gameObject.SetActive(true);

                // Если индекс меньше текущего ХП — сердечко полное, иначе пустое
                if (i < currentHealth)
                {
                    element.icon.sprite = display.full;
                }
                else
                {
                    element.icon.sprite = display.empty;
                }
            }
            else
            {
                // Если индекс больше максимального ХП (лишние слоты) — выключаем объект полностью
                element.gameObject.SetActive(false);
            }
        }
    }
}
