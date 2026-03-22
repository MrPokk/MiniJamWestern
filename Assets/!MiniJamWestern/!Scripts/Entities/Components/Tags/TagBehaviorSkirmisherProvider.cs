using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct TagBehaviorSkirmisher
{
    [ReadOnly] public SkirmisherPhase phase;
}

public enum SkirmisherPhase
{
    Approach,       // Подходит к игроку
    Attack,         // Бьет в ближнем бою
    Retreat,        // Отходит (разворачиваясь)
    FinalAttack     // Разворачивается и бьет еще раз
}

[RequireComponent(typeof(TagEnemyProvider))]
public class TagBehaviorSkirmisherProvider : ProviderEcs<TagBehaviorSkirmisher>
{ }
