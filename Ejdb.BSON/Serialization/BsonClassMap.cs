using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Ejdb.BSON
{
    public class BsonClassMap
    {
        private readonly ReadOnlyCollection<BsonMemberMap> _allMemberMapsReadonly;

        public BsonClassMap(Type type)
        {
            ClassType = type;
            _allMemberMapsReadonly = new ReadOnlyCollection<BsonMemberMap>(_AutoMap());
        }


        private List<BsonMemberMap> _AutoMap()
        {
            var list = new List<BsonMemberMap>();

            foreach (var propertyInfo in ClassType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (propertyInfo.GetIndexParameters().Length != 0) // skip indexers
                    continue;

                list.Add(new BsonMemberMap(propertyInfo));
            }

            return list;
        }

        // public properties
        /// <summary>
        /// Gets all the member maps (including maps for inherited members).
        /// </summary>
        public ReadOnlyCollection<BsonMemberMap> AllMemberMaps
        {
            get { return _allMemberMapsReadonly; }
        }


        public Type ClassType { get; private set; }
    }
}