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

namespace Ejdb.BSON {

	[Serializable]
	public class BsonArray : BsonDocument {

		public override BsonType BSONType {
			get {
				return BsonType.ARRAY;
			}
		}

		public object this[int key] {
			get {
				return GetObjectValue(key.ToString());
			}
		}

		public BsonArray() 
        {
		}

        public BsonArray(Array objects)
        {
            for (int i = 0; i < objects.Length; i++)
                _SetValue(i, BsonValue.ValueOf(objects.GetValue(i)));
        }

		public void SetMaxKey(int idx)
		{
		    _SetValue(idx, BsonValue.GetMaxKey());
		}

		public void SetMinKey(int idx)
		{
            _SetValue(idx, BsonValue.GetMinKey());
		}

        private void _SetValue(int idx, BsonValue val)
        {
            Add(idx.ToString(), val);
        }

		protected override void CheckKey(string key) 
        {
			int idx;
			if (key == null || !int.TryParse(key, out idx) || idx < 0) {
				throw new InvalidBSONDataException(string.Format("Invalid array key: {0}", key));
			}
		}
	}
}

