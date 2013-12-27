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