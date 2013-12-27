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

        public static IQuery GT(string fieldName, object value)
        {
            return BinaryQuery("$gt", fieldName, value);
        }

        public static IQuery GTE(string fieldName, object value)
        {
            return BinaryQuery("$gte", fieldName, value);
        }

        public static IQuery LT(string fieldName, object value)
        {
            return BinaryQuery("$lt", fieldName, value);
        }

        public static IQuery LTE(string fieldName, object value)
        {
            return BinaryQuery("$lte", fieldName, value);
        }

        public static IQuery In<T>(string fieldName, params T[] comparisonValues)
        {
            return BinaryQuery("$in", fieldName, comparisonValues);
        }

        public static IQuery Not(string fieldName, object comparisonValue)
        {
            return BinaryQuery("$not", fieldName, comparisonValue);
        }

        public static IQuery Between<T>(string fieldName, T comparisonValue1, T comparisonValue2)
        {
            var comparisonValues = new T[] { comparisonValue1, comparisonValue2 };
            return BinaryQuery("$bt", fieldName, comparisonValues);
        }

        public static IQuery BinaryQuery(string queryOperation, string fieldName, object comparisonValue)
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