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

using System;
using System.Linq.Expressions;

namespace Ejdb.DB
{
    public static class Query<TDocument>
    {
		public static QueryBuilder<TDocument> EQ<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember value)
        {
			return new QueryBuilder<TDocument>().EQ(memberExpression, value);
        }

		public static QueryBuilder<TDocument> EqualsIgnoreCase(Expression<Func<TDocument, string>> memberExpression, string value)
        {
			return new QueryBuilder<TDocument>().EqualsIgnoreCase(memberExpression, value);
        }

		public static QueryBuilder<TDocument> BeginsWith(Expression<Func<TDocument, string>> memberExpression, string value)
        {
			return new QueryBuilder<TDocument>().BeginsWith(memberExpression, value);
        }

		public static QueryBuilder<TDocument> EndsWith(Expression<Func<TDocument, string>> memberExpression, string value)
        {
			return new QueryBuilder<TDocument>().EndsWith(memberExpression, value);
        }

		public static QueryBuilder<TDocument> GT<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember value)
        {
			return new QueryBuilder<TDocument>().GT(memberExpression, value);
        }

		public static QueryBuilder<TDocument> GTE<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember value)
        {
			return new QueryBuilder<TDocument>().GTE(memberExpression, value);
        }

		public static QueryBuilder<TDocument> LT<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember value)
        {
			return new QueryBuilder<TDocument>().LT(memberExpression, value);
        }

		public static QueryBuilder<TDocument> LTE<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember value)
        {
			return new QueryBuilder<TDocument>().LTE(memberExpression, value);
        }

		public static QueryBuilder<TDocument> In<TMember>(Expression<Func<TDocument, TMember>> memberExpression, params TMember[] comparisonValues)
        {
            return new QueryBuilder<TDocument>().In(memberExpression, comparisonValues);
        }

		public static QueryBuilder<TDocument> NotIn<TMember>(Expression<Func<TDocument, TMember>> memberExpression, params TMember[] comparisonValues)
        {
            return new QueryBuilder<TDocument>().NotIn(memberExpression, comparisonValues);
        }

		public static QueryBuilder<TDocument> NotEquals<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember value)
        {
            return new QueryBuilder<TDocument>().NotEquals(memberExpression, value);
        }

		public static QueryBuilder<TDocument> Not<TMember>(Expression<Func<TDocument, TMember>> memberExpression, PartialQuery<TMember> query) 
        {
            return new QueryBuilder<TDocument>().Not(memberExpression, query);
        }

        public static IQuery Between<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember comparisonValue1, TMember comparisonValue2)
        {
            return new QueryBuilder<TDocument>().Between(memberExpression, comparisonValue1, comparisonValue2);
        }

		public static QueryBuilder<TDocument> Exists<TMember>(Expression<Func<TDocument, TMember>> memberExpression)
        {
			return new QueryBuilder<TDocument>().Exists(memberExpression);
        }

		public static QueryBuilder<TDocument> NotExists<TMember>(Expression<Func<TDocument, TMember>> memberExpression)
        {
            return new QueryBuilder<TDocument>().NotExists(memberExpression);
        }

		public static QueryBuilder<TDocument> StringMatchesAllTokens(Expression<Func<TDocument, string>> memberExpression, params string[] values)
		{
			return new QueryBuilder<TDocument>().StringMatchesAllTokens(memberExpression, values);
		}

		public static QueryBuilder<TDocument> StringMatchesAnyTokens(Expression<Func<TDocument, string>> memberExpression, params string[] values)
		{
			return new QueryBuilder<TDocument>().StringMatchesAnyTokens(memberExpression, values);
		}

		public static QueryBuilder<TDocument> Or(params QueryBuilder<TDocument>[] queries)
		{
			return new QueryBuilder<TDocument>().Or(queries);
		}

		public static QueryBuilder<TDocument> ElemMatch<TProperty>(Expression<Func<TDocument, TProperty>> memberExpression, params IQuery[] queries)
		{
			return new QueryBuilder<TDocument>().ElemMatch(memberExpression, queries);
		}
    }
}