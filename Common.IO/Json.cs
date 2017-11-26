using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Common.IO
{
	public static class Json
	{
		public static T Deserialize<T>(string jsonString)
		{
			if (string.IsNullOrWhiteSpace(jsonString))
			{
				throw new ArgumentNullException { Data = { { nameof(jsonString), jsonString } } };
			}

			return JsonConvert.DeserializeObject<T>(jsonString);
		}

		public static async Task<T> DeserializeAsync<T>(string jsonString)
		{
			if (string.IsNullOrWhiteSpace(jsonString))
			{
				throw new ArgumentNullException { Data = { { nameof(jsonString), jsonString } } };
			}

			return await Task.Factory.StartNew(() => Deserialize<T>(jsonString));
		}
	}
}
