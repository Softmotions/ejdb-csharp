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

namespace Ejdb.BSON {

	/// <summary>
	/// BSON field value.
	/// </summary>
	[Serializable]
    public sealed class BsonValue : IBsonValue, ICloneable
    {	

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

		public override bool Equals(object obj) 
		{
			if (obj == null) 
				return false;
			
			if (ReferenceEquals(this, obj)) 
				return true;
			
			if (obj.GetType() != typeof(BsonValue))
				return false;
			
			BsonValue other = (BsonValue) obj;
			if (BSONType != other.BSONType) 
				return false;
			
			if (Value != null) 
				return Value.Equals(other.Value);
			
			return (Value == other.Value);
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

		public static bool operator !=(BsonValue v1, BsonValue v2) 
		{
			return !(v1 == v2);
		}

		public override int GetHashCode() 
		{
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

		public object Clone() 
		{
			return new BsonValue(this.BSONType, this.Value);
		}

		  public static BsonValue ValueOf(object v)
		  {
			if (v == null)
				return GetNull();

			var arrayValue = v as Array;
			if (arrayValue != null)
				return Create(new BsonArray(arrayValue));

			Func<object, BsonValue> setter;
			var vtype = v.GetType();
			TYPE_SETTERS.TryGetValue(vtype, out setter);

			if (setter == null)
				setter = GetValueForCustomClassObject;

			var bsonValue = setter(v);
			return bsonValue;
		  }

		public static BsonDocument GetDocumentForCustomClassObject(object val)
		{
			var type = val.GetType();
			if ((type.IsClass || (type.IsValueType && !type.IsPrimitive)) &&
				!typeof (Array).IsAssignableFrom(type) &&
				!typeof (Enum).IsAssignableFrom(type))
			{
				var bsonDocument = new BsonDocument();
				var classMap = BsonClassSerialization.LookupClassMap(type);
				foreach (var member in classMap.AllMemberMaps)
				{
					var value = member.Getter(val);
					bsonDocument.Add(member.ElementName, value);
				}

				return bsonDocument;
			}

			throw new Exception(string.Format("Type '{0}' not supported for custom class serialization", type.FullName));
		}

		public static BsonValue GetValueForCustomClassObject(object val)
		{
			return Create(GetDocumentForCustomClassObject(val));
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

		public static BsonValue Create(BsonOid oid)
		{
			return new BsonValue(BsonType.OID, oid);
		}

		public static BsonValue Create(bool val)
		{
			return new BsonValue(BsonType.BOOL, val);
		}

		public static BsonValue Create(int val)
		{
			return new BsonValue(BsonType.INT, val);
		}

		public static BsonValue Create(long val)
		{
			return new BsonValue(BsonType.LONG, val);
		}

		public static BsonValue Create(double val)
		{
			return new BsonValue(BsonType.DOUBLE, val);
		}

		public static BsonValue Create(float val)
		{
			return new BsonValue(BsonType.DOUBLE, val);
		}

		public static BsonValue Create(string val)
		{
			return new BsonValue(BsonType.STRING, val);
		}

		public static BsonValue CreateCode(string val)
		{
			return new BsonValue(BsonType.CODE, val);
		}

		public static BsonValue CreateSymbol(string val)
		{
			return new BsonValue(BsonType.SYMBOL, val);
		}

		public static BsonValue Create(DateTime val)
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

		public static BsonValue Create(BsonDocument val) 
		{
			return new BsonValue(BsonType.OBJECT, val);
		}

		public static BsonValue Create(BsonArray val)
		{
			return new BsonValue(BsonType.ARRAY, val);
		}

		public static BsonValue Create(BsonTimestamp val)
		{
			return new BsonValue(BsonType.TIMESTAMP, val);
		}

		public static BsonValue Create(BsonCodeWScope val)
		{
			return new BsonValue(BsonType.CODEWSCOPE, val);
		}

		private static Dictionary<Type, Func<object, BsonValue>> TYPE_SETTERS =  new Dictionary<Type, Func<object, BsonValue>> 
		{
			{typeof(bool), v => Create((bool) v)},
			{typeof(byte), v => Create((int) v)},
			{typeof(sbyte), v => Create((int) v)},
			{typeof(ushort), v => Create((int) v)},
			{typeof(short), v => Create((int) v)},
			{typeof(uint), v => Create((int) v)},
			{typeof(int), v => Create((int) v)},
			{typeof(ulong), v => Create((long) v)},
			{typeof(long), v => Create((long) v)},
			{typeof(float), v => Create((float) v)},
			{typeof(double), v => Create((double) v)},
			{typeof(char), v => Create(v.ToString())},
			{typeof(string), v => Create((string) v)},
			{typeof(BsonOid), v => Create((BsonOid) v)},
			{typeof(BsonRegexp), v => GetRegexp((BsonRegexp) v)},
			{typeof(BsonValue), v => (BsonValue) v},
			{typeof(BsonTimestamp), v => Create((BsonTimestamp) v)},
			{typeof(BsonCodeWScope), v => Create((BsonCodeWScope) v)},
			{typeof(BsonBinData), v => GetBinData((BsonBinData) v)},
			{typeof(BsonDocument), v => Create((BsonDocument) v)},
			{typeof(BsonArray), v => Create((BsonArray) v)},
			{typeof(DateTime), v => Create((DateTime) v)},
			{typeof(BsonUndefined), v => GetUndefined()},
			{typeof(BsonNull), v => GetNull() },
		};
	}
}

