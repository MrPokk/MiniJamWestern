using System;
using BitterECS.Integration.Unity;
using UnityEngine;

public struct TagPlayer
{ }

[RequireComponent(typeof(FacingComponentProvider))]
public class TagPlayerProvider : ProviderEcs<TagPlayer>
{

}
