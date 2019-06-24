
using UIKit;

namespace Bss.iOS.Extensions
{
	public static class UINavigationBarExtensions
	{
		public static void SetTransparentNavigationBar(this UINavigationBar This)
		{
			This.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
			This.ShadowImage = new UIImage();
			This.Translucent = true;
		}
	}
}
