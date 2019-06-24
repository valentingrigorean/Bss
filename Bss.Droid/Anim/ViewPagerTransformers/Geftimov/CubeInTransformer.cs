using Android.Views;

namespace Bss.Droid.Anim.ViewPagerTransformers.Geftimov
{
    public class CubeInTransformer : BaseTransformer
    {

        protected override bool IsPagingEnabled => true;
       
        protected override void OnTransform(View view, float position)
        {
            view.PivotX = position > 0 ? 0 : view.Width;
            view.PivotY = 0;
            view.RotationY = -90f * position;
        }
    }
}