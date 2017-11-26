using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Common.Collections.Tests
{
	[TestFixture]
	public class TestNotifyingCollection
	{
		[Test]
		public void TestNotifyingCollection_AddOneItem()
		{
			var added = new List<string>();
			var removed = new List<string>();

			var collection = new NotifyingCollection<string>();

			collection.CollectionChanged += (sender, args) =>
			{
				if (!(args.NewItems is null))
				{
					added.AddRange(args.NewItems.Cast<string>());
				}

				if (!(args.OldItems is null))
				{
					removed.AddRange(args.OldItems.Cast<string>());
				}
			};

			// Act
			collection.Add("test");

			// Assert
			Assert.AreEqual(1, added.Count);
			Assert.AreEqual("test", added[0]);
			Assert.AreEqual(0, removed.Count);
		}
	}
}
