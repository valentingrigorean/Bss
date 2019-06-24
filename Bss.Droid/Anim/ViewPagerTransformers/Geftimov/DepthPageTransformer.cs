using Android.Views;

namespace Bss.Droid.Anim.ViewPagerTransformers.Geftimov
{
    class DepthPageTransformer : BaseTransformer
    {
        private const float MinScale = 0.75f;

        protected override bool IsPagingEnabled => true;


        protected override void OnTransform(View view, float position)
        {
            if (position <= 0)
            {
                view.TranslationX = 0f;
                view.ScaleX = 1f;
                view.ScaleY = 1f;
            }
            else if (position <= 1f)
            {
                var scaleFactor = MinScale + (1 - MinScale) * (1 - System.Math.Abs(position));
                view.Alpha = 1 - position;
                view.PivotY = 0.5f * view.Height;
                view.TranslationX = view.Width * -position;
                view.ScaleX = scaleFactor;
                view.ScaleY = scaleFactor;
            }
        }
    }
}