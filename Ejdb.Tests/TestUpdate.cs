using Ejdb.BSON;
using Ejdb.DB;
using NUnit.Framework;

namespace Ejdb.Tests
{
	[TestFixture]
	public class TestUpdate
	{
		private EJDB _db;
		private const string COLLECTION_NAME = "results";

		[SetUp]
		public void Setup()
		{
			_db = new EJDB("testdb1", EJDB.DEFAULT_OPEN_MODE | EJDB.JBOTRUNC);

			/* var results = new BsonDocument[100];
			for (int i = 0; i < results.Length; i++)
			{
				results[i] = BsonDocument.ValueOf(new MeasurementResult
				{
					_id = BsonOid.GenerateNewId(),
					MeasuredTemperature = i,
					OpticalRotation = i % 10,
					UserName = "test" + i,
				});
			}

			_db.Save(COLLECTION_NAME, results); */
		}

		[Test]
		public void UpdateSet()
		{
			var doc = new BsonDocument().Add("x", 1);
			_db.Save(COLLECTION_NAME, doc);
			var countMatched = _db.Update(COLLECTION_NAME, Query.EQ("x", 1), Update.Set("x", 2));
			Assert.That(countMatched, Is.EqualTo(1));

			doc = _db.Load(COLLECTION_NAME, (BsonOid) doc["_id"]).ToBsonDocument();
			Assert.AreEqual(2, doc["x"]);
		}

		[Test]
		public void TestUpdateEmptyQueryDocument()
		{
			var doc = new BsonDocument().Add("x", 1);
			_db.Save(COLLECTION_NAME, doc);
			var countMatched = _db.Update(COLLECTION_NAME, Query.Empty, Update.Set("x", 2));
			Assert.That(countMatched, Is.EqualTo(1));

			doc = _db.Load(COLLECTION_NAME, (BsonOid)doc["_id"]).ToBsonDocument();
			Assert.AreEqual(2, doc["x"]);
		}
	}
}