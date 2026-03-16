using UnityEngine;

public struct IsDraggingAbility { }

public struct PointerDownAbility
{
    public float pressTime;
}

public struct BeginDragAbilityEvent { }
public struct DragAbilityEvent { public Vector2 screenPosition; }
public struct PointerDownAbilityEvent { public float pressTime; }
public struct PointerUpAbilityEvent { }
public struct LongPressAbilityEvent { }
public struct ShortPressAbilityEvent { }
