using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Android.Views;
using Bss.Droid.Anim.Views;
using Rx.Core;
using Rx.Core.Views;

namespace Rx.Extensions
{
    public static class ViewExtensions
    {
        public static IObservable<Unit> WhenGlobalLayout(this View This)
        {
            return Observable.FromEventPattern(e => This.ViewTreeObserver.GlobalLayout += e,
                                               e => This.ViewTreeObserver.GlobalLayout -= e)
                             .Select(_ => Unit.Default);
        }

        public static IObservable<EventPattern<View.TouchEventArgs>> WhenTouch(this View This)
        {
            return Observable.FromEventPattern<View.TouchEventArgs>(e => This.Touch += e, e => This.Touch -= e);
        }

        public static IObservable<object> WhenClick(this View This)
        {
            This.Clickable = true;
            return Observable.FromEventPattern(e => This.Click += e, e => This.Click -= e);
        }

        public static IDisposable WhenClick(this View This, Action action)
        {
            return This.WhenClick().Subscribe(e => action?.Invoke());
        }

        public static IObservable<object> WhenLongClick(this View This)
        {
            return Observable.FromEventPattern<View.LongClickEventArgs>(e => This.LongClick += e, e => This.LongClick -= e);
        }

        public static IDisposable WhenTouch(this View This, Action action, IClickableView del = null)
        {
            del = del ?? new DefaultTouchEffect();
            bool valid = false;
            return This.WhenTouch()
                       .Subscribe(e =>
                       {
                           switch (e.EventArgs.Event.ActionMasked)
                           {
                               case MotionEventActions.Move:
                                   var ev = e.EventArgs.Event;
                                   if (ev.GetX() < 0 ||
                                       ev.GetY() < 0 ||
                                       ev.GetX() > This.MeasuredWidth ||
                                       ev.GetY() > This.MeasuredHeight)
                                   {
                                       valid = false;
                                       del.OnTouch(This, TState.Cancel, action);
                                   }
                                   break;
                               case MotionEventActions.Down:
                                   valid = true;
                                   del.OnTouch(This, TState.Began, action);
                                   break;
                               case MotionEventActions.Cancel:
                                   valid = false;
                                   del.OnTouch(This, TState.Cancel, action);
                                   break;
                               case MotionEventActions.Up:
                                   if (valid)
                                   {
                                       valid = false;
                                       del.OnTouch(This, TState.Ended, action);
                                   }
                                   break;
                           }
                       });
        }

        public static IList<ISupportRxSubscription>  FindAllRxViews(this View This)
        {
            var list = new List<ISupportRxSubscription>();

            if (This is ViewGroup)
                FindAllRxViews((ViewGroup)This);

			void FindAllRxViews(ViewGroup viewGrp)
			{
				if (viewGrp == null)
					return;
                if (viewGrp is ISupportRxSubscription rxView)
					list.Add(rxView);

				for (var i = 0; i < viewGrp.ChildCount; i++)
				{
					var view = viewGrp.GetChildAt(i);
					switch (view)
					{
						case ViewGroup grp:
							FindAllRxViews(grp);
							break;
                        case ISupportRxSubscription rxViewInternal:
							list.Add(rxViewInternal);
							break;
					}
				}
			}

            return list;
        }		
    }
}