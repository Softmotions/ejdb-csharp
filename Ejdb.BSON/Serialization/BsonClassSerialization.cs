using System;
using System.Collections.Generic;
using System.Threading;

namespace Ejdb.BSON
{
    public class BsonClassSerialization
    {
        private static readonly Dictionary<Type, BsonClassMap> __classMaps = new Dictionary<Type, BsonClassMap>();
        private static readonly ReaderWriterLockSlim __configurationLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <summary>
        /// Looks up a class map (will AutoMap the class if no class map is registered).
        /// </summary>
        /// <param name="classType">The class type.</param>
        /// <returns>The class map.</returns>
        public static BsonClassMap LookupClassMap(Type classType)
        {
            if (classType == null)
            {
                throw new ArgumentNullException("classType");
            }

            __configurationLock.EnterReadLock();
            try
            {
                BsonClassMap classMap;
                if (__classMaps.TryGetValue(classType, out classMap))
                    return classMap;
            }
            finally
            {
                __configurationLock.ExitReadLock();
            }

            __configurationLock.EnterWriteLock();
            try
            {
                BsonClassMap classMap;
                if (!__classMaps.TryGetValue(classType, out classMap))
                {
                    // automatically create a classMap for classType and register it
                    classMap = new BsonClassMap(classType);
                    _RegisterClassMap(classMap);
                }

                return classMap;
            }
            finally
            {
                __configurationLock.ExitWriteLock();
            }
        }


        private static void _RegisterClassMap(BsonClassMap classMap)
        {
            if (classMap == null)
                throw new ArgumentNullException("classMap");

            __configurationLock.EnterWriteLock();
            try
            {
                __classMaps.Add(classMap.ClassType, classMap);
            }
            finally
            {
                __configurationLock.ExitWriteLock();
            }
        }
    }
}