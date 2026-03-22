using System;
using BitterECS.Integration.Unity;
using UnityEngine;

[Serializable]
public struct TagBehaviorMelee
{ }

[RequireComponent(typeof(TagEnemyProvider))]
public class TagBehaviorMeleeProvider : ProviderEcs<TagBehaviorMelee>
{ }
