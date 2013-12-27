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
        public static IQuery EQ<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember value)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.EQ(fieldName, value);
        }

        public static IQuery EqualsIgnoreCase(Expression<Func<TDocument, string>> memberExpression, string value)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.EqualsIgnoreCase(fieldName, value);
        }

        public static IQuery BeginsWith(Expression<Func<TDocument, string>> memberExpression, string value)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.BeginsWith(fieldName, value);
        }

        public static IQuery EndsWith(Expression<Func<TDocument, string>> memberExpression, string value)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.EndsWith(fieldName, value);
        }

        public static IQuery GT<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember value)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.GT(fieldName, value);
        }

        public static IQuery GTE<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember value)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.GTE(fieldName, value);
        }

        public static IQuery LT<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember value)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.LT(fieldName, value);
        }

        public static IQuery LTE<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember value)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.LTE(fieldName, value);
        }

        public static IQuery In<TMember>(Expression<Func<TDocument, TMember>> memberExpression, params TMember[] comparisonValues)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.In(fieldName, comparisonValues);
        }

        public static IQuery NotIn<TMember>(Expression<Func<TDocument, TMember>> memberExpression, params TMember[] comparisonValues)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.NotIn(fieldName, comparisonValues);
        }

        public static IQuery NotEquals<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember value)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.NotEquals(fieldName, value);
        }

        public static IQuery Not<TMember>(Expression<Func<TDocument, TMember>> memberExpression, PartialQuery<TMember> query) 
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.Not(fieldName, query);
        }

        public static IQuery Between<TMember>(Expression<Func<TDocument, TMember>> memberExpression, TMember comparisonValue1, TMember comparisonValue2)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.Between(fieldName, comparisonValue1, comparisonValue2);
        }

        public static IQuery Exists<TMember>(Expression<Func<TDocument, TMember>> memberExpression)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.Exists(fieldName);
        }

        public static IQuery NotExists<TMember>(Expression<Func<TDocument, TMember>> memberExpression)
        {
            var fieldName = _GetFieldName(memberExpression);
            return Query.NotExists(fieldName);
        }

        private static string _GetFieldName<TMember>(Expression<Func<TDocument, TMember>> memberExpression)
        {
            return DynamicReflectionHelper.GetProperty(memberExpression).Name;
        }
    }
}