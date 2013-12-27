// ============================================================================================
//   .NET API for EJDB database library http://ejdb.org
//   Copyright (C) 2012-2013 Softmotions Ltd <info@softmotions.com>
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
using System.Collections;
using System.Globalization;

namespace Ejdb.BSON {

	[Serializable]
	public class BsonArray : BsonDocument {

		
		public BsonArray() 
        {
		}

        public BsonArray(BsonDocument document)
        {
            var iterator = new BsonIterator(document);
            while (iterator.Next() != BsonType.EOO)
                Add(iterator.CurrentKey, iterator.FetchCurrentValue());
        }

        public BsonArray(IEnumerable objects)
        {
            AddRange(objects);
        }

        public override BsonType BSONType
        {
            get
            {
                return BsonType.ARRAY;
            }
        }

        public object this[int key]
        {
            get
            {
                return GetObjectValue(key.ToString());
            }
        }

		public void SetMaxKey(int idx)
		{
		    Add(BsonValue.GetMaxKey());
		}

		public void SetMinKey(int idx)
		{
            Add(BsonValue.GetMinKey());
		}

        /// <summary>
        /// Adds multiple elements to the array.
        /// </summary>
        /// <param name="values">A list of values to add to the array.</param>
        /// <returns>The array (so method calls can be chained).</returns>
        public BsonArray AddRange(IEnumerable values)
        {
            if (values != null)
            {
                foreach (var value in values)
                    Add(BsonValue.ValueOf(value));
            }

            return this;
        }

        /// <summary>
        /// Adds an element to the array.
        /// </summary>
        /// <param name="value">The value to add to the array.</param>
        /// <returns>The array (so method calls can be chained).</returns>
        public BsonArray Add(BsonValue value)
        {
            if (value != null)
            {
                var key = Keys.Count.ToString(CultureInfo.InvariantCulture);
                Add(key, value);
            }

            return this;
        }

        public BsonArray Add(object value)
        {
            if (value != null)
            {
                var bsonValue = BsonValue.ValueOf(value);
                return Add(bsonValue);
            }

            return this;
        }

	    public int Count
        {
            get { return KeysCount; }
        }
	}
}

