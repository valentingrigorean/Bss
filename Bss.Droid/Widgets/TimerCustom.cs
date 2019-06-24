using System;
using System.Timers;
namespace Bss.Droid.Widgets
{
	public class TimerCustom : Timer
	{
		double _interval;
		Action _action;

		public TimerCustom(double interval, Action action)
		{
			_interval = interval;
			_action = action;
			Initialize();
		}

		private void Initialize()
		{
			Interval = _interval;
			Enabled = true;
			Elapsed += (object sender, ElapsedEventArgs e) => _action();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			Enabled = false;
		}
	}
}
