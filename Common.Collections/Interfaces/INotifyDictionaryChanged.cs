namespace Common.Collections.Interfaces
{
	public interface INotifyDictionaryChanged<T, TN>
	{
		event NotifyDictionaryChangedEventHandler<T, TN> DictionaryChanged;
		
	}
}