using System;
using BitterECS.Integration.Unity;

[Serializable]
public struct TagRotation : IActionAbility, IRotationAbility
{ }

public class TagRotationProvider : ProviderEcs<TagRotation> { }
