using BitterECS.Core;

public struct IsAttackerTo
{
    public EcsEntity targetEntity;

    public IsAttackerTo(EcsEntity targetEntity) => this.targetEntity = targetEntity;
}
