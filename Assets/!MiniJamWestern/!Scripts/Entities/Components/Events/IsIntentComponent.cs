using BitterECS.Core;
using UnityEngine;

public struct IsIntentComponent
{
    public IActionAbility chosenAbility;
    public Vector2Int targetPosition;
    public EcsEntity abilityEntity;
}
