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

        public static IQuery Not(string fieldName, object comparisonValue)
        {
            return _BinaryQuery("$not", fieldName, comparisonValue);
        }

        /* public static IQuery Not(string fieldName, IQuery query)
        {
            var childValue = query.GetQueryDocument();
            return new Query("$not", childValue);
        } */

        public static IQuery Between<T>(string fieldName, T comparisonValue1, T comparisonValue2)
        {
            var comparisonValues = new T[] { comparisonValue1, comparisonValue2 };
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

        private static IQuery _CombinedQuery(string combinator, IQuery[] queries)
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