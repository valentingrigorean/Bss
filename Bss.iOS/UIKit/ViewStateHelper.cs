using UIKit;

namespace Bss.iOS.UIKit
{
	public enum ViewState
	{
		Gone,
		Visible,
		Invisible
	}

	public class ViewStateHelper
	{
		private readonly UIViewRemoveHelper _removeHelper = new UIViewRemoveHelper();

		public void SetViewState(UIView view, ViewState viewState)
		{
			switch (viewState)
			{
				case ViewState.Visible:
                    if (view.Superview is UIStackView)
                    {
                        view.Hidden = false;
                        return;
                    }
					if (view.Superview == null)
						_removeHelper.AddView(view);
					view.Hidden = false;
					break;
				case ViewState.Invisible:
					view.Hidden = true;
					break;
				case ViewState.Gone:
					if (view.Superview == null)
                        return;
                    if (view.Superview is UIStackView)
                    {
                        view.Hidden = true;
                        return;
                    }
					_removeHelper.RemoveView(view);
					break;
			}
		}
	}
}
