using System.Threading;
using Common.Timers;
using NUnit.Framework;

namespace Common.Tests.Timers
{
	[TestFixture]
	public class TestPausableTimer
	{
		[Test]
		public void TestPausableTimer_2EventsIn500ms()
		{
			var timers = new[]
			{
				new PausableTimer(200),
				new System.Timers.Timer(200),
			};

			foreach (var timer in timers)
			{
				var count = 0;

				using (timer)
				{
					timer.Elapsed += (sender, args) => count++;

					timer.Start();

					Thread.Sleep(500);

					timer.Stop();
				}

				Assert.AreEqual(2, count);
			}
		}

		[Test]
		public void TestPausableTimer_1EventPauseThen1Event()
		{
			var count = 0;

			using (var pausableTimer = new PausableTimer(200))
			{
				pausableTimer.Elapsed += (sender, args) => count++;

				pausableTimer.Start();

				Thread.Sleep(350);

				pausableTimer.Pause();

				Thread.Sleep(1_000);

				pausableTimer.Resume();

				Thread.Sleep(100);

				pausableTimer.Stop();
			}

			Assert.AreEqual(2, count);
		}

		[Test]
		public void TestPausableTimer_2EventsPause1EventPause1EventPause2Events()
		{
			var count = 0;

			using (var pausableTimer = new PausableTimer(100))
			{
				pausableTimer.Elapsed += (sender, args) => count++;

				pausableTimer.Start();

				Thread.Sleep(250);

				pausableTimer.Pause();

				Thread.Sleep(500);

				pausableTimer.Resume();

				Thread.Sleep(100);

				pausableTimer.Pause();

				Thread.Sleep(500);

				pausableTimer.Resume();

				Thread.Sleep(100);

				pausableTimer.Pause();

				Thread.Sleep(500);

				pausableTimer.Resume();

				Thread.Sleep(200);

				pausableTimer.Stop();
			}

			Assert.AreEqual(6, count);
		}

		[Test]
		public void TestPausableTimer_AutoResetFalse()
		{
			var timers = new[]
			{
				new PausableTimer(100) { AutoReset = false },
				new System.Timers.Timer(100) { AutoReset = false },
			};

			foreach (var timer in timers)
			{
				var count = 0;

				using (timer)
				{
					timer.Elapsed += (sender, args) => count++;

					timer.Start();

					Thread.Sleep(1_000);

					timer.Stop();
				}

				Assert.AreEqual(1, count);
			}
		}

		[Test]
		public void TestPausableTimer_AutoResetFalse2()
		{
			var timers = new[]
			{
				new PausableTimer(100) { AutoReset = false },
				new System.Timers.Timer(100) { AutoReset = false },
			};

			foreach (var timer in timers)
			{
				var count = 0;

				using (timer)
				{
					timer.Elapsed += (sender, args) => count++;

					timer.Start();

					Thread.Sleep(1_000);

					timer.Start();

					Thread.Sleep(1_000);

					timer.Stop();
				}

				Assert.AreEqual(2, count);
			}
		}

	}
}
