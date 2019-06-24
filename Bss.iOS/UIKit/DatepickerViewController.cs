using System;
using UIKit;
using Foundation;
using System.Collections.Generic;

namespace Bss.iOS.UIKit
{

    public enum TitleFormat
    {
        /// <summary>
        /// Jan 2017
        /// </summary>
        Short,
        /// <summary>
        /// January 2017
        /// </summary>
        Long
    }

    public enum Theme
    {
        Light,
        Grey,
        BlueGrey,
        Dark,
        DeepOrange,
        Custom
    }

    public partial class DatepickerViewController : UIViewController
    {
        private const string ShortFormat = "MMM yyyy";
        private const string LongFormat = "MMMM yyyy";

        private static readonly Dictionary<Theme, UIColor> ThemeMap = new Dictionary<Theme, UIColor>
        {
            {Theme.Light,UIColor.White},
            {Theme.Grey,UIColor.FromRGB(189,189,189)},
            {Theme.BlueGrey,UIColor.FromRGB(22,28,31)},
            {Theme.DeepOrange,UIColor.FromRGB(100,34,13)},
            {Theme.Dark,UIColor.Black}
        };

        private Theme _theme = Theme.Light;

        private bool _wasInit;

        private UIDatePickerMode _mode = UIDatePickerMode.Date;

        private string _cancel = "Cancel";
        private string _confirm = "Confirm";

        private bool _blurred = true;

        private UIColor _themeColor = UIColor.White;
        private UIView[] _themeViews;

        private TitleFormat _titleFormat;

        public DatepickerViewController() : base("DatepickerViewController", null)
        {
            ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
            ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen;
        }

        public bool CloseOnTouchOutside { get; set; } = true;

        public Theme Theme
        {
            get { return _theme; }
            set
            {
                _theme = value;
                if (!_wasInit || _theme == Theme.Custom) return;
                _themeColor = ThemeMap[Theme];
                ChangeThemeColor();
            }
        }

        public UIDatePickerMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                if (!_wasInit) return;
                DatePicker.Mode = value;
            }
        }

        public TitleFormat TitleFormat
        {
            get { return _titleFormat; }
            set
            {
                _titleFormat = value;
                if (!_wasInit) return;
                UpdateDate();
            }
        }

        public bool Blured
        {
            get { return _blurred; }
            set
            {
                _blurred = value;
                if (!_wasInit) return;
                BlurView.Hidden = true;
            }
        }

        public DateTime Date { get; set; } = DateTime.Now;

        public UIColor ThemeColor
        {
            get { return _themeColor; }
            set
            {
                _themeColor = value;
                if (!_wasInit) return;
                _theme = Theme.Custom;
                ChangeThemeColor();
            }
        }

        public string CancelText
        {
            get { return _cancel; }
            set
            {
                _cancel = value;
                if (!_wasInit) return;
                CancelBtn.SetTitle(_cancel);
            }
        }

        public string ConfirmText
        {
            get { return _confirm; }
            set
            {
                _confirm = value;
                if (!_wasInit) return;
                ConfirmBtn.SetTitle(_confirm);
            }
        }

        #region Events

        public class DatePickEventArgs : EventArgs
        {
            public DatePickEventArgs(DateTime date)
            {
                Date = date;
            }

            public DateTime Date { get; }
        }

        public event EventHandler OnCancel;
        public event EventHandler<DatePickEventArgs> OnDatePick;

        #endregion

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            _wasInit = true;
            Initialize();
        }

        private void UpdateDate(bool animated = true)
        {
            MonthLbl.Text = Date.ToString(_titleFormat == TitleFormat.Short ?
                                                     ShortFormat : LongFormat);

            DatePicker.SetDate(Date.ToNsDate(), animated);
        }

        private void ChangeThemeColor()
        {
            foreach (var view in _themeViews)
            {
                var lbl = view as UILabel;
                if (lbl != null)
                {
                    lbl.TextColor = ThemeColor;
                    continue;
                }

                var btn = view as UIButton;
                if (btn != null)
                {
                    btn.SetTitleColor(ThemeColor, UIControlState.Normal);
                    continue;
                }
                view.BackgroundColor = ThemeColor;
            }
            ContentView.Layer.BorderColor = ThemeColor.CGColor;
            DatePicker.SetValueForKey(ThemeColor, new NSString("textColor"));
            DatePicker.Mode = UIDatePickerMode.CountDownTimer;
            DatePicker.Mode = Mode;
        }

        private void MakeBorder()
        {
            ContentView.Layer.CornerRadius = 10f;
            ContentView.SetBorder(ThemeColor);
        }

        private void Initialize()
        {
            _themeViews = new[] { TopView, PrevBtn, MonthLbl, NextBtn,
                BottomView, CancelBtn, SplitView, ConfirmBtn };

            BlurView.Hidden = !Blured;

            CancelBtn.SetTitle(_cancel);
            ConfirmBtn.SetTitle(_confirm);


            NextBtn.TouchUpInside += (sender, e) =>
            {
                Date = Date.AddMonths(1);
                UpdateDate();
            };

            PrevBtn.TouchUpInside += (sender, e) =>
            {
                Date = Date.AddMonths(-1);
                UpdateDate();
            };

            CancelBtn.TouchUpInside += (sender, e) => OnCancel?.Invoke(this, EventArgs.Empty);
            ConfirmBtn.TouchUpInside += (sender, e) => OnDatePick?.Invoke(this, new DatePickEventArgs(
                DatePicker.Date.ToDateTime()));

            View.OnClick(() =>
            {
                if (CloseOnTouchOutside)
                    OnCancel?.Invoke(this, EventArgs.Empty);
            });


            BlurView.Hidden = !Blured;

            MakeBorder();

            ChangeThemeColor();

            DatePicker.Mode = Mode;

            UpdateDate(false);

            DatePicker.ValueChanged += (sender, e) =>
           {
               Date = DatePicker.Date.ToDateTime();
               UpdateDate(false);
           };
        }
    }
}

