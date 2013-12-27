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
	public sealed class BsonValue : IBsonValue, ICloneable {	

		/// <summary>
		/// BSON.Type
		/// </summary>
		public BsonType BSONType { get; internal set; }

		/// <summary>
		/// Deserialized BSON field value.
		/// </summary>
		public object Value { get; internal set; }

	    public BsonValue(BsonType type, object value) 
        {
			BSONType = type;
			Value = value;
		}

		public BsonValue(BsonType type) : this(type, null) {
		}

		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != typeof(BsonValue)) {
				return false;
			}
			BsonValue other = (BsonValue) obj;
			if (BSONType != other.BSONType) {
				return false;
			}
			if (Value != null) {
				return Value.Equals(other.Value);
			} else {
				return (Value == other.Value);
			}
		}

		public static bool operator ==(BsonValue v1, BsonValue v2) {
			if (ReferenceEquals(v1, v2)) {
				return true;
			}
			if ((object) v1 == null || (object) v2 == null) {
				return false;
			}
			return v1.Equals(v2);
		}

		public static bool operator !=(BsonValue v1, BsonValue v2) {
			return !(v1 == v2);
		}

		public override int GetHashCode() {
			unchecked {
				return BSONType.GetHashCode() ^ (Value != null ? Value.GetHashCode() : 0);
			}
		}

		public override string ToString() {
			return String.Format("[BsonValue: BsonType={0}, Value={1}]", BSONType, Value);
		}

        public string ToStringWithKey(string key)
        {
            return String.Format("[BsonValue: BsonType={0}, Key={1}, Value={2}]", BSONType, key, Value);
        }

		public object Clone() {
			return new BsonValue(this.BSONType, this.Value);
		}

        public static BsonValue ValueOf(object v)
        {
            if (v == null)
                return GetNull();

            var arrayValue = v as Array;
            if (arrayValue != null)
                return GetArray(new BsonArray(arrayValue));

            Func<object, BsonValue> setter;
            var vtype = v.GetType();
            TYPE_SETTERS.TryGetValue(vtype, out setter);

            if (setter == null)
                setter = GetAnonTypeValue;

            var bsonValue = setter(v);
            return bsonValue;
        }

	    public static BsonDocument GetAnonTypeDocument(object val)
	    {
	        var ndoc = new BsonDocument();
	        Type vtype = val.GetType();
	        foreach (PropertyInfo pi in vtype.GetProperties()) 
	        {
	            if (pi.CanRead) 
	                ndoc[pi.Name] = pi.GetValue(val, null);
	        }

	        return ndoc;
	    }

	    public static BsonValue GetAnonTypeValue(object val)
	    {
	        return GetDocument(GetAnonTypeDocument(val));
	    }

	    public static BsonValue GetNull() 
	    {
	        return new BsonValue(BsonType.NULL);
	    }

	    public static BsonValue GetUndefined()
	    {
	        return new BsonValue(BsonType.UNKNOWN);
	    }

	    public static BsonValue GetMaxKey()
	    {
	        return new BsonValue(BsonType.MAXKEY);
	    }

	    public static BsonValue GetMinKey()
	    {
	        return new BsonValue(BsonType.MINKEY);
	    }

	    public static BsonValue GetOID(string oid)
	    {
	        return new BsonValue(BsonType.OID, new BsonOid(oid));
	    }

	    public static BsonValue GetOID(BsonOid oid)
	    {
	        return new BsonValue(BsonType.OID, oid);
	    }

	    public static BsonValue GetBool(bool val)
	    {
	        return new BsonValue(BsonType.BOOL, val);
	    }

	    public static BsonValue GetNumber(int val)
	    {
	        return new BsonValue(BsonType.INT, val);
	    }

	    public static BsonValue GetNumber(long val)
	    {
	        return new BsonValue(BsonType.LONG, val);
	    }

	    public static BsonValue GetNumber(double val)
	    {
	        return new BsonValue(BsonType.DOUBLE, val);
	    }

	    public static BsonValue GetNumber(float val)
	    {
	        return new BsonValue(BsonType.DOUBLE, val);
	    }

	    public static BsonValue GetString(string val)
	    {
	        return new BsonValue(BsonType.STRING, val);
	    }

	    public static BsonValue GetCode(string val)
	    {
	        return new BsonValue(BsonType.CODE, val);
	    }

	    public static BsonValue GetSymbol(string val)
	    {
	        return new BsonValue(BsonType.SYMBOL, val);
	    }

	    public static BsonValue GetDate(DateTime val)
	    {
	        return new BsonValue(BsonType.DATE, val);
	    }

	    public static BsonValue GetRegexp(BsonRegexp val)
	    {
	        return new BsonValue(BsonType.REGEX, val);
	    }

	    public static BsonValue GetBinData(BsonBinData val)
	    {
	        return new BsonValue(BsonType.BINDATA, val);
	    }

	    public static BsonValue GetDocument(BsonDocument val) 
        {
	        return new BsonValue(BsonType.OBJECT, val);
	    }

	    public static BsonValue GetArray(BsonArray val)
	    {
	        return new BsonValue(BsonType.ARRAY, val);
	    }

	    public static BsonValue GetTimestamp(BsonTimestamp val)
	    {
	        return new BsonValue(BsonType.TIMESTAMP, val);
	    }

	    public static BsonValue GetCodeWScope(BsonCodeWScope val)
	    {
	        return new BsonValue(BsonType.CODEWSCOPE, val);
	    }

	    private static Dictionary<Type, Func<object, BsonValue>> TYPE_SETTERS =  new Dictionary<Type, Func<object, BsonValue>> 
	    {
	        {typeof(bool), v => GetBool((bool) v)},
	        {typeof(byte), v => GetNumber((int) v)},
	        {typeof(sbyte), v => GetNumber((int) v)},
	        {typeof(ushort), v => GetNumber((int) v)},
	        {typeof(short), v => GetNumber((int) v)},
	        {typeof(uint), v => GetNumber((int) v)},
	        {typeof(int), v => GetNumber((int) v)},
	        {typeof(ulong), v => GetNumber((long) v)},
	        {typeof(long), v => GetNumber((long) v)},
	        {typeof(float), v => GetNumber((float) v)},
	        {typeof(double), v => GetNumber((double) v)},
	        {typeof(char), v => GetString(v.ToString())},
	        {typeof(string), v => GetString((string) v)},
	        {typeof(BsonOid), v => GetOID((BsonOid) v)},
	        {typeof(BsonRegexp), v => GetRegexp((BsonRegexp) v)},
	        {typeof(BsonValue), v => (BsonValue) v},
	        {typeof(BsonTimestamp), v => GetTimestamp((BsonTimestamp) v)},
	        {typeof(BsonCodeWScope), v => GetCodeWScope((BsonCodeWScope) v)},
	        {typeof(BsonBinData), v => GetBinData((BsonBinData) v)},
	        {typeof(BsonDocument), v => GetDocument((BsonDocument) v)},
	        {typeof(BsonArray), v => GetArray((BsonArray) v)},
	        {typeof(DateTime), v => GetDate((DateTime) v)},
	        {typeof(BsonUndefined), v => GetUndefined()},
	        {typeof(BsonNull), v => GetNull() },
	    };
	}
}

