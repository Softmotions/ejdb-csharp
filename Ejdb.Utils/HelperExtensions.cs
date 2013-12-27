// // Copyright © Anton Paar GmbH, 2004-2013
//  

using Ejdb.DB;

namespace Ejdb.Utils
{
    public static class HelperExtensions
    {
        public static EJDBQCursor Find<T>(this EJDB db, IQuery query)
        {
            var collection = _GetCollectionName<T>();
            return db.Find(collection, query);
        }

        public static bool Save<T>(this EJDB db, params T[] objects)
        {
            var collection = _GetCollectionName<T>();
            return db.Save(collection, objects);
        }

        private static string _GetCollectionName<T>()
        {
            return typeof (T).Name.ToLower();
        }
    }
}