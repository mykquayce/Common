using System;
using System.Collections.Specialized;

namespace Common.Exceptions
{
	public class NotifyCollectionChangedActionNotRecognizedException : Exception
	{
		public NotifyCollectionChangedActionNotRecognizedException(NotifyCollectionChangedAction action)
		{
			base.Data.Add(nameof(action), action);
		}
	}
}
