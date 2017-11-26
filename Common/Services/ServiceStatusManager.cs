using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Common.Services
{
	public class ServiceStatusManager : INotifyPropertyChanging, INotifyPropertyChanged, IDisposable
	{
		private static readonly ICollection<ServiceState> InvalidStates = new[] { ServiceState.None, };

		private static readonly IDictionary<ServiceState, ICollection<ServiceState>> AllowedStateChanges = new Dictionary<ServiceState, ICollection<ServiceState>>
		{
			{ 0, new[] { ServiceState.StartPending, } },
			{ ServiceState.StartPending, new[] { ServiceState.Running, ServiceState.StopPending, } },
		};

		private readonly IntPtr _serviceHandle;
		private ServiceStatus _serviceStatus;
		private ServiceState _previousState;
		private ServiceState _state;

		private ServiceState PreviousState
		{
			get => _previousState;
			set
			{
				if (_previousState == value) return;
				PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(PreviousState)));
				_previousState = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PreviousState)));
			}
		}

		public ServiceState State
		{
			get => _state;
			set
			{
				if (_state == value) return;
				PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(State)));
				_state = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
			}
		}

		/// <summary>
		/// Initialize a new instance of ServiceStatusManager
		/// </summary>
		/// <param name="serviceHandle">ServiceBase.ServiceHandle</param>
		public ServiceStatusManager(IntPtr serviceHandle)
		{
			_serviceHandle = serviceHandle;
			_serviceStatus = new ServiceStatus { dwCurrentState = 0, dwWaitHint = 100_000, };
			PropertyChanging += ServiceStatus_PropertyChanging;
			PropertyChanged += ServiceStatus_PropertyChanged;
		}

		#region INotifyPropertyChanging, INotifyPropertyChanged implementations
		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;

		private void ServiceStatus_PropertyChanging(object sender, PropertyChangingEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(State):
					PreviousState = State;
					break;
			}
		}

		private void ServiceStatus_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(State):
					if (!AllowedStateChanges[PreviousState].Contains(State))
					{
						throw new InvalidOperationException($"Cannot change from {PreviousState:G} to {State:G}")
						{
							Data = { { nameof(PreviousState), PreviousState }, { nameof(State), State }, },
						};
					}
					SetServiceStatus(State);
					break;
			}
		}
		#endregion INotifyPropertyChanging, INotifyPropertyChanged implementations

		#region SetServiceStatus
		private void SetServiceStatus(ServiceState serviceState)
		{
			foreach (var invalidServiceState in InvalidStates)
			{
				if (serviceState == invalidServiceState)
				{
					throw new ArgumentOutOfRangeException(nameof(serviceState), serviceState, $"{nameof(serviceState)} cannot be {invalidServiceState}")
					{
						Data = { { nameof(serviceState), serviceState }, },
					};
				}
			}

			_serviceStatus.dwCurrentState = serviceState;
			SetServiceStatus(_serviceHandle, ref _serviceStatus);
		}

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
		#endregion SetServiceStatus

		#region IDisposable implementation
		public void Dispose()
		{
			PropertyChanging -= ServiceStatus_PropertyChanging;
			PropertyChanged -= ServiceStatus_PropertyChanged;
		}
		#endregion IDisposable implementation
	}
}
