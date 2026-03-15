using System;
using BitterECS.Integration.Unity;

[Serializable]
public struct TagMoveBehind : IActionAbility
{ }

public class TagMoveBehindProvider : ProviderEcs<TagMoveBehind> { }
