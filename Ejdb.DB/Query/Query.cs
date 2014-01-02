// ============================================================================================
//   .NET API for EJDB database library http://ejdb.org
//   Copyright (C) 2013-2014 Oliver Klemencic <oliver.klemencic@gmail.com>
//
//   This file is part of EJDB.
//   EJDB is free software; you can redistribute it and/or modify it under the terms of
//   the GNU Lesser General Public License as published by the Free Software Foundation; either
//   version 2.1 of the License or any later version.  EJDB is distributed in the hope
//   that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
//   License for more details.
//   You should have received a copy of the GNU Lesser General Public License along with EJDB;
//   if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330,
//   Boston, MA 02111-1307 USA.
// ============================================================================================

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

	    public static IQuery Empty
	    {
		    get { return EmptyQuery.Instance; }
	    }

	    public static QueryBuilder EQ(string fieldName, object value)
	    {
		    return new QueryBuilder().EQ(fieldName, value);
	    }

		public static QueryBuilder EqualsIgnoreCase(string fieldName, string value)
		{
			return new QueryBuilder().EqualsIgnoreCase(fieldName, value);
        }

		public static IQuery BeginsWith(string fieldName, string value)
		{
			return new QueryBuilder().BeginsWith(fieldName, value);
		}

		public static IQuery EndsWith(string fieldName, string value)
		{
			return new QueryBuilder().EndsWith(fieldName, value);
        }

		public static QueryBuilder GT(string fieldName, object value)
		{
			return new QueryBuilder().GT(fieldName, value);
		}

		public static QueryBuilder GTE(string fieldName, object value)
		{
			return new QueryBuilder().GTE(fieldName, value);
		}

		public static QueryBuilder LT(string fieldName, object value)
        {
			return new QueryBuilder().LT(fieldName, value);
        }

		public static QueryBuilder LTE(string fieldName, object value)
        {
			return new QueryBuilder().LTE(fieldName, value);
        }

        public static QueryBuilder In<T>(string fieldName, params T[] comparisonValues)
		{
			return new QueryBuilder().In(fieldName, comparisonValues);
        }

		public static QueryBuilder NotIn<T>(string fieldName, params T[] comparisonValues)
		{
			return new QueryBuilder().NotIn(fieldName, comparisonValues);
        }

        public static QueryBuilder NotEquals(string fieldName, object comparisonValue)
		{
			return new QueryBuilder().NotEquals(fieldName, comparisonValue);
        }

        public static QueryBuilder Not(string fieldName, IPartialQuery query)
        {
			return new QueryBuilder().Not(fieldName, query);
        }

        public static QueryBuilder Between<T>(string fieldName, T comparisonValue1, T comparisonValue2)
        {
	        return new QueryBuilder().Between(fieldName, comparisonValue1, comparisonValue2);
        }

        /* public static IQuery And(params QueryBuilder[] queries)
        {
            return _CombinedQuery("$and", queries);
        } */

		public static IQuery Or(params QueryBuilder[] queries)
		{
			return new QueryBuilder().Or(queries);
		}

        public static IQuery Exists(string fieldName)
        {
	        return new QueryBuilder().Exists(fieldName);
        }

        public static IQuery NotExists(string fieldName)
        {
	        return new QueryBuilder().NotExists(fieldName);
        }

		public static IQuery StringMatchesAllTokens(string fieldName, params string[] values)
		{
			return new QueryBuilder().StringMatchesAllTokens(fieldName, values);
		}

		public static IQuery StringMatchesAnyTokens(string fieldName, params string[] values)
		{
			return new QueryBuilder().StringMatchesAnyTokens(fieldName, values);
		}


        public static IQuery ElemMatch(string fieldName, params IQuery[] queries)
        {
            var queryDocument = new BsonDocument();

            foreach (var query in queries)
            {
                foreach (var field in query.Document)
                    queryDocument[field.Key] = field.Value;
            }

            return new Query("$elemMatch", queryDocument);
        }

		public BsonDocument Document
		{
			get { return mQueryDocument; }
		}

		public static QueryBuilder Join(string fieldName, string foreignCollectionName)
		{
			return new QueryBuilder().Join(fieldName, foreignCollectionName);
		}
    }
}