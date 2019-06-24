using UIKit;

namespace Bss.iOS.Extensions
{
    public static class UISearchBarExtensions
    {
        public static void ClearBackground(this UISearchBar This)
        {
			This.Translucent = true;
            This.SetBackgroundImage(new UIImage(),UIBarPosition.Any,UIBarMetrics.Default);
            This.ScopeBarBackgroundImage = new UIImage();           
        }
    }
}
