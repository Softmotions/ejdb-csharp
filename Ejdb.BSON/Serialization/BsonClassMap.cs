using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Ejdb.BSON
{
	public class BsonClassMap
	{
		private readonly Dictionary<string, BsonMemberMap> _allMembersDictionary;
		private Func<object> _creator;

        public BsonClassMap(Type type)
        {
            ClassType = type;
			_allMembersDictionary = _AutoMap();
        }

		private Dictionary<string, BsonMemberMap> _AutoMap()
		{
			var dictionary = new Dictionary<string, BsonMemberMap>();

            foreach (var propertyInfo in ClassType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (propertyInfo.GetIndexParameters().Length != 0) // skip indexers
                    continue;

				var memberMap = new BsonMemberMap(propertyInfo);
	            dictionary.Add(memberMap.ElementName, memberMap);
            }

            return dictionary;
        }

        // public properties
        /// <summary>
        /// Gets all the member maps (including maps for inherited members).
        /// </summary>
        public Dictionary<string, BsonMemberMap> AllMemberMaps
        {
            get { return _allMembersDictionary; }
        }

		public Func<object> Creator
		{
			get
			{
				if (_creator == null)
					_creator = _GetCreator();

				return _creator;
			}
		}

		private Func<object> _GetCreator()
		{
			Expression body;
			var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var defaultConstructor = ClassType.GetConstructor(bindingFlags, null, new Type[0], null);
			if (defaultConstructor != null)
			{
				// lambdaExpression = () => (object) new TClass()
				body = Expression.New(defaultConstructor);
			}
			else
			{
				// lambdaExpression = () => FormatterServices.GetUninitializedObject(classType)
				var getUnitializedObjectMethodInfo = typeof (FormatterServices).GetMethod("GetUninitializedObject",
				                                                                          BindingFlags.Public | BindingFlags.Static);
				body = Expression.Call(getUnitializedObjectMethodInfo, Expression.Constant(ClassType));
			}
			var lambdaExpression = Expression.Lambda<Func<object>>(body);
			return lambdaExpression.Compile();
		}

		public Type ClassType { get; private set; }
    }
}