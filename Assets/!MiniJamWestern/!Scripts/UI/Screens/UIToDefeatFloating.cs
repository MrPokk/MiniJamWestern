using UINotDependence.Core;

public class UIToDefeatFloating : UIScreen
{
    public override void Open()
    {
        UIAnimationComponent
        .Using(gameObject)
        .SetPresets(UIAnimationPresets.FadeIn,
                    UIAnimationPresets.FadeOut)
        .PlayOpen();

        base.Open();
    }
}
