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
using System.IO;

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

		public BsonArray() {
		}

        public BsonArray(BsonUndefined[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetUndefined(i);
			}
		}

		public BsonArray(BsonNull[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetNull(i);
			}
		}

	    public BsonArray(ushort[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BsonArray(uint[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BsonArray(ulong[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, (long) arr[i]);
	    }

	    public BsonArray(short[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BsonArray(string[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetString(i, arr[i]);
	    }

	    public BsonArray(int[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BsonArray(long[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BsonArray(float[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BsonArray(double[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BsonArray(bool[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetBool(i, arr[i]);
	    }

	    public BsonArray(BsonOid[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetOID(i, arr[i]);
	    }

	    public BsonArray(DateTime[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetDate(i, arr[i]);
	    }

	    public BsonArray(BsonDocument[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetObject(i, arr[i]);
	    }

	    public BsonArray(BsonArray[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetArray(i, arr[i]);
	    }

	    public BsonArray(BsonRegexp[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetRegexp(i, arr[i]);
	    }

	    public BsonArray(BsonTimestamp[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetTimestamp(i, arr[i]);
	    }

	    public BsonArray(BsonCodeWScope[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetCodeWScope(i, arr[i]);
	    }

	    public BsonArray(BsonBinData[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetBinData(i, arr[i]);
	    }

		public void SetNull(int idx)
		{
		    _SetValue(idx, BsonValue.GetNull());
		}

        public void SetUndefined(int idx)
		{
		    _SetValue(idx, BsonValue.GetUndefined());
		}

		public void SetMaxKey(int idx)
		{
		    _SetValue(idx, BsonValue.GetMaxKey());
		}

		public void SetMinKey(int idx)
		{
            _SetValue(idx, BsonValue.GetMinKey());
		}

	    public void SetOID(int idx, string oid)
	    {
            _SetValue(idx, BsonValue.GetOID(oid));
	    }

	    public void SetOID(int idx, BsonOid oid)
	    {
            _SetValue(idx, BsonValue.GetOID(oid));
	    }

	    public void SetBool(int idx, bool val)
	    {
	        _SetValue(idx, BsonValue.GetBool(val));
	    }

	    public void SetNumber(int idx, int val)
	    {
            _SetValue(idx, BsonValue.GetNumber(val));
	    }

	    public void SetNumber(int idx, long val)
	    {
            _SetValue(idx, BsonValue.GetNumber(val));
	    }

	    public void SetNumber(int idx, double val)
	    {
            _SetValue(idx, BsonValue.GetNumber(val));
	    }

	    public void SetNumber(int idx, float val)
	    {
            _SetValue(idx, BsonValue.GetNumber(val));
	    }

	    public void SetString(int idx, string val)
	    {
            _SetValue(idx, BsonValue.GetString(val));
	    }

	    public void SetCode(int idx, string val)
	    {
            _SetValue(idx, BsonValue.GetCode(val));
	    }

	    public void SetSymbol(int idx, string val)
	    {
            _SetValue(idx, BsonValue.GetSymbol(val));
	    }

	    public void SetDate(int idx, DateTime val)
	    {
            _SetValue(idx, BsonValue.GetDate(val));
	    }

		public void SetRegexp(int idx, BsonRegexp val) 
        {
            _SetValue(idx, BsonValue.GetRegexp(val));
		}

		public void SetBinData(int idx, BsonBinData val) 
        {
            _SetValue(idx, BsonValue.GetBinData(val));
		}

		public void SetObject(int idx, BsonDocument val) 
        {
            _SetValue(idx, BsonValue.GetDocument(val));
		}

		public void SetArray(int idx, BsonArray val) 
        {
            _SetValue(idx, BsonValue.GetArray(val));
		}

		public void SetTimestamp(int idx, BsonTimestamp val) 
        {
            _SetValue(idx, BsonValue.GetTimestamp(val));
		}

		public void SetCodeWScope(int idx, BsonCodeWScope val) 
        {
            _SetValue(idx, BsonValue.GetCodeWScope(val));
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

