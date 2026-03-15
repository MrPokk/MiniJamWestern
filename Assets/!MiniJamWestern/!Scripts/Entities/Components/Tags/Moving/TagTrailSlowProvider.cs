using System;
using BitterECS.Integration.Unity;

[Serializable]
public struct TagTrailSlow : IActionAbility
{ }

public class TagTrailSlowProvider : ProviderEcs<TagTrailSlow> { }
