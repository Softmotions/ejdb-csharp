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