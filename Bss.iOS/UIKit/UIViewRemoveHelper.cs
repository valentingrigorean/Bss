using UIKit;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Bss.iOS.UIKit
{
	public class UIViewRemoveHelper : IDisposable
	{
		private readonly HashSet<Item> _container = new HashSet<Item>();

		private class Item
		{
			public bool IsValid => View != null && Superview != null;

			public UIView View { get; set; }

			public UIView Superview { get; set; }

			public IDictionary<UIView, NSLayoutConstraint[]> Constraints { get; set; }
		}

		public void Dispose()
		{
			_container.Clear();
		}

		public void AddView(UIView view)
		{
			var item = _container.FirstOrDefault(_ => _.View == view);
			if (item == null) 
                return;
			_container.Remove(item);
			if (item.IsValid)
			{
				item.Superview.Add(item.View);
				foreach (var pair in item.Constraints)
					pair.Key.AddConstraints(pair.Value.ToArray());
			}
		}

		public void RemoveView(UIView view)
		{
			var item = _container.FirstOrDefault(_ => _.View == view);
			if (item != null) 
                return;

			item = new Item
			{
				View = view,
				Superview = view.Superview,
				Constraints = view.GetAllConstraintWithParent() ?? new Dictionary<UIView, NSLayoutConstraint[]>()
			};
			_container.Add(item);
			view.RemoveFromSuperview();
		}
	}
}