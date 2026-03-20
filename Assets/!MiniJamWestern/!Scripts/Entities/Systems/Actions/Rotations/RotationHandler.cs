using BitterECS.Core;
using UnityEngine;

public static class RotationHandler
{
    public static void Execute(EcsEntity entity, GridComponent grid, ref TargetTo target)
    {
        // 1. Пытаемся понять, куда указывает текущий таргет относительно игрока
        if (!VectorUtility.TryGetStepDirection(grid.currentPosition, target.position, out var newDir))
        {
            // Если стоим на той же клетке, берем текущее направление
            newDir = entity.Has<FacingComponent>() ? entity.Get<FacingComponent>().direction : Vector2Int.up;
        }

        ref var facing = ref entity.GetOrAdd<FacingComponent>();

        // 2. ГЛАВНАЯ ЛОГИКА: Если мы уже смотрим в сторону newDir, значит разворачиваемся на 180 градусов
        if (facing.direction == newDir)
        {
            newDir = -newDir; // Инверсия (0, 1) станет (0, -1)
        }

        // 3. Сохраняем новое направление
        facing.direction = newDir;

        // 4. Обновляем позицию таргета, чтобы селектор (PlayerTargetingSystemY) переместился
        target.position = grid.currentPosition + newDir;
    }
}
