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
	public class BSONArray : BSONDocument {

		public override BSONType BSONType {
			get {
				return BSONType.ARRAY;
			}
		}

		public object this[int key] {
			get {
				return GetObjectValue(key.ToString());
			}
		}

		public BSONArray() {
		}

        public BSONArray(BSONUndefined[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetUndefined(i);
			}
		}

		public BSONArray(BSONull[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetNull(i);
			}
		}

	    public BSONArray(ushort[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BSONArray(uint[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BSONArray(ulong[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, (long) arr[i]);
	    }

	    public BSONArray(short[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BSONArray(string[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetString(i, arr[i]);
	    }

	    public BSONArray(int[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BSONArray(long[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BSONArray(float[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BSONArray(double[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetNumber(i, arr[i]);
	    }

	    public BSONArray(bool[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetBool(i, arr[i]);
	    }

	    public BSONArray(BSONOid[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetOID(i, arr[i]);
	    }

	    public BSONArray(DateTime[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetDate(i, arr[i]);
	    }

	    public BSONArray(BSONDocument[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetObject(i, arr[i]);
	    }

	    public BSONArray(BSONArray[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetArray(i, arr[i]);
	    }

	    public BSONArray(BSONRegexp[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetRegexp(i, arr[i]);
	    }

	    public BSONArray(BSONTimestamp[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetTimestamp(i, arr[i]);
	    }

	    public BSONArray(BSONCodeWScope[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetCodeWScope(i, arr[i]);
	    }

	    public BSONArray(BSONBinData[] arr)
	    {
	        for (int i = 0; i < arr.Length; ++i)
	            SetBinData(i, arr[i]);
	    }

		public void SetNull(int idx)
		{
		    _SetValue(idx, BSONValue.GetNull());
		}

        public void SetUndefined(int idx)
		{
		    _SetValue(idx, BSONValue.GetUndefined());
		}

		public void SetMaxKey(int idx)
		{
		    _SetValue(idx, BSONValue.GetMaxKey());
		}

		public void SetMinKey(int idx)
		{
            _SetValue(idx, BSONValue.GetMinKey());
		}

	    public void SetOID(int idx, string oid)
	    {
            _SetValue(idx, BSONValue.GetOID(oid));
	    }

	    public void SetOID(int idx, BSONOid oid)
	    {
            _SetValue(idx, BSONValue.GetOID(oid));
	    }

	    public void SetBool(int idx, bool val)
	    {
	        _SetValue(idx, BSONValue.GetBool(val));
	    }

	    public void SetNumber(int idx, int val)
	    {
            _SetValue(idx, BSONValue.GetNumber(val));
	    }

	    public void SetNumber(int idx, long val)
	    {
            _SetValue(idx, BSONValue.GetNumber(val));
	    }

	    public void SetNumber(int idx, double val)
	    {
            _SetValue(idx, BSONValue.GetNumber(val));
	    }

	    public void SetNumber(int idx, float val)
	    {
            _SetValue(idx, BSONValue.GetNumber(val));
	    }

	    public void SetString(int idx, string val)
	    {
            _SetValue(idx, BSONValue.GetString(val));
	    }

	    public void SetCode(int idx, string val)
	    {
            _SetValue(idx, BSONValue.GetCode(val));
	    }

	    public void SetSymbol(int idx, string val)
	    {
            _SetValue(idx, BSONValue.GetSymbol(val));
	    }

	    public void SetDate(int idx, DateTime val)
	    {
            _SetValue(idx, BSONValue.GetDate(val));
	    }

		public void SetRegexp(int idx, BSONRegexp val) 
        {
            _SetValue(idx, BSONValue.GetRegexp(val));
		}

		public void SetBinData(int idx, BSONBinData val) 
        {
            _SetValue(idx, BSONValue.GetBinData(val));
		}

		public void SetObject(int idx, BSONDocument val) 
        {
            _SetValue(idx, BSONValue.GetDocument(val));
		}

		public void SetArray(int idx, BSONArray val) 
        {
            _SetValue(idx, BSONValue.GetArray(val));
		}

		public void SetTimestamp(int idx, BSONTimestamp val) 
        {
            _SetValue(idx, BSONValue.GetTimestamp(val));
		}

		public void SetCodeWScope(int idx, BSONCodeWScope val) 
        {
            _SetValue(idx, BSONValue.GetCodeWScope(val));
		}

        private void _SetValue(int idx, BSONValue val)
        {
            SetBSONValueNew(idx.ToString(), val);
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

