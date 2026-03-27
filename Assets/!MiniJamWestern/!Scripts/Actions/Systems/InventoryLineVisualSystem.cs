using System;
using System.Collections.Generic;
using BitterECS.Core;
using UnityEngine;

public class InventoryLineVisualSystem : IEcsRunSystem
{
    public Priority Priority => Priority.Low;

    private static readonly Color s_lineColor = ColorUtility.TryParseHtmlString("#6ab6e9", out var c) ? c : Color.cyan;
    private const float LineThickness = 3f;

    private EcsFilter<AbilityInventory, TagInventoryEffects> _effectsFilter;
    private EcsFilter<AbilityInventory, TagInventoryUsing> _usingFilter;

    private readonly HashSet<int> _activeLineIds = new();
    private readonly HashSet<int> _previousLineIds = new();
    private readonly Dictionary<AbilityTypeLimit, List<AbilitySlotProvider>> _usingSlotsByLimit = new();

    public void Run()
    {
        PrepareLineTracking();
        CollectUsingSlots();

        _effectsFilter.For((EcsEntity e, ref AbilityInventory inv, ref TagInventoryEffects _) =>
        {
            if (inv.listSlot == null) return;

            var effectGroups = GroupSlotsByLimit(inv.listSlot);
            DrawLinesForGroups(effectGroups);
        });

        CleanupOldLines();
    }

    private void PrepareLineTracking()
    {
        _previousLineIds.Clear();
        foreach (var id in _activeLineIds) _previousLineIds.Add(id);
        _activeLineIds.Clear();
    }

    private void CollectUsingSlots()
    {
        _usingSlotsByLimit.Clear();
        _usingFilter.For((EcsEntity e, ref AbilityInventory inv, ref TagInventoryUsing _) =>
        {
            if (inv.listSlot == null) return;
            PopulateGroupDictionary(inv.listSlot, _usingSlotsByLimit);
        });
    }

    private Dictionary<AbilityTypeLimit, List<AbilitySlotProvider>> GroupSlotsByLimit(List<AbilitySlotProvider> slots)
    {
        var group = new Dictionary<AbilityTypeLimit, List<AbilitySlotProvider>>();
        PopulateGroupDictionary(slots, group);
        return group;
    }

    private void PopulateGroupDictionary(List<AbilitySlotProvider> slots, Dictionary<AbilityTypeLimit, List<AbilitySlotProvider>> dictionary)
    {
        foreach (var slot in slots)
        {
            if (slot == null || !slot.gameObject.activeSelf) continue;
            if (!slot.Entity.TryGet<AbilitySlotLimitComponent>(out var limitComp)) continue;

            if (!dictionary.TryGetValue(limitComp.allowedTypes, out var list))
            {
                list = new List<AbilitySlotProvider>();
                dictionary[limitComp.allowedTypes] = list;
            }
            list.Add(slot);
        }
    }

    private void DrawLinesForGroups(Dictionary<AbilityTypeLimit, List<AbilitySlotProvider>> effectGroups)
    {
        foreach (var kvp in effectGroups)
        {
            var limitType = kvp.Key;
            var effectSlots = kvp.Value;

            for (var i = 0; i < effectSlots.Count; i++)
            {
                var currentSlot = effectSlots[i];

                if (i > 0)
                {
                    DrawConnection(currentSlot, effectSlots[i - 1]);
                }
                else
                {
                    DrawConnectionsToUsingSlots(currentSlot, limitType);
                }
            }
        }
    }

    private void DrawConnectionsToUsingSlots(AbilitySlotProvider effectSlot, AbilityTypeLimit limitType)
    {
        if (!_usingSlotsByLimit.TryGetValue(limitType, out var matchingUsingSlots)) return;

        foreach (var usingSlot in matchingUsingSlots)
        {
            DrawConnection(effectSlot, usingSlot);
        }
    }

    private void DrawConnection(AbilitySlotProvider a, AbilitySlotProvider b)
    {
        if (DrawRectUtility.Instance == null) return;

        int lineId = HashCode.Combine(a.GetInstanceID(), b.GetInstanceID());
        _activeLineIds.Add(lineId);

        DrawRectUtility.Instance.DrawStaticLine(lineId, a.transform.position, b.transform.position, LineThickness, s_lineColor);
    }

    private void CleanupOldLines()
    {
        if (DrawRectUtility.Instance == null) return;

        foreach (var id in _previousLineIds)
        {
            if (!_activeLineIds.Contains(id))
            {
                DrawRectUtility.Instance.HideStaticLine(id);
            }
        }
    }
}
