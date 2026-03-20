using System;
using BitterECS.Integration.Unity;

[Serializable]
public struct TagRotation : IActionAbility
{ }

public class TagRotationProvider : ProviderEcs<TagRotation> { }
