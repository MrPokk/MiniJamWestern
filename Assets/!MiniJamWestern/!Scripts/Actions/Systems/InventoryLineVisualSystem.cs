using System;
using System.Collections.Generic;
using BitterECS.Core;
using UnityEngine;

public class InventoryLineVisualSystem : IEcsRunSystem
{
    public Priority Priority => Priority.Low; // Отрисовываем после движения слотов

    private EcsFilter<AbilityInventory, TagInventoryEffects> _effectsFilter;
    private EcsFilter<AbilityInventory, TagInventoryUsing> _usingFilter;

    private HashSet<int> _activeLineIds = new HashSet<int>();
    private HashSet<int> _previousLineIds = new HashSet<int>();

    public void Run()
    {
        _previousLineIds.Clear();
        foreach (var id in _activeLineIds) _previousLineIds.Add(id);
        _activeLineIds.Clear();

        var usingSlotsByLimit = new Dictionary<AbilityTypeLimit, List<AbilitySlotProvider>>();

        _usingFilter.For((EcsEntity e, ref AbilityInventory inv, ref TagInventoryUsing _) =>
        {
            if (inv.listSlot == null) return;
            foreach (var slot in inv.listSlot)
            {
                if (slot != null && slot.gameObject.activeSelf)
                {
                    // Получаем лимит слота
                    if (slot.Entity.TryGet<AbilitySlotLimitComponent>(out var limitComp))
                    {
                        var limit = limitComp.allowedTypes;
                        if (!usingSlotsByLimit.ContainsKey(limit))
                            usingSlotsByLimit[limit] = new List<AbilitySlotProvider>();

                        usingSlotsByLimit[limit].Add(slot);
                    }
                }
            }
        });

        _effectsFilter.For((EcsEntity e, ref AbilityInventory inv, ref TagInventoryEffects _) =>
        {
            if (inv.listSlot == null) return;

            var effectSlotsByLimit = new Dictionary<AbilityTypeLimit, List<AbilitySlotProvider>>();

            foreach (var slot in inv.listSlot)
            {
                if (slot != null && slot.gameObject.activeSelf)
                {
                    if (slot.Entity.TryGet<AbilitySlotLimitComponent>(out var limitComp))
                    {
                        var limit = limitComp.allowedTypes;
                        if (!effectSlotsByLimit.ContainsKey(limit))
                            effectSlotsByLimit[limit] = new List<AbilitySlotProvider>();

                        effectSlotsByLimit[limit].Add(slot);
                    }
                }
            }

            foreach (var kvp in effectSlotsByLimit)
            {
                var limitType = kvp.Key;
                var effectSlots = kvp.Value; // Слоты эффектов одного типа (уже отсортированы по порядку UI)
                var lineColor = GetColorForLimit(limitType); // Получаем цвет для типа

                for (int i = 0; i < effectSlots.Count; i++)
                {
                    Vector3 currentPos = effectSlots[i].transform.position;

                    if (i > 0)
                    {
                        Vector3 prevPos = effectSlots[i - 1].transform.position;

                        int lineId = HashCode.Combine(effectSlots[i].GetInstanceID(), effectSlots[i - 1].GetInstanceID());
                        _activeLineIds.Add(lineId);

                        DrawRectUtility.Instance?.DrawStaticLine(lineId, currentPos, prevPos, 3f, lineColor);
                    }
                    else
                    {
                        if (usingSlotsByLimit.TryGetValue(limitType, out var matchingUsingSlots))
                        {
                            foreach (var usingSlot in matchingUsingSlots)
                            {
                                Vector3 targetPos = usingSlot.transform.position;

                                int lineId = HashCode.Combine(effectSlots[i].GetInstanceID(), usingSlot.GetInstanceID());
                                _activeLineIds.Add(lineId);

                                DrawRectUtility.Instance?.DrawStaticLine(lineId, currentPos, targetPos, 3f, lineColor);
                            }
                        }
                    }
                }
            }
        });

        foreach (var id in _previousLineIds)
        {
            if (!_activeLineIds.Contains(id))
            {
                DrawRectUtility.Instance?.HideStaticLine(id);
            }
        }
    }

    private Color GetColorForLimit(AbilityTypeLimit _)
    {
        //if ((limit & AbilityTypeLimit.Attack) != 0) return Color.white;  // Светло-красный
        //if ((limit & AbilityTypeLimit.Move) != 0) return new Color(0.4f, 1f, 0.4f);    // Светло-зеленый
        //if ((limit & AbilityTypeLimit.Rotation) != 0) return new Color(1f, 1f, 0.4f);  // Светло-желтый
        ColorUtility.TryParseHtmlString("#6ab6e9", out var parseColor);
        return parseColor;
    }
}
