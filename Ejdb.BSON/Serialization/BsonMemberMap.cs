using System;
using System.Reflection;
using System.Linq.Expressions;

namespace Ejdb.BSON
{
    public class BsonMemberMap
    {
		private Func<object, object> _getter;
		private Action<object, object> _setter;
		private readonly PropertyInfo _propertyInfo;

        // constructors
        /// <summary>
        /// Initializes a new instance of the BsonMemberMap class.
        /// </summary>
        /// <param name="propertyInfo">The member info.</param>
        public BsonMemberMap(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
            ElementName = propertyInfo.Name;
        }

		public bool IsWritable { get; private set; }

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
                    _getter = _GetPropertyGetter();

                return _getter;
            }
        }

		/// <summary>
		/// Gets the setter function.
		/// </summary>
		public Action<object, object> Setter
		{
			get
			{
				if (_setter == null)
					_setter = _GetPropertySetter();
				
				return _setter;
			}
		}
		

		/// <remarks>
		/// Readonly indicates that the member is written to the database, but not read from the database.
		/// </remarks>
		public bool IsReadOnly
		{
			get { return !_propertyInfo.CanWrite; }
		}

		private Action<object, object> _GetPropertySetter()
		{
			var setMethodInfo = _propertyInfo.GetSetMethod(true);
			if (IsReadOnly)
			{
				var message = string.Format("The property '{0} {1}' of class '{2}' has no 'set' accessor. ",
					_propertyInfo.PropertyType.FullName, _propertyInfo.Name, _propertyInfo.DeclaringType.FullName);
				throw new BsonSerializationException(message);
			}

			// lambdaExpression = (obj, value) => ((TClass) obj).SetMethod((TMember) value)
			var objParameter = Expression.Parameter(typeof(object), "obj");
			var valueParameter = Expression.Parameter(typeof(object), "value");
			var lambdaExpression = Expression.Lambda<Action<object, object>>(
				Expression.Call(
					Expression.Convert(objParameter, _propertyInfo.DeclaringType),
					setMethodInfo,
					Expression.Convert(valueParameter, _propertyInfo.PropertyType)
				),
				objParameter,
				valueParameter
			);

			return lambdaExpression.Compile();
		}

        private Func<object, object> _GetPropertyGetter()
        {
	        if (_propertyInfo != null)
            {
                var getMethodInfo = _propertyInfo.GetGetMethod(true);
                if (getMethodInfo == null)
                {
                    var message = string.Format(
                        "The property '{0} {1}' of class '{2}' has no 'get' accessor.",
                        _propertyInfo.PropertyType.FullName, _propertyInfo.Name, _propertyInfo.DeclaringType.FullName);
                    throw new BsonSerializationException(message);
                }
            }

            // lambdaExpression = (obj) => (object) ((TClass) obj).Member
            var objParameter = Expression.Parameter(typeof(object), "obj");
            var lambdaExpression = Expression.Lambda<Func<object, object>>(
                Expression.Convert(
                    Expression.MakeMemberAccess(
                        Expression.Convert(objParameter, _propertyInfo.DeclaringType),
                        _propertyInfo
                    ),
                    typeof(object)
                ),
                objParameter
            );

            return lambdaExpression.Compile();
        }
    }
}