using Android.Views;

namespace Bss.Droid.Anim.ViewPagerTransformers.Geftimov
{
    public class BackgroundToForegroundTransformer : BaseTransformer
    {
        protected override void OnTransform(View view, float position)
        {
            var height = view.Height;
            var width = view.Width;
            var scale = Min(position < 0 ? 1f : System.Math.Abs(1f - position), 0.5f);

            view.ScaleX = scale;
            view.ScaleY = scale;
            view.PivotX = width * 0.5f;
            view.PivotY = height * 0.5f;
            view.TranslationX = position < 0 ? width * position : -width * position * 0.25f;
        }

        private static float Min(float val, float min)
        {
            return val < min ? min : val;
        }

    }
}