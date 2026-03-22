using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct TagBehaviorSkirmisher
{ }

[RequireComponent(typeof(TagEnemyProvider))]
public class TagBehaviorSkirmisherProvider : ProviderEcs<TagBehaviorSkirmisher>
{ }
