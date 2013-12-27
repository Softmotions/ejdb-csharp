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