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
using System.IO;
using System.Text;
using System.Diagnostics;
using Ejdb.IO;
using Ejdb.SON;

namespace Ejdb.SON {

	/// <summary>
	/// BSON document deserialized data wrapper.
	/// </summary>
	[Serializable]
	public class BSONDocument : IBSONValue, IEnumerable<BSONValue>, ICloneable {
		static Dictionary<Type, Action<BSONDocument, string, object>> TYPE_SETTERS = 
		new Dictionary<Type, Action<BSONDocument, string, object>> {
			{typeof(bool), (d, k, v) => d.SetBool(k, (bool) v)},
			{typeof(byte), (d, k, v) => d.SetNumber(k, (int) v)},
			{typeof(sbyte), (d, k, v) => d.SetNumber(k, (int) v)},
			{typeof(ushort), (d, k, v) => d.SetNumber(k, (int) v)},
			{typeof(short), (d, k, v) => d.SetNumber(k, (int) v)},
			{typeof(uint), (d, k, v) => d.SetNumber(k, (int) v)},
			{typeof(int), (d, k, v) => d.SetNumber(k, (int) v)},
			{typeof(ulong), (d, k, v) => d.SetNumber(k, (long) v)},
			{typeof(long), (d, k, v) => d.SetNumber(k, (long) v)},
			{typeof(float), (d, k, v) => d.SetNumber(k, (float) v)},
			{typeof(double), (d, k, v) => d.SetNumber(k, (double) v)},
			{typeof(char), (d, k, v) => d.SetString(k, v.ToString())},
			{typeof(string), (d, k, v) => d.SetString(k, (string) v)},
			{typeof(BSONOid), (d, k, v) => d.SetOID(k, (BSONOid) v)},
			{typeof(BSONRegexp), (d, k, v) => d.SetRegexp(k, (BSONRegexp) v)},
			{typeof(BSONValue), (d, k, v) => d.SetBSONValue((BSONValue) v)},
			{typeof(BSONTimestamp), (d, k, v) => d.SetTimestamp(k, (BSONTimestamp) v)},
			{typeof(BSONCodeWScope), (d, k, v) => d.SetCodeWScope(k, (BSONCodeWScope) v)},
			{typeof(BSONBinData), (d, k, v) => d.SetBinData(k, (BSONBinData) v)},
		};
		readonly List<BSONValue> _fieldslist;
		[NonSerializedAttribute]
		Dictionary<string, BSONValue> _fields;
		[NonSerializedAttribute]
		int? _cachedhash;

		/// <summary>
		/// BSON Type this document. 
		/// </summary>
		/// <remarks>
		/// Type can be either <see cref="Ejdb.SON.BSONType.OBJECT"/> or <see cref="Ejdb.SON.BSONType.ARRAY"/>
		/// </remarks>
		/// <value>The type of the BSON.</value>
		public virtual BSONType BSONType {
			get {
				return BSONType.OBJECT;
			}
		}

		/// <summary>
		/// Gets the document keys.
		/// </summary>
		public ICollection<string> Keys {
			get {
				CheckFields();
				return _fields.Keys;
			}
		}

		/// <summary>
		/// Gets count of document keys.
		/// </summary>
		public int KeysCount {
			get {
				return _fieldslist.Count;
			}
		}

		public BSONDocument() {
			this._fields = null;
			this._fieldslist = new List<BSONValue>();
		}

		public BSONDocument(BSONIterator it) : this() {
			while (it.Next() != BSONType.EOO) {
				Add(it.FetchCurrentValue());
			}
		}

		public BSONDocument(byte[] bsdata) : this() {
			using (BSONIterator it = new BSONIterator(bsdata)) {
				while (it.Next() != BSONType.EOO) {
					Add(it.FetchCurrentValue());
				}
			}
		}

		public BSONDocument(Stream bstream) : this() {
			using (BSONIterator it = new BSONIterator(bstream)) {
				while (it.Next() != BSONType.EOO) {
					Add(it.FetchCurrentValue());
				}
			}
		}

		public BSONDocument(BSONDocument doc) : this() {
			foreach (var bv in doc) {
				Add((BSONValue) bv.Clone());
			}
		}

		public IEnumerator<BSONValue> GetEnumerator() {
			return _fieldslist.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		/// <summary>
		/// Convert BSON document object into BSON binary data byte array.
		/// </summary>
		/// <returns>The byte array.</returns>
		public byte[] ToByteArray() {
			byte[] res;
			using (var ms = new MemoryStream()) {
				Serialize(ms);
				res = ms.ToArray();
			}
			return res;
		}

		public string ToDebugDataString() {
			return BitConverter.ToString(ToByteArray());
		}

		/// <summary>
		/// Gets the field value.
		/// </summary>
		/// <returns>The BSON value.</returns>
		/// <see cref="Ejdb.SON.BSONValue"/>
		/// <param name="key">Document field name</param>
		public BSONValue GetBSONValue(string key) {
			CheckFields();
			BSONValue ov;
			if (_fields.TryGetValue(key, out ov)) {
				return ov;
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets the field value object. 
		/// </summary>
		/// <returns>The object value.</returns>
		/// <see cref="Ejdb.SON.BSONValue"/>
		/// <param name="key">Key.</param>
		/// <returns>Key object </c> or <c>null</c> if the key is not exists or value type is either 
		/// <see cref="Ejdb.SON.BSONType.NULL"/> or <see cref="Ejdb.SON.BSONType.UNDEFINED"/></returns>
		public object GetObjectValue(string key) {
			var bv = GetBSONValue(key);
			return bv != null ? bv.Value : null;
		}

		/// <summary>
		/// Gets the <see cref="Ejdb.SON.BSONDocument"/> with the specified key.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <returns>Key object </c> or <c>null</c> if the key is not exists or value type is either 
		/// <see cref="Ejdb.SON.BSONType.NULL"/> or <see cref="Ejdb.SON.BSONType.UNDEFINED"/></returns>
		public object this[string key] {
			get {
				return GetObjectValue(key);
			}
			set {
				object v = value;
				if (v == null) {
					SetNull(key);
					return;
				}
				Action<BSONDocument, string, object> setter;
				TYPE_SETTERS.TryGetValue(v.GetType(), out setter);
				if (setter == null) {
					throw new Exception(string.Format("Unsupported value type: {0} for doc[key] assign operation", v.GetType()));
				}
				setter(this, key, v);
			}
		}

		public BSONDocument SetNull(string key) {
			return SetBSONValue(new BSONValue(BSONType.NULL, key));
		}

		public BSONDocument SetUndefined(string key) {
			return SetBSONValue(new BSONValue(BSONType.UNKNOWN, key));
		}

		public BSONDocument SetMaxKey(string key) {
			return SetBSONValue(new BSONValue(BSONType.MAXKEY, key));
		}

		public BSONDocument SetMinKey(string key) {
			return SetBSONValue(new BSONValue(BSONType.MINKEY, key));
		}

		public BSONDocument SetOID(string key, string oid) {
			return SetBSONValue(new BSONValue(BSONType.OID, key, new BSONOid(oid)));
		}

		public BSONDocument SetOID(string key, BSONOid oid) {
			return SetBSONValue(new BSONValue(BSONType.OID, key, oid));
		}

		public BSONDocument SetBool(string key, bool val) {
			return SetBSONValue(new BSONValue(BSONType.BOOL, key, val));
		}

		public BSONDocument SetNumber(string key, int val) {
			return SetBSONValue(new BSONValue(BSONType.INT, key, val));
		}

		public BSONDocument SetNumber(string key, long val) {
			return SetBSONValue(new BSONValue(BSONType.LONG, key, val));
		}

		public BSONDocument SetNumber(string key, double val) {
			return SetBSONValue(new BSONValue(BSONType.DOUBLE, key, val));
		}

		public BSONDocument SetNumber(string key, float val) {
			return SetBSONValue(new BSONValue(BSONType.DOUBLE, key, val));
		}

		public BSONDocument SetString(string key, string val) {
			return SetBSONValue(new BSONValue(BSONType.STRING, key, val));
		}

		public BSONDocument SetCode(string key, string val) {
			return SetBSONValue(new BSONValue(BSONType.CODE, key, val));
		}

		public BSONDocument SetSymbol(string key, string val) {
			return SetBSONValue(new BSONValue(BSONType.SYMBOL, key, val));
		}

		public BSONDocument SetDate(string key, DateTime val) {
			return SetBSONValue(new BSONValue(BSONType.DATE, key, val));
		}

		public BSONDocument SetRegexp(string key, BSONRegexp val) {
			return SetBSONValue(new BSONValue(BSONType.REGEX, key, val));
		}

		public BSONDocument SetBinData(string key, BSONBinData val) {
			return SetBSONValue(new BSONValue(BSONType.BINDATA, key, val));
		}

		public BSONDocument SetObject(string key, BSONDocument val) {
			return SetBSONValue(new BSONValue(BSONType.OBJECT, key, val));
		}

		public BSONDocument SetArray(string key, BSONArray val) {
			return SetBSONValue(new BSONValue(BSONType.ARRAY, key, val));
		}

		public BSONDocument SetTimestamp(string key, BSONTimestamp val) {
			return SetBSONValue(new BSONValue(BSONType.TIMESTAMP, key, val));
		}

		public BSONDocument SetCodeWScope(string key, BSONCodeWScope val) {
			return SetBSONValue(new BSONValue(BSONType.CODEWSCOPE, key, val));
		}

		public BSONValue DropValue(string key) {
			var bv = GetBSONValue(key);
			if (bv == null) {
				return bv;
			}
			_cachedhash = null;
			_fields.Remove(key);
			_fieldslist.RemoveAll(x => x.Key == key);
			return bv;
		}

		/// <summary>
		/// Removes all data from document.
		/// </summary>
		public void Clear() {
			_cachedhash = null;
			_fieldslist.Clear();
			if (_fields != null) {
				_fields.Clear();
				_fields = null;
			}
		}

		public void Serialize(Stream os) {
			if (os.CanSeek) {
				long start = os.Position;
				os.Position += 4; //skip int32 document size
				using (var bw = new ExtBinaryWriter(os, Encoding.UTF8, true)) {
					foreach (BSONValue bv in _fieldslist) {
						WriteBSONValue(bv, bw);
					}
					bw.Write((byte) 0x00);
					long end = os.Position;
					os.Position = start;
					bw.Write((int) (end - start));
					os.Position = end; //go to the end
				}
			} else {
				byte[] darr;
				var ms = new MemoryStream();
				using (var bw = new ExtBinaryWriter(ms)) {
					foreach (BSONValue bv in _fieldslist) {
						WriteBSONValue(bv, bw);
					}
					darr = ms.ToArray();
				}	
				using (var bw = new ExtBinaryWriter(os, Encoding.UTF8, true)) {
					bw.Write(darr.Length + 4/*doclen*/ + 1/*0x00*/);
					bw.Write(darr);
					bw.Write((byte) 0x00); 
				}
			}
			os.Flush();
		}

		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (!(obj is BSONDocument)) {
				return false;
			}
			BSONDocument d1 = this;
			BSONDocument d2 = ((BSONDocument) obj);
			if (d1.KeysCount != d2.KeysCount) {
				return false;
			}
			foreach (BSONValue bv1 in d1._fieldslist) {
				BSONValue bv2 = d2.GetBSONValue(bv1.Key);
				if (bv1 != bv2) {
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode() {
			if (_cachedhash != null) {
				return (int) _cachedhash;
			}
			unchecked {
				int hash = 1;
				foreach (var bv in _fieldslist) {
					hash = (hash * 31) + bv.GetHashCode(); 
				}
				_cachedhash = hash;
			}
			return (int) _cachedhash;
		}

		public static bool operator ==(BSONDocument d1, BSONDocument d2) {
			return Equals(d1, d2);
		}

		public static bool operator !=(BSONDocument d1, BSONDocument d2) {
			return !(d1 == d2);
		}

		public object Clone() {
			return new BSONDocument(this);
		}

		public override string ToString() {
			return string.Format("[{0}: {1}]", GetType().Name, _fieldslist);
		}
		//.//////////////////////////////////////////////////////////////////
		// 						Private staff										  
		//.//////////////////////////////////////////////////////////////////
		internal BSONDocument Add(BSONValue bv) {
			_cachedhash = null;
			_fieldslist.Add(bv);
			if (_fields != null) {
				_fields[bv.Key] = bv;
			}
			return this;
		}

		internal BSONDocument SetBSONValue(BSONValue val) {
			_cachedhash = null;
			CheckFields();
			BSONValue ov;
			if (_fields.TryGetValue(val.Key, out ov)) {
				ov.Key = val.Key;
				ov.BSONType = val.BSONType;
				ov.Value = val.Value;
			} else {
				_fieldslist.Add(val);
				_fields.Add(val.Key, val);
			}
			return this;
		}

		protected virtual void CheckKey(string key) {
		}

		protected void WriteBSONValue(BSONValue bv, ExtBinaryWriter bw) {
			BSONType bt = bv.BSONType;
			switch (bt) {
				case BSONType.EOO:
					break;
				case BSONType.NULL:
				case BSONType.UNDEFINED:	
				case BSONType.MAXKEY:
				case BSONType.MINKEY:
					WriteTypeAndKey(bv, bw);
					break;
				case BSONType.OID:
					{
						WriteTypeAndKey(bv, bw);
						BSONOid oid = (BSONOid) bv.Value;
						Debug.Assert(oid._bytes.Length == 12);
						bw.Write(oid._bytes);
						break;
					}
				case BSONType.STRING:
				case BSONType.CODE:
				case BSONType.SYMBOL:										
					WriteTypeAndKey(bv, bw);
					bw.WriteBSONString((string) bv.Value);
					break;
				case BSONType.BOOL:
					WriteTypeAndKey(bv, bw);
					bw.Write((bool) bv.Value);
					break;
				case BSONType.INT:
					WriteTypeAndKey(bv, bw);
					bw.Write((int) bv.Value);
					break;
				case BSONType.LONG:
					WriteTypeAndKey(bv, bw);
					bw.Write((long) bv.Value);
					break;
				case BSONType.ARRAY:
				case BSONType.OBJECT:
					{					
						BSONDocument doc = (BSONDocument) bv.Value;
						WriteTypeAndKey(bv, bw);
						doc.Serialize(bw.BaseStream);
						break;
					}
				case BSONType.DATE:
					{	
						DateTime dt = (DateTime) bv.Value;
						var diff = dt.ToUniversalTime() - BSONConstants.Epoch;
						long time = (long) Math.Floor(diff.TotalMilliseconds);
						WriteTypeAndKey(bv, bw);
						bw.Write(time);
						break;
					}
				case BSONType.DOUBLE:
					WriteTypeAndKey(bv, bw);
					bw.Write((double) bv.Value);
					break;	
				case BSONType.REGEX:
					{
						BSONRegexp rv = (BSONRegexp) bv.Value;		
						WriteTypeAndKey(bv, bw);
						bw.WriteCString(rv.Re ?? "");
						bw.WriteCString(rv.Opts ?? "");						
						break;
					}				
				case BSONType.BINDATA:
					{						
						BSONBinData bdata = (BSONBinData) bv.Value;
						WriteTypeAndKey(bv, bw);
						bw.Write(bdata.Data.Length);
						bw.Write(bdata.Subtype);
						bw.Write(bdata.Data);						
						break;		
					}
				case BSONType.DBREF:
					//Unsupported DBREF!
					break;
				case BSONType.TIMESTAMP:
					{
						BSONTimestamp ts = (BSONTimestamp) bv.Value;
						WriteTypeAndKey(bv, bw);
						bw.Write(ts.Inc);
						bw.Write(ts.Ts);
						break;										
					}		
				case BSONType.CODEWSCOPE:
					{
						BSONCodeWScope cw = (BSONCodeWScope) bv.Value;						
						WriteTypeAndKey(bv, bw);						
						using (var cwwr = new ExtBinaryWriter(new MemoryStream())) {							
							cwwr.WriteBSONString(cw.Code);
							cw.Scope.Serialize(cwwr.BaseStream);					
							byte[] cwdata = ((MemoryStream) cwwr.BaseStream).ToArray();
							bw.Write(cwdata.Length);
							bw.Write(cwdata);
						}
						break;
					}
				default:
					throw new InvalidBSONDataException("Unknown entry type: " + bt);											
			}		
		}

		protected void WriteTypeAndKey(BSONValue bv, ExtBinaryWriter bw) {
			bw.Write((byte) bv.BSONType);
			bw.WriteCString(bv.Key);
		}

		protected void CheckFields() {
			if (_fields != null) {
				return;
			}
			_fields = new Dictionary<string, BSONValue>(Math.Max(_fieldslist.Count + 1, 32));
			foreach (var bv in _fieldslist) {
				_fields.Add(bv.Key, bv);
			}
		}
	}
}
