using System;
using BitterECS.Integration.Unity;

[Serializable]
public struct TagPushForward : IActionAbility
{ }

public class TagPushForwardProvider : ProviderEcs<TagPushForward> { }
