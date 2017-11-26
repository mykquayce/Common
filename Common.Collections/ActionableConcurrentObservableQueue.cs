using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System.Timers;
using Common.Exceptions;

namespace Common.Collections
{
	public abstract class ActionableConcurrentObservableQueue<T> : ConcurrentObservableQueue<T>, IDisposable
	{
		#region Private members
		private bool _isDisposed = false; // To detect redundant calls
		private readonly object _lock = new object();
		private readonly int _maxBufferSize;
		private readonly Timer _timer;

		#endregion Private members

		#region Properties
		public Action<IEnumerable<T>> Action { get; set; }
		#endregion Properties

		#region Constructors / Destructor
		protected ActionableConcurrentObservableQueue(int maxBufferSize, double maxBufferTime, Action<IEnumerable<T>> action)
			: this(maxBufferSize, maxBufferTime)
		{
			Contract.Requires(action != null);

			Action = action;
		}

		protected ActionableConcurrentObservableQueue(int maxBufferSize, double maxBufferTime)
		{
			Contract.Requires(maxBufferSize > 0);
			Contract.Requires(maxBufferTime > 0);
			Contract.Requires(maxBufferTime <= int.MaxValue);

			_maxBufferSize = maxBufferSize;

			_timer = new Timer(maxBufferTime);
			_timer.Elapsed += PerformAction;

			CollectionChanged += ActionableConcurrentObservableQueue_CollectionChanged;
		}

		~ActionableConcurrentObservableQueue()
		{
			Dispose(disposing: false);
		}
		#endregion Constructors / Destructor

		#region Event Handlers
		private void ActionableConcurrentObservableQueue_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					if (!_timer.Enabled)
					{
						_timer.Start();
					}
					if (Count >= _maxBufferSize)
					{
						PerformAction();
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					break;
				default:
					throw new NotifyCollectionChangedActionNotRecognizedException(args.Action);
			}
		}

		private void PerformAction(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			PerformAction();
		}
		#endregion Event Handlers

		private void PerformAction()
		{
			_timer.Stop();

			var items = new List<T>();

			lock (_lock)
			{
				while (TryDequeue(out var item))
				{
					items.Add(item);
				}
			}

			Action(items);
		}

		#region IDisposable Support
		protected virtual void Dispose(bool disposing)
		{
			if (_isDisposed) return;

			if (disposing)
			{
				_timer?.Stop();
				CollectionChanged -= ActionableConcurrentObservableQueue_CollectionChanged;
				if (_timer != null) _timer.Elapsed -= PerformAction;
				_timer?.Dispose();
				PerformAction();
			}

			_isDisposed = true;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
