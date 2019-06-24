using UIKit;
using System.Reactive.Linq;
using System;
using System.Reactive.Disposables;

namespace Rx.Extensions
{
    public class UITextFieldTextChangedEventArgs : EventArgs
    {
        public UITextFieldTextChangedEventArgs(string text)
        {
            Text = text;
        }
        public string Text { get; internal set; }
    }

    public static class UITextFieldExtensions
    {
        private static Lazy<UITextFieldTextChangedEventArgs> _lazyTextChangeArgs =
            new Lazy<UITextFieldTextChangedEventArgs>(() => new UITextFieldTextChangedEventArgs(""));

        /// <summary>
        /// Fires when text change.
        /// Will not work if your settings delegate to ShouldChangeCharacters
        /// </summary>
        /// <returns>The text change.</returns>
        /// <param name="This">This.</param>
        public static IObservable<UITextFieldTextChangedEventArgs> WhenTextChange(this UITextField This)
        {
            return Observable.Create<UITextFieldTextChangedEventArgs>(obs =>
                        {
                            var args = _lazyTextChangeArgs.Value;
                            args.Text = This.Text;

                            This.AddTarget(handler, UIControlEvent.EditingChanged);
                            return Disposable.Create(() => This.RemoveTarget(handler, UIControlEvent.EditingChanged));

                            void handler(object sender, EventArgs e)
                            {
                                args.Text = This.Text;
                                obs.OnNext(args);
                            }
                        });
        }

        public static IDisposable WhenTextChange(this UITextField This, Action<string> callback)
        {
            return This.WhenTextChange()
                       .Select(e => e.Text)
                       .Subscribe(callback);
        }

        public static IObservable<string> WhenReturnPress(this UITextField This, bool autoDismiss = true)
        {
            return Observable.Create<string>(obser =>
            {
                UITextFieldCondition handler = (arg) =>
                {
                    if (autoDismiss)
                        This.ResignFirstResponder();
                    obser.OnNext(This.Text);
                    return true;
                };
                This.ShouldReturn = handler;
                return Disposable.Create(() => This.ShouldReturn = null);
            });
        }
    }
}