using BitterECS.Integration.Unity;
using UnityEngine;

public struct TagEnemy
{ }

[RequireComponent(typeof(FacingComponentProvider))]
[RequireComponent(typeof(EnemyStateProvider))]
public class TagEnemyProvider : ProviderEcs<TagEnemy>
{ }
