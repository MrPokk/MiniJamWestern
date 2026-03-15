public struct IsActionComponent
{
    public IActionAbility ability;

    public IsActionComponent(IActionAbility ability) => this.ability = ability;

    public readonly bool Is<T>() where T : IActionAbility => ability is T;

    public readonly bool Is<T>(out T result) where T : IActionAbility =>
    (result = ability is T t ? t : default) is not null || ability is T;
}
