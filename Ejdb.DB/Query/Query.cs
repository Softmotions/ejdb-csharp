// // Copyright © Anton Paar GmbH, 2004-2013
//  

using System.Linq;
using Ejdb.BSON;

namespace Ejdb.DB
{
    public class Query : IQuery
    {
        private readonly BsonDocument mQueryDocument;

        public Query(string fieldName, object query)
        {
            mQueryDocument = new BsonDocument();
            mQueryDocument[fieldName] = query;
        }

        public static IQuery EQ(string fieldName, object value)
        {
            return new Query(fieldName, value);
        }

        public static IQuery EqualsIgnoreCase(string fieldName, string value)
        {
            return _BinaryQuery("$icase", fieldName, value);
        }

        public static IQuery BeginsWith(string fieldName, string value)
        {
            return _BinaryQuery("$begin", fieldName, value);
        }

        public static IQuery EndsWith(string fieldName, string value)
        {
            return _BinaryQuery("end", fieldName, value);
        }

        public static IQuery GT(string fieldName, object value)
        {
            return _BinaryQuery("$gt", fieldName, value);
        }

        public static IQuery GTE(string fieldName, object value)
        {
            return _BinaryQuery("$gte", fieldName, value);
        }

        public static IQuery LT(string fieldName, object value)
        {
            return _BinaryQuery("$lt", fieldName, value);
        }

        public static IQuery LTE(string fieldName, object value)
        {
            return _BinaryQuery("$lte", fieldName, value);
        }

        public static IQuery In<T>(string fieldName, params T[] comparisonValues)
        {
            return _BinaryQuery("$in", fieldName, comparisonValues);
        }

        public static IQuery NotIn<T>(string fieldName, params T[] comparisonValues)
        {
            return _BinaryQuery("$nin", fieldName, comparisonValues);
        }

        public static IQuery NotEquals(string fieldName, object comparisonValue)
        {
            return _BinaryQuery("$not", fieldName, comparisonValue);
        }

        public static IQuery Not(string fieldName, IPartialQuery query)
        {
            var childValue = new BsonDocument();
            childValue.Add(query.QueryOperator, query.ComparisonValue);
            return _BinaryQuery("$not", fieldName, childValue);
        }

        public static IQuery Between<T>(string fieldName, T comparisonValue1, T comparisonValue2)
        {
            var comparisonValues = new[] { comparisonValue1, comparisonValue2 };
            return _BinaryQuery("$bt", fieldName, comparisonValues);
        }

        public static IQuery And(params IQuery[] queries)
        {
            return _CombinedQuery("$and", queries);
        }

        public static IQuery Or(params IQuery[] queries)
        {
            return _CombinedQuery("$or", queries);
        }

        public static IQuery Exists(string fieldName)
        {
            return _BinaryQuery("$exists", fieldName, true);
        }

        public static IQuery NotExists(string fieldName)
        {
            return _BinaryQuery("$exists", fieldName, false);
        }

        public static IQuery ElemMatch(string fieldName, params IQuery[] queries)
        {
            var queryDocument = new BsonDocument();

            foreach (var query in queries)
            {
                foreach (var field in query.GetQueryDocument())
                    queryDocument[field.Key] = field.Value;
            }

            return new Query("$elemMatch", queryDocument);
        }

        private static IQuery _CombinedQuery(string combinator, params IQuery[] queries)
        {
            var documents = queries.Select(x => x.GetQueryDocument()).ToArray();
            var childValue = new BsonArray(documents);
            return new Query(combinator, childValue);
        }

        private static IQuery _BinaryQuery(string queryOperation, string fieldName, object comparisonValue)
        {
            var query1 = new BsonDocument();
            query1[queryOperation] = comparisonValue;
            return new Query(fieldName, query1);
        }

        public BsonDocument GetQueryDocument()
        {
            return mQueryDocument;
        }
    }
}