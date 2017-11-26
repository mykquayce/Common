using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Common.Entities
{
	public abstract class EntityBase : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private States _state;

		protected States State
		{
			get => _state;
			private set
			{
				if (_state == value) return;
				NotifyPropertyChanging();
				_state = value;
				NotifyPropertyChanged();
			}
		}

		protected ObservableCollection<Exception> Exceptions { get; } = new ObservableCollection<Exception>();

		public event PropertyChangingEventHandler PropertyChanging;
		public event PropertyChangedEventHandler PropertyChanged;

		protected EntityBase()
		{
			PropertyChanged += EntityBase_PropertyChanged;
			Exceptions.CollectionChanged += Exceptions_CollectionChanged;
		}

		private void EntityBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(State):
					switch (State)
					{
						case States.Deserialized:
							ThrowExceptions();
							break;
					}
					break;
			}
		}

		private void Exceptions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					if (State == States.None
						|| State == States.Deserialized)
					{
						ThrowExceptions();
					}
					break;
			}
		}

		private void ThrowExceptions()
		{
			try
			{
				switch (Exceptions.Count)
				{
					case 0:
						break;
					case 1:
						throw Exceptions.Single();
					default:
						throw new AggregateException(Exceptions);
				}
			}
			finally
			{
				Exceptions.Clear();
			}
		}

		protected void NotifyPropertyChanging([CallerMemberName] string propertyName = null)
		{
			PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
		}

		protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		[OnDeserializing]
		public void OnDeserializing(StreamingContext context)
		{
			State = States.Deserializing;
		}

		[OnDeserialized]
		public void OnDeserialized(StreamingContext context)
		{
			State = States.Deserialized;
		}
	}

	[Flags]
	public enum States : byte
	{
		None = 0,
		Deserializing = 1,
		Deserialized = 2,
	}
}
