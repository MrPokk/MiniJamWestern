using System;
using BitterECS.Integration.Unity;

[Serializable]
public struct TagMoveForward : IActionAbility, IMoveAbility
{ }

public class TagMoveForwardProvider : ProviderEcs<TagMoveForward> { }
