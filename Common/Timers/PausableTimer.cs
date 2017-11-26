using System.Diagnostics;
using System.Timers;

namespace Common.Timers
{
	public class PausableTimer : Timer
	{
		private double? _interval;
		private readonly Stopwatch _stopwatch = new Stopwatch();

		public PausableTimer()
			: base()
		{
			base.Elapsed += PausableTimer_Elapsed;
		}

		public PausableTimer(double interval)
			: base(interval)
		{
			base.Elapsed += PausableTimer_Elapsed;
		}

		public new void Dispose(bool disposing = false)
		{
			_stopwatch.Stop();
			base.Elapsed -= PausableTimer_Elapsed;
			base.Dispose(disposing);
		}

		private void PausableTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (base.AutoReset)
			{
				if (_interval.HasValue)
				{
					base.Interval = _interval.Value;
				}

				_stopwatch.Restart();
			}
			else
			{
				_stopwatch.Reset();
			}
		}

		public new void Start()
		{
			_interval = base.Interval;
			_stopwatch.Reset();
			_stopwatch.Start();
			base.Start();
		}

		public new void Stop()
		{
			_stopwatch.Stop();
			base.Stop();
		}

		public void Pause()
		{
			_stopwatch.Stop();
			base.Stop();
		}

		public void Resume()
		{
			_stopwatch.Start();
			base.Interval = base.Interval - _stopwatch.ElapsedMilliseconds;
			base.Start();
		}
	}
}
