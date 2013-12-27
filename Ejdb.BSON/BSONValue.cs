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
using System.Collections.Generic;
using System.Reflection;

namespace Ejdb.BSON {

	/// <summary>
	/// BSON field value.
	/// </summary>
	[Serializable]
	public sealed class BSONValue : IBSONValue, ICloneable {	

		/// <summary>
		/// BSON.Type
		/// </summary>
		public BSONType BSONType { get; internal set; }

		/// <summary>
		/// Deserialized BSON field value.
		/// </summary>
		public object Value { get; internal set; }

	    public BSONValue(BSONType type, object value) 
        {
			BSONType = type;
			Value = value;
		}

		public BSONValue(BSONType type) : this(type, null) {
		}

		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != typeof(BSONValue)) {
				return false;
			}
			BSONValue other = (BSONValue) obj;
			if (BSONType != other.BSONType) {
				return false;
			}
			if (Value != null) {
				return Value.Equals(other.Value);
			} else {
				return (Value == other.Value);
			}
		}

		public static bool operator ==(BSONValue v1, BSONValue v2) {
			if (ReferenceEquals(v1, v2)) {
				return true;
			}
			if ((object) v1 == null || (object) v2 == null) {
				return false;
			}
			return v1.Equals(v2);
		}

		public static bool operator !=(BSONValue v1, BSONValue v2) {
			return !(v1 == v2);
		}

		public override int GetHashCode() {
			unchecked {
				return BSONType.GetHashCode() ^ (Value != null ? Value.GetHashCode() : 0);
			}
		}

		public override string ToString() {
			return String.Format("[BSONValue: BSONType={0}, Value={1}]", BSONType, Value);
		}

        public string ToStringWithKey(string key)
        {
            return String.Format("[BSONValue: BSONType={0}, Key={1}, Value={2}]", BSONType, key, Value);
        }

		public object Clone() {
			return new BSONValue(this.BSONType, this.Value);
		}

        public static BSONValue ValueOf(object v)
        {
            if (v == null)
                return GetNull();

            Func<object, BSONValue> setter;
            var vtype = v.GetType();
            TYPE_SETTERS.TryGetValue(vtype, out setter);

            if (setter == null)
                setter = GetAnonTypeValue;

            var bsonValue = setter(v);
            return bsonValue;
        }

	    public static BSONDocument GetAnonTypeDocument(object val)
	    {
	        var ndoc = new BSONDocument();
	        Type vtype = val.GetType();
	        foreach (PropertyInfo pi in vtype.GetProperties()) 
	        {
	            if (pi.CanRead) 
	                ndoc[pi.Name] = pi.GetValue(val, null);
	        }

	        return ndoc;
	    }

	    public static BSONValue GetAnonTypeValue(object val)
	    {
	        return GetDocument(GetAnonTypeDocument(val));
	    }

	    public static BSONValue GetNull() 
	    {
	        return new BSONValue(BSONType.NULL);
	    }

	    public static BSONValue GetUndefined()
	    {
	        return new BSONValue(BSONType.UNKNOWN);
	    }

	    public static BSONValue GetMaxKey()
	    {
	        return new BSONValue(BSONType.MAXKEY);
	    }

	    public static BSONValue GetMinKey()
	    {
	        return new BSONValue(BSONType.MINKEY);
	    }

	    public static BSONValue GetOID(string oid)
	    {
	        return new BSONValue(BSONType.OID, new BSONOid(oid));
	    }

	    public static BSONValue GetOID(BSONOid oid)
	    {
	        return new BSONValue(BSONType.OID, oid);
	    }

	    public static BSONValue GetBool(bool val)
	    {
	        return new BSONValue(BSONType.BOOL, val);
	    }

	    public static BSONValue GetNumber(int val)
	    {
	        return new BSONValue(BSONType.INT, val);
	    }

	    public static BSONValue GetNumber(long val)
	    {
	        return new BSONValue(BSONType.LONG, val);
	    }

	    public static BSONValue GetNumber(double val)
	    {
	        return new BSONValue(BSONType.DOUBLE, val);
	    }

	    public static BSONValue GetNumber(float val)
	    {
	        return new BSONValue(BSONType.DOUBLE, val);
	    }

	    public static BSONValue GetString(string val)
	    {
	        return new BSONValue(BSONType.STRING, val);
	    }

	    public static BSONValue GetCode(string val)
	    {
	        return new BSONValue(BSONType.CODE, val);
	    }

	    public static BSONValue GetSymbol(string val)
	    {
	        return new BSONValue(BSONType.SYMBOL, val);
	    }

	    public static BSONValue GetDate(DateTime val)
	    {
	        return new BSONValue(BSONType.DATE, val);
	    }

	    public static BSONValue GetRegexp(BSONRegexp val)
	    {
	        return new BSONValue(BSONType.REGEX, val);
	    }

	    public static BSONValue GetBinData(BSONBinData val)
	    {
	        return new BSONValue(BSONType.BINDATA, val);
	    }

	    public static BSONValue GetDocument(BSONDocument val) 
        {
	        return new BSONValue(BSONType.OBJECT, val);
	    }

	    public static BSONValue GetArray(BSONArray val)
	    {
	        return new BSONValue(BSONType.ARRAY, val);
	    }

	    public static BSONValue GetTimestamp(BSONTimestamp val)
	    {
	        return new BSONValue(BSONType.TIMESTAMP, val);
	    }

	    public static BSONValue GetCodeWScope(BSONCodeWScope val)
	    {
	        return new BSONValue(BSONType.CODEWSCOPE, val);
	    }

	    public static Dictionary<Type, Func<object, BSONValue>> TYPE_SETTERS =  new Dictionary<Type, Func<object, BSONValue>> 
	    {
	        {typeof(bool), v => GetBool((bool) v)},
	        {typeof(bool[]), v => GetArray(new BSONArray((bool[]) v))},
	        {typeof(byte), v => GetNumber((int) v)},
	        {typeof(sbyte), v => GetNumber((int) v)},
	        {typeof(ushort), v => GetNumber((int) v)},
	        {typeof(ushort[]), v => GetArray(new BSONArray((ushort[]) v))},
	        {typeof(short), v => GetNumber((int) v)},
	        {typeof(short[]), v => GetArray(new BSONArray((short[]) v))},
	        {typeof(uint), v => GetNumber((int) v)},
	        {typeof(uint[]), v => GetArray(new BSONArray((uint[]) v))},
	        {typeof(int), v => GetNumber((int) v)},
	        {typeof(int[]), v => GetArray(new BSONArray((int[]) v))},
	        {typeof(ulong), v => GetNumber((long) v)},
	        {typeof(ulong[]), v => GetArray(new BSONArray((ulong[]) v))},
	        {typeof(long), v => GetNumber((long) v)},
	        {typeof(long[]), v => GetArray(new BSONArray((long[]) v))},
	        {typeof(float), v => GetNumber((float) v)},
	        {typeof(float[]), v => GetArray(new BSONArray((float[]) v))},
	        {typeof(double), v => GetNumber((double) v)},
	        {typeof(double[]), v => GetArray(new BSONArray((double[]) v))},
	        {typeof(char), v => GetString(v.ToString())},
	        {typeof(string), v => GetString((string) v)},
	        {typeof(string[]), v => GetArray(new BSONArray((string[]) v))},
	        {typeof(BSONOid), v => GetOID((BSONOid) v)},
	        {typeof(BSONOid[]), v => GetArray(new BSONArray((BSONOid[]) v))},
	        {typeof(BSONRegexp), v => GetRegexp((BSONRegexp) v)},
	        {typeof(BSONRegexp[]), v => GetArray(new BSONArray((BSONRegexp[]) v))},
	        {typeof(BSONValue), v => (BSONValue) v},
	        {typeof(BSONTimestamp), v => GetTimestamp((BSONTimestamp) v)},
	        {typeof(BSONTimestamp[]), v => GetArray(new BSONArray((BSONTimestamp[]) v))},
	        {typeof(BSONCodeWScope), v => GetCodeWScope((BSONCodeWScope) v)},
	        {typeof(BSONCodeWScope[]), v => GetArray(new BSONArray((BSONCodeWScope[]) v))},
	        {typeof(BSONBinData), v => GetBinData((BSONBinData) v)},
	        {typeof(BSONBinData[]), v => GetArray(new BSONArray((BSONBinData[]) v))},
	        {typeof(BSONDocument), v => GetDocument((BSONDocument) v)},
	        {typeof(BSONDocument[]), v => GetArray(new BSONArray((BSONDocument[]) v))},
	        {typeof(BSONArray), v => GetArray((BSONArray) v)},
	        {typeof(BSONArray[]), v => GetArray(new BSONArray((BSONArray[]) v))},
	        {typeof(DateTime), v => GetDate((DateTime) v)},
	        {typeof(DateTime[]), v => GetArray(new BSONArray((DateTime[]) v))},
	        {typeof(BSONUndefined), v => GetUndefined()},
	        {typeof(BSONUndefined[]), v => GetArray(new BSONArray((BSONUndefined[]) v))},
	        {typeof(BSONull), v => GetNull() },
	        {typeof(BSONull[]), v => GetArray(new BSONArray((BSONull[]) v))}
	    };
	}
}

