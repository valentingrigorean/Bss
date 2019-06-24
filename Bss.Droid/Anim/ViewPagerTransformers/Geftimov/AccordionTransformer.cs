using Android.Views;

namespace Bss.Droid.Anim.ViewPagerTransformers.Geftimov
{
    public class AccordionTransformer : BaseTransformer
    {
        protected override void OnTransform(View view, float position)
        {
            view.PivotX = position < 0 ? 0 : view.Width;
            view.ScaleX = position < 0 ? 1f + position : 1f - position;
        }
    }
}