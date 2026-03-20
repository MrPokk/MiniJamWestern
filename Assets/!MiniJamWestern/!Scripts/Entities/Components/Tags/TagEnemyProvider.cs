using BitterECS.Integration.Unity;
using UnityEngine;

public struct TagEnemy
{ }

[RequireComponent(typeof(FacingComponentProvider))]
public class TagEnemyProvider : ProviderEcs<TagEnemy>
{ }
