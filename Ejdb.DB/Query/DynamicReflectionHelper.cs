// // Copyright © Anton Paar GmbH, 2004-2013
//  

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Ejdb.DB
{
    public class DynamicReflectionHelper
    {
        /// <summary>
        /// </summary>
        public static PropertyInfo GetProperty(LambdaExpression expression)
        {
            var memberExpression = _GetMemberExpression(expression);
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                var message = string.Format("Could not find a property for expression {0}. Check that your are using a property and not a field", expression);
                throw new Exception(message);
            }

            return propertyInfo;
        }

        public static string GetMethodName<TProperty>(Expression<Func<TProperty>> expression)
        {
            var body = (MethodCallExpression) expression.Body;
            return body.Method.Name;
        }

        /// <summary>
        /// </summary>
        private static MemberExpression _GetMemberExpression(LambdaExpression expression)
        {
            MemberExpression memberExpression = null;

            if (expression.Body.NodeType == ExpressionType.Convert || expression.Body.NodeType == ExpressionType.TypeAs)
            {
                var body = (UnaryExpression) expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }

            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
                memberExpression = expression.Body as MemberExpression;

            return memberExpression;
        }
    }
}