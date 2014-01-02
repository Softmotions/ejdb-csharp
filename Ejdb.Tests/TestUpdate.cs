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

		private static readonly BsonOid _doc1Id = BsonOid.GenerateNewId();
		private static readonly BsonOid _doc2Id = BsonOid.GenerateNewId();

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			_db = new EJDB("testdb1", EJDB.DEFAULT_OPEN_MODE | EJDB.JBOTRUNC);
		}

		[SetUp]
		public void Setup()
		{
			_db.DropCollection(COLLECTION_NAME);

			var doc1 = new BsonDocument()
				.Add("x", 1)
				.Add("y", new[] { 1, 2, 3 })
				.Add("_id", _doc1Id);

			_db.Save(COLLECTION_NAME, doc1);

			var doc2 = new BsonDocument()
				.Add("x", 2)
				.Add("y", new[] { 2, 3, 4 })
				.Add("_id", _doc2Id);

			_db.Save(COLLECTION_NAME, doc2);
		}

		[Test]
		public void Set()
		{
			_Execute(Query.EQ("x", 1), Update.Set("x", 10), 1);
			_TestDocumentValues(10, 2);
		}

		[Test]
		public void UpdateSet_EmptyQueryDocument()
		{
			_Execute(Query.Empty, Update.Set("x", 10), 2);
			_TestDocumentValues(10, 10);
		}

		[Test]
		public void Set_NullQuery()
		{
			_Execute(null, Update.Set("x", 10), 2);
			_TestDocumentValues(10, 10);
		}

		[Test]
		public void Inc()
		{
			_Execute(Query.Empty, Update.Inc("x", 10), 2);
			_TestDocumentValues(11, 12);
		}

		[Test]
		public void DropAll()
		{
			_Execute(Query.Empty, Update.DropAll(), 2);
			Assert.That(_db.Find(COLLECTION_NAME, Query.Empty).Length, Is.EqualTo(0));
		}

		[Test]
		public void Pull()
		{
			_Execute(Query.Empty, Update.Pull("y", 1), 2);

			var doc1 = _GetDocument(_doc1Id);
			Assert.That(doc1["y"], Is.EqualTo(new[] { 2, 3}));

			var doc2 = _GetDocument(_doc2Id);
			Assert.That(doc2["y"], Is.EqualTo(new[] { 2, 3, 4 }));
		}

		[Test]
		public void PullAll()
		{
			_Execute(Query.Empty, Update.PullAll("y", 1, 3), 2);

			var doc1 = _GetDocument(_doc1Id);
			Assert.That(doc1["y"], Is.EqualTo(new[] { 2 }));

			var doc2 = _GetDocument(_doc2Id);
			Assert.That(doc2["y"], Is.EqualTo(new[] { 2, 4 }));
		}

		[Test]
		public void AddToSet()
		{
			_Execute(Query.Empty, Update.AddToSet("y", 4), 2);

			var doc1 = _GetDocument(_doc1Id);
			Assert.That(doc1["y"], Is.EqualTo(new[] { 1, 2, 3, 4 }));

			var doc2 = _GetDocument(_doc2Id);
			Assert.That(doc2["y"], Is.EqualTo(new[] { 2, 3, 4 }));
		}

		[Test]
		public void AddToSetAll()
		{
			_Execute(Query.Empty, Update.AddToSetAll("y", 4, 5), 2);

			var doc1 = _GetDocument(_doc1Id);
			Assert.That(doc1["y"], Is.EqualTo(new[] { 1, 2, 3, 4, 5 }));

			var doc2 = _GetDocument(_doc2Id);
			Assert.That(doc2["y"], Is.EqualTo(new[] { 2, 3, 4, 5 }));
		}

		private void _Execute(IQuery query, UpdateBuilder updateQuery, int expectedCountThatMatched)
		{
			var countMatched = _db.Update(COLLECTION_NAME, query, updateQuery);
			Assert.That(countMatched, Is.EqualTo(expectedCountThatMatched));
		}

		private BsonDocument _GetDocument(BsonOid id)
		{
			return _db.Load(COLLECTION_NAME, id).ToBsonDocument();
		}

		private void _TestDocumentValues(int expectedDoc1Value, int expectedDoc2Value)
		{
			var doc1 = _GetDocument(_doc1Id);
			Assert.That(doc1["x"], Is.EqualTo(expectedDoc1Value));

			var doc2 = _GetDocument(_doc2Id);
			Assert.That(doc2["x"], Is.EqualTo(expectedDoc2Value));
		}
		

	}
}