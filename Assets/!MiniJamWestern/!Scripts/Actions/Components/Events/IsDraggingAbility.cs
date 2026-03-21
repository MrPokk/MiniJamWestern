using UnityEngine;

public struct IsDraggingAbility { }

public struct PointerDownAbility
{
    public float pressTime;
}

public struct PhysicsDragComponent
{
    public Vector3 targetWorldPosition;
    public Vector3 currentVelocity;
}

public struct DraggingFromSlotComponent
{
    public AbilitySlotProvider slotProvider;
}

public struct BeginDragAbilityEvent { }
public struct DragAbilityEvent { public Vector2 screenPosition; }
public struct PointerDownAbilityEvent { public float pressTime; }
public struct PointerUpAbilityEvent { }
public struct LongPressAbilityEvent { }
public struct ShortPressAbilityEvent { }
