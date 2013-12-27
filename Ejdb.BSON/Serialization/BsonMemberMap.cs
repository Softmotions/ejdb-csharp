using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Ejdb.BSON
{
    public class BsonMemberMap
    {
        private Func<object, object> _getter;
        private readonly MemberInfo _memberInfo;

        // constructors
        /// <summary>
        /// Initializes a new instance of the BsonMemberMap class.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        public BsonMemberMap(MemberInfo memberInfo)
        {
            _memberInfo = memberInfo;
            ElementName = memberInfo.Name;
        }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        public string ElementName { get; private set; }

        /// <summary>
        /// Gets the getter function.
        /// </summary>
        public Func<object, object> Getter
        {
            get
            {
                if (_getter == null)
                    _getter = GetGetter();

                return _getter;
            }
        }

        private Func<object, object> GetGetter()
        {
            var propertyInfo = _memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                var getMethodInfo = propertyInfo.GetGetMethod(true);
                if (getMethodInfo == null)
                {
                    var message = string.Format(
                        "The property '{0} {1}' of class '{2}' has no 'get' accessor.",
                        propertyInfo.PropertyType.FullName, propertyInfo.Name, propertyInfo.DeclaringType.FullName);
                    throw new BsonSerializationException(message);
                }
            }

            // lambdaExpression = (obj) => (object) ((TClass) obj).Member
            var objParameter = Expression.Parameter(typeof(object), "obj");
            var lambdaExpression = Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    Expression.MakeMemberAccess(
                        Expression.Convert(objParameter, _memberInfo.DeclaringType),
                        _memberInfo
                    ),
                    typeof(object)
                ),
                objParameter
            );

            return lambdaExpression.Compile();
        }
    }
}