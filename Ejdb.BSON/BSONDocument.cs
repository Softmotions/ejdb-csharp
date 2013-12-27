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
using System.Linq;

namespace Ejdb.BSON {

	/// <summary>
	/// BSON document deserialized data wrapper.
	/// </summary>
	[Serializable]
    public class BSONDocument : IBSONValue, IEnumerable<BSONValueWithKey>, ICloneable
    {
	    [NonSerializedAttribute]
		Dictionary<string, BSONValue> _fields;

		[NonSerializedAttribute]
		int? _cachedhash;

		/// <summary>
		/// BSON Type this document. 
		/// </summary>
		/// <remarks>
		/// Type can be either <see cref="Ejdb.BSON.BSONType.OBJECT"/> or <see cref="Ejdb.BSON.BSONType.ARRAY"/>
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
				return _fields.Count;
			}
		}

		public BSONDocument() {
            _fields = new Dictionary<string, BSONValue>();
		}

		public BSONDocument(BSONIterator it) : this() {
			while (it.Next() != BSONType.EOO)
			{
			    var value = it.FetchCurrentValue();
			    Add(it.CurrentKey, value);
			}
		}

		public BSONDocument(BSONIterator it, string[] fields) : this() {
			Array.Sort(fields);
			BSONType bt;
			int ind = -1;
			int nfc = 0;
			foreach (string f in fields) {
				if (f != null) {
					nfc++;
				}
			}
			while ((bt = it.Next()) != BSONType.EOO) {
				if (nfc < 1) {
					continue;
				}
				string kk = it.CurrentKey;
				if ((ind = Array.IndexOf(fields, kk)) != -1) {
					Add(it.CurrentKey, it.FetchCurrentValue());
					fields[ind] = null;
					nfc--;
				} else if (bt == BSONType.OBJECT || bt == BSONType.ARRAY) {
					string[] narr = null;
					for (var i = 0; i < fields.Length; ++i) {
						var f = fields[i];
						if (f == null) {
							continue;
						}
						if (f.IndexOf(kk, StringComparison.Ordinal) == 0 && 
							f.Length > kk.Length + 1 && 
							f[kk.Length] == '.') {
							if (narr == null) {
								narr = new string[fields.Length];
							}
							narr[i] = f.Substring(kk.Length + 1);
							fields[i] = null;
							nfc--;
						}
					}
					if (narr != null) {
						BSONIterator nit = new BSONIterator(it);
						BSONDocument ndoc = new BSONDocument(nit, narr);
						if (ndoc.KeysCount > 0) {
							Add(kk, new BSONValue(bt, ndoc));
						}
					}
				}
			}
			it.Dispose();
		}

		public BSONDocument(byte[] bsdata) : this() {
			using (BSONIterator it = new BSONIterator(bsdata)) {
				while (it.Next() != BSONType.EOO) {
					Add(it.CurrentKey, it.FetchCurrentValue());
				}
			}
		}

		public BSONDocument(Stream bstream) : this() {
			using (BSONIterator it = new BSONIterator(bstream)) {
				while (it.Next() != BSONType.EOO) {
					Add(it.CurrentKey, it.FetchCurrentValue());
				}
			}
		}

		public BSONDocument(BSONDocument doc) : this() {
			foreach (var bv in doc._fields) {
				Add(bv.Key, (BSONValue) bv.Value.Clone());
			}
		}

        public IEnumerator<BSONValueWithKey> GetEnumerator()
        {
            foreach (var entry in _fields)
                yield return new BSONValueWithKey(entry.Key, entry.Value, entry.Value.BSONType);
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
		/// <see cref="Ejdb.BSON.BSONValue"/>
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
		/// <remarks>
		/// Hierarchical field paths are NOT supported. Use <c>[]</c> operator instead.
		/// </remarks>
		/// <param name="key">BSON document key</param>
		/// <see cref="Ejdb.BSON.BSONValue"/>
		public object GetObjectValue(string key) {
			var bv = GetBSONValue(key);
			return bv != null ? bv.Value : null;
		}

		/// <summary>
		/// Determines whether this document has the specified key.
		/// </summary>
		/// <returns><c>true</c> if this document has the specified key; otherwise, <c>false</c>.</returns>
		/// <param name="key">Key.</param>
		public bool HasKey(string key) {
			return (GetBSONValue(key) != null);
		}

		/// <summary>
		/// Gets the <see cref="Ejdb.BSON.BSONDocument"/> with the specified key.
		/// </summary>
		/// <remarks>
		/// Getter for hierarchical field paths are supported.
		/// </remarks>
		/// <param name="key">Key.</param>
		/// <returns>Key object </c> or <c>null</c> if the key is not exists or value type is either 
		/// <see cref="Ejdb.BSON.BSONType.NULL"/> or <see cref="Ejdb.BSON.BSONType.UNDEFINED"/></returns>
		public object this[string key] {
			get {
				int ind;
				if ((ind = key.IndexOf(".", StringComparison.Ordinal)) == -1) {
					return GetObjectValue(key);
				} else {
					string prefix = key.Substring(0, ind);
					var doc = GetObjectValue(prefix) as BSONDocument;
					if (doc == null || key.Length < ind + 2) {
						return null;
					}
					return doc[key.Substring(ind + 1)];
				}
			}
			set
			{
                var bsonValue = BSONValue.ValueOf(value);
			    SetBSONValueNew(key, bsonValue);
			}
		}

	    public BSONValue DropValue(string key) {
			var bv = GetBSONValue(key);
			if (bv == null) {
				return bv;
			}
			_cachedhash = null;
			_fields.Remove(key);
			return bv;
		}

		/// <summary>
		/// Removes all data from document.
		/// </summary>
		public void Clear() {
			_cachedhash = null;
		    _fields.Clear();
		}

		public void Serialize(Stream os) {
			if (os.CanSeek) {
				long start = os.Position;
				os.Position += 4; //skip int32 document size
				using (var bw = new ExtBinaryWriter(os, Encoding.UTF8, true)) {
					foreach (var bv in _fields) {
						WriteBSONValue(bv.Key, bv.Value, bw);
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
					foreach (var bv in _fields) {
						WriteBSONValue(bv.Key, bv.Value, bw);
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
			foreach (var bv1 in d1._fields) {
				BSONValue bv2 = d2.GetBSONValue(bv1.Key);
				if (bv1.Value != bv2) {
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
				foreach (var bv in _fields) {
					hash = (hash * 31) + bv.Key.GetHashCode() + bv.Value.GetHashCode(); 
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

		public static BSONDocument ValueOf(object val) {
		    if (val == null)
		        return new BSONDocument();

		    var vtype = val.GetType();

		    if (val is BSONDocument)
		        return (BSONDocument) val;

		    if (vtype == typeof (byte[]))
		        return new BSONDocument((byte[]) val);

		    return BSONValue.GetAnonTypeDocument(val);
		}

		public object Clone() {
			return new BSONDocument(this);
		}

		public override string ToString() {
			return string.Format("[{0}: {1}]", GetType().Name, 
                string.Join(", ", from bv in _fields select bv.Value.ToStringWithKey(bv.Key))); 
		}
		//.//////////////////////////////////////////////////////////////////
		// 						Private staff										  
		//.//////////////////////////////////////////////////////////////////

        internal BSONDocument Add(string key, BSONValue bv)
        {
            _cachedhash = null;
            _fields[key] = bv;
            return this;
        }

	    internal BSONDocument SetBSONValueNew(string key, BSONValue val)
        {
            _cachedhash = null;
            CheckFields();

            if (val.BSONType == BSONType.STRING && key == "_id")
                val = new BSONValue(BSONType.OID, new BSONOid((string)val.Value));

            BSONValue ov;
            if (_fields.TryGetValue(key, out ov))
            {
                ov.BSONType = val.BSONType;
                ov.Value = val.Value;
            }
            else
                _fields.Add(key, val);

            return this;
        }

	    protected virtual void CheckKey(string key) {
		}

        protected void WriteBSONValue(string key, BSONValue bv, ExtBinaryWriter bw) {
            BSONType bt = bv.BSONType;
            switch (bt)
            {
                case BSONType.EOO:
                    break;
                case BSONType.NULL:
                case BSONType.UNDEFINED:
                case BSONType.MAXKEY:
                case BSONType.MINKEY:
                    WriteTypeAndKey(key, bv, bw);
                    break;
                case BSONType.OID:
                    {
                        WriteTypeAndKey(key, bv, bw);
                        BSONOid oid = (BSONOid)bv.Value;
                        Debug.Assert(oid._bytes.Length == 12);
                        bw.Write(oid._bytes);
                        break;
                    }
                case BSONType.STRING:
                case BSONType.CODE:
                case BSONType.SYMBOL:
                    WriteTypeAndKey(key, bv, bw);
                    bw.WriteBSONString((string)bv.Value);
                    break;
                case BSONType.BOOL:
                    WriteTypeAndKey(key, bv, bw);
                    bw.Write((bool)bv.Value);
                    break;
                case BSONType.INT:
                    WriteTypeAndKey(key, bv, bw);
                    bw.Write((int)bv.Value);
                    break;
                case BSONType.LONG:
                    WriteTypeAndKey(key, bv, bw);
                    bw.Write((long)bv.Value);
                    break;
                case BSONType.ARRAY:
                case BSONType.OBJECT:
                    {
                        BSONDocument doc = (BSONDocument)bv.Value;
                        WriteTypeAndKey(key, bv, bw);
                        doc.Serialize(bw.BaseStream);
                        break;
                    }
                case BSONType.DATE:
                    {
                        DateTime dt = (DateTime)bv.Value;
                        var diff = dt.ToLocalTime() - BSONConstants.Epoch;
                        long time = (long)Math.Floor(diff.TotalMilliseconds);
                        WriteTypeAndKey(key, bv, bw);
                        bw.Write(time);
                        break;
                    }
                case BSONType.DOUBLE:
                    WriteTypeAndKey(key, bv, bw);
                    bw.Write((double)bv.Value);
                    break;
                case BSONType.REGEX:
                    {
                        BSONRegexp rv = (BSONRegexp)bv.Value;
                        WriteTypeAndKey(key, bv, bw);
                        bw.WriteCString(rv.Re ?? "");
                        bw.WriteCString(rv.Opts ?? "");
                        break;
                    }
                case BSONType.BINDATA:
                    {
                        BSONBinData bdata = (BSONBinData)bv.Value;
                        WriteTypeAndKey(key, bv, bw);
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
                        BSONTimestamp ts = (BSONTimestamp)bv.Value;
                        WriteTypeAndKey(key, bv, bw);
                        bw.Write(ts.Inc);
                        bw.Write(ts.Ts);
                        break;
                    }
                case BSONType.CODEWSCOPE:
                    {
                        BSONCodeWScope cw = (BSONCodeWScope)bv.Value;
                        WriteTypeAndKey(key, bv, bw);
                        using (var cwwr = new ExtBinaryWriter(new MemoryStream()))
                        {
                            cwwr.WriteBSONString(cw.Code);
                            cw.Scope.Serialize(cwwr.BaseStream);
                            byte[] cwdata = ((MemoryStream)cwwr.BaseStream).ToArray();
                            bw.Write(cwdata.Length);
                            bw.Write(cwdata);
                        }
                        break;
                    }
                default:
                    throw new InvalidBSONDataException("Unknown entry type: " + bt);
            }		
        }

	    protected void WriteTypeAndKey(string key, BSONValue bv, ExtBinaryWriter bw) {
			bw.Write((byte) bv.BSONType);
            bw.WriteCString(key);
		}

		protected void CheckFields()
		{
		}
	}
}
