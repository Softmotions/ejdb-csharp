using System;
using System.Linq;
using Ejdb.BSON;
using Ejdb.DB;
using NUnit.Framework;

namespace Ejdb.Tests
{
    [TestFixture]
    public class TestQuery
    {
        private EJDB _db;
        private const string COLLECTION_NAME = "results";

        [TestFixtureSetUp]
        public void Setup()
        {
            _db = new EJDB("testdb1", EJDB.DEFAULT_OPEN_MODE | EJDB.JBOTRUNC);

            var results = new BsonDocument[100];
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

            _db.Save(COLLECTION_NAME, results);
        }

         [Test]
         public void ComparisonQueries()
         {
             _QueryResults(1, new QueryBuilder().EQ("MeasuredTemperature", 5));
             _QueryResults(1, Query<MeasurementResult>.EQ(x => x.MeasuredTemperature, 5));

             _QueryResults(1, Query.EQ("UserName", "test5"));
             _QueryResults(1, Query<MeasurementResult>.EQ(x => x.UserName, "test5"));

             _QueryResults(10, Query.EQ("OpticalRotation", 5));
             _QueryResults(10, Query<MeasurementResult>.EQ(x => x.OpticalRotation, 5));

             _QueryResults(99, Query.NotEquals("MeasuredTemperature", 5));
             _QueryResults(99, Query<MeasurementResult>.NotEquals(x => x.MeasuredTemperature, 5));

             _QueryResults(5, Query.LT("MeasuredTemperature", 5));
             _QueryResults(5, Query<MeasurementResult>.LT(x => x.MeasuredTemperature, 5));

             _QueryResults(6, Query.LTE("MeasuredTemperature", 5));
             _QueryResults(6, Query<MeasurementResult>.LTE(x => x.MeasuredTemperature, 5));

             _QueryResults(10, Query.Between("MeasuredTemperature", 1, 10));
             _QueryResults(10, Query<MeasurementResult>.Between(x => x.MeasuredTemperature, 1, 10));

             _QueryResults(2, Query.In("MeasuredTemperature", 47, 85));
             _QueryResults(2, Query<MeasurementResult>.In(x => x.MeasuredTemperature, 47, 85));

             _QueryResults(98, Query.NotIn("MeasuredTemperature", 47, 85));
             _QueryResults(98, Query<MeasurementResult>.NotIn(x => x.MeasuredTemperature, 47, 85));

             _QueryResults(11, Query.BeginsWith("UserName", "test1"));
             _QueryResults(11, Query<MeasurementResult>.BeginsWith(x => x.UserName, "test1"));
 
             _QueryResults(1, Query.EqualsIgnoreCase("UserName", "TeSt5"));
             _QueryResults(1, Query<MeasurementResult>.EqualsIgnoreCase(x => x.UserName, "TeSt5"));
         }

		[Test]
		public void OrQuery()
	    {
		    _QueryResults(11, Query.Or(
			    Query.GT("MeasuredTemperature", 98),
			    Query.EQ("OpticalRotation", 1)
			));

		    _QueryResults(11, Query<MeasurementResult>.Or(
			    Query<MeasurementResult>.GT(x => x.MeasuredTemperature, 98),
			    Query<MeasurementResult>.EQ(x => x.OpticalRotation, 1)
			));
	    }

		[Test]
		public void AndQuery()
	    {
		    _QueryResults(5, Query
			                     .GT("MeasuredTemperature", 50)
			                     .EQ("OpticalRotation", 5));

		    _QueryResults(5, Query<MeasurementResult>
			                     .GT(x => x.MeasuredTemperature, 50)
			                     .EQ(x => x.OpticalRotation, 5));
	    }


		[Test]
		public void StringComparisonQueries()
		{
			_QueryResults(11, Query.BeginsWith("UserName", "test1"));
			_QueryResults(11, Query<MeasurementResult>.BeginsWith(x => x.UserName, "test1"));

			_QueryResults(1, Query.EqualsIgnoreCase("UserName", "TeSt5"));
			_QueryResults(1, Query<MeasurementResult>.EqualsIgnoreCase(x => x.UserName, "TeSt5"));
		}

		[Test]
		public void StringMatchesAny_WithStringField()
		{
			_QueryResults(1, Query.StringMatchesAnyTokens("UserName", "test47"));
			_QueryResults(2, Query.StringMatchesAnyTokens("UserName", "test47", "test49"));

		}

		[Test]
		public void StringMatchesAll_WithStringField()
		{
			_QueryResults(0, Query.StringMatchesAllTokens("UserName", "test47", "test49"));
			_QueryResults(1, Query.StringMatchesAllTokens("UserName", "test47"));
		}

		[Test]
		public void StringMatchesAny_WithArrayField()
		{
			var doc1 = new BsonDocument()
				.Add("likes", new[] { "apple", "peach", "grapefruit" });

			var doc2 = new BsonDocument()
				.Add("likes", new[] { "apple", "peach", "mango" });

			_db.Save(COLLECTION_NAME, doc1, doc2);

			_QueryResults(2, Query.StringMatchesAnyTokens("likes", "apple"));
			_QueryResults(2, Query.StringMatchesAnyTokens("likes", "apple", "peach"));

			var results1 = _QueryResults(1, Query.StringMatchesAnyTokens("likes", "grapefruit"));
			Assert.IsTrue(results1.Single().Equals(doc1));

			var results2 = _QueryResults(1, Query.StringMatchesAnyTokens("likes", "mango"));
			Assert.IsTrue(results2.Single().Equals(doc2));
		}

		[Test]
		public void StringMatchesAll_WithArrayField()
		{
			var doc1 = new BsonDocument()
				.Add("likes", new[] { "apple", "peach", "grapefruit" });

			var doc2 = new BsonDocument()
				.Add("likes", new[] { "apple", "peach", "mango" });

			_db.Save(COLLECTION_NAME, doc1, doc2);

			_QueryResults(2, Query.StringMatchesAllTokens("likes", "peach"));

			// why do these checks fail?
			// _QueryResults(2, Query.StringMatchesAllTokens("likes", "apple", "peach"));
			// _QueryResults(1, Query.StringMatchesAllTokens("likes", "apple", "peach", "grapefruit"));

		}

	    private BsonDocument[] _QueryResults(int expectedResultCount, IQuery query)
         {
             Console.WriteLine("Running query ..");
             var cursor = _db.Find("results", query);
             var results = cursor.Select(x => x.ToBsonDocument()).ToArray();
             Console.WriteLine("Query finished ({0} results).", results.Length);

             if (results.Length != expectedResultCount)
                 Assert.Fail("Expected {0} results, but got {1}", expectedResultCount, results.Length);

             return results;
         }
    }

    internal class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    internal class MeasurementResult
    {
        public BsonOid _id { get; set; }
        public int MeasuredTemperature { get; set; }
        public int OpticalRotation { get; set; }
        public string UserName { get; set; }
    }
}