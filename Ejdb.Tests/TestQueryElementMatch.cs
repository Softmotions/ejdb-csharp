using System.Linq;
using Ejdb.BSON;
using Ejdb.DB;
using NUnit.Framework;

namespace Ejdb.Tests
{
	[TestFixture]
	public class TestQueryElementMatch
	{
		private const string COLLECTION_NAME = "records";

		private EJDB _db;

		[TestFixtureSetUp]
		public void Setup()
		{
			_db = new EJDB("testdb1", EJDB.DEFAULT_OPEN_MODE | EJDB.JBOTRUNC);

			var city1 = new BsonDocument()
				.Add("number", 1)
				.Add("zipcode", 109)
				.Add("students", new[] {
						BsonDocument.ValueOf(new { name = "john", school = 102, age = 10 }),
						BsonDocument.ValueOf(new { name = "jess", school = 102, age = 11 }),
						BsonDocument.ValueOf(new { name = "jeff", school = 108, age = 15 })
					});

			var city2 = new BsonDocument()
				.Add("number", 2)
				.Add("zipcode", 110)
				.Add("students", new[] {
						BsonDocument.ValueOf(new { name = "ajax", school = 100, age = 7 }),
						BsonDocument.ValueOf(new { name = "achilles", school = 100, age = 8 }),
					});

			var city3 = new BsonDocument()
				.Add("number", 3)
				.Add("zipcode", 109)
				.Add("students", new[] {
						BsonDocument.ValueOf(new { name = "ajax", school = 100, age = 7 }),
						BsonDocument.ValueOf(new { name = "achilles", school = 100, age = 8 }),
					});

			var city4 = new BsonDocument()
				.Add("number", 4)
				.Add("zipcode", 109)
				.Add("students", new[] {
						BsonDocument.ValueOf(new { name = "barney", school = 102, age = 7 })
					});

			_db.Save(COLLECTION_NAME, city1, city2, city3, city4);
		}


		[Test]
		public void Example1()
		{
			var results = _db.Find(COLLECTION_NAME, Query.And(
				Query.EQ("zipcode", 109),
				Query.ElemMatch("students", Query.EQ("school", 102))
			)).Select(x => x.ToBsonDocument()).ToArray();

			Assert.That(results.Length, Is.EqualTo(3));

			// this is different from MongoDb

			Assert.That(_GetNumberOfStudents(results[0]), Is.EqualTo(3)); // should be 1
			Assert.That(_GetNumberOfStudents(results[1]), Is.EqualTo(2)); // should be 0
			Assert.That(_GetNumberOfStudents(results[2]), Is.EqualTo(1)); // should be 1
		}

		private int _GetNumberOfStudents(BsonDocument document)
		{
			var array = (object[])document["students"];
			return array.Length;
		}
	}
}