using System;
using BitterECS.Integration.Unity;

[Serializable]
public struct TagMoveForward : IActionAbility
{ }

public class TagMoveForwardProvider : ProviderEcs<TagMoveForward> { }
