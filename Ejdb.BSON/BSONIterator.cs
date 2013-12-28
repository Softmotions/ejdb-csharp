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
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Ejdb.IO;
using System.Collections.Generic;

namespace Ejdb.BSON {

	public sealed class BsonIterator : IDisposable, IEnumerable<BsonType> 
    {
        private static readonly Encoding STRICT_ENCODING = new UTF8Encoding(false, true);

		ExtBinaryReader _input;

		bool _closeOnDispose = true;

		bool _disposed;

		int _doclen;

		BsonType _ctype = BsonType.UNKNOWN;

		string _entryKey;

		int _entryLen;

		bool _entryDataSkipped;

		BsonValue _entryDataValue;

		/// <summary>
		/// Returns <c>true</c> if this <see cref="BsonIterator"/> is disposed. 
		/// </summary>
		public bool Disposed {
			get {
				return _disposed;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="BsonIterator"/> is empty.
		/// </summary>
		public bool Empty {
			get {
				return (_ctype == BsonType.EOO);
			}
		}

		/// <summary>
		/// Gets the length of the document in bytes represented by this iterator.
		/// </summary>
		public int DocumentLength {
			get { return _doclen; }
			private set { _doclen = value; }
		}

		/// <summary>
		/// Gets the current document key pointed by this iterator.
		/// </summary>
		public string CurrentKey {
			get { return _entryKey; }
		}

		public BsonIterator() { //empty iterator
			this._ctype = BsonType.EOO;
		}

		public BsonIterator(BsonDocument doc) : this(doc.ToByteArray()) {
		}

		public BsonIterator(byte[] bbuf) : this(new MemoryStream(bbuf)) {
		}

		public BsonIterator(Stream input) {
			if (!input.CanRead) {
				Dispose();
				throw new IOException("Input stream must be readable");
			}
			if (!input.CanSeek) {
				Dispose();
				throw new IOException("Input stream must be seekable");
			}
			this._input = new ExtBinaryReader(input);
			this._ctype = BsonType.UNKNOWN;
			this._doclen = _input.ReadInt32();
			if (this._doclen < 5) {
				Dispose();
				throw new InvalidBSONDataException("Unexpected end of BSON document");
			}
		}

		BsonIterator(ExtBinaryReader input, int doclen) {
			this._input = input;
			this._doclen = doclen;
			if (this._doclen < 5) {
				Dispose();
				throw new InvalidBSONDataException("Unexpected end of BSON document");
			}
		}

		internal BsonIterator(BsonIterator it) : this(it._input, it._entryLen + 4) {
			_closeOnDispose = false;
			it._entryDataSkipped = true;
		}

		~BsonIterator() {
			Dispose();
		}

		public void Dispose() {
			_disposed = true;
			if (_closeOnDispose && _input != null) {
				_input.Close();
				_input = null;
			}
		}

		void CheckDisposed() {
			if (Disposed) {
				throw new ObjectDisposedException("BsonIterator");
			}
		}

		public IEnumerator<BsonType> GetEnumerator() {
			while (Next() != BsonType.EOO) {
				yield return _ctype;
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public IEnumerable<BsonValue> Values() {
			while (Next() != BsonType.EOO) {
				yield return FetchCurrentValue();
			}
		}

		public BsonDocument ToBsonDocument(params string[] fields)
		{
			if (fields.Length > 0)
				return new BsonDocument(this, fields);

			return new BsonDocument(this);
		}

		public T To<T>()
		{
			var classMap = BsonClassSerialization.LookupClassMap(typeof (T));
			var obj = classMap.Creator();

			while (Next() != BsonType.EOO)
			{
				BsonMemberMap memberMap;
				if (classMap.AllMemberMaps.TryGetValue(CurrentKey, out memberMap))
				{
					var value = FetchCurrentValue();
					memberMap.Setter(obj, value.Value);
				}
				else
					Debug.Fail(string.Format("Could not find a property '{0}' on type {1}", CurrentKey, typeof (T).Name));
			}

			return (T) obj;
		}

		public BsonType Next() {
			CheckDisposed();
			if (_ctype == BsonType.EOO) {
				return BsonType.EOO;
			}
			if (!_entryDataSkipped && _ctype != BsonType.UNKNOWN) {
				SkipData();
			}
			byte bv = _input.ReadByte();
			if (!Enum.IsDefined(typeof(BsonType), bv)) {
				throw new InvalidBSONDataException("Unknown bson type: " + bv);
			}
			_entryDataSkipped = false;
			_entryDataValue = null;
			_entryKey = null;
			_ctype = (BsonType) bv;
			_entryLen = 0;
			if (_ctype != BsonType.EOO) {
				ReadKey();
			}
			switch (_ctype) {
				case BsonType.EOO:
					Dispose();
					return BsonType.EOO;
				case BsonType.UNDEFINED:
				case BsonType.NULL:
				case BsonType.MAXKEY:
				case BsonType.MINKEY:	
					_entryLen = 0;
					break;
				case BsonType.BOOL:
					_entryLen = 1;
					break;
				case BsonType.INT:
					_entryLen = 4;
					break;
				case BsonType.LONG:
				case BsonType.DOUBLE:
				case BsonType.TIMESTAMP:
				case BsonType.DATE:
					_entryLen = 8;
					break;
				case BsonType.OID:
					_entryLen = 12;
					break;	
				case BsonType.STRING:
				case BsonType.CODE:
				case BsonType.SYMBOL:
					_entryLen = _input.ReadInt32();
					break;
				case BsonType.DBREF:
					//Unsupported DBREF!
					_entryLen = 12 + _input.ReadInt32();
					break;
				case BsonType.BINDATA:
					_entryLen = 1 + _input.ReadInt32();
					break;
				case BsonType.OBJECT:
				case BsonType.ARRAY:
				case BsonType.CODEWSCOPE:
					_entryLen = _input.ReadInt32() - 4; 
					Debug.Assert(_entryLen > 0);
					break;
				case BsonType.REGEX:
					_entryLen = 0;
					break;				
				default:
					throw new InvalidBSONDataException("Unknown entry type: " + _ctype);
			}
			return _ctype;
		}

		public BsonValue FetchCurrentValue() {
			CheckDisposed();
			if (_entryDataSkipped) {
				return _entryDataValue;
			}
			_entryDataSkipped = true;
			switch (_ctype) {
                case BsonType.EOO:
                case BsonType.UNDEFINED:
                case BsonType.NULL:
                case BsonType.MAXKEY:
                case BsonType.MINKEY:
                    _entryDataValue = new BsonValue(_ctype, _entryKey);
                    break;
                case BsonType.OID:
                    Debug.Assert(_entryLen == 12);
                    var bytes = _input.ReadBytes(12);
                    _entryDataValue = new BsonValue(_ctype, new BsonOid(bytes));
                    break;
                case BsonType.STRING:
                case BsonType.CODE:
                case BsonType.SYMBOL:
                    {
                        if (_entryLen <= 0)
                        {
                            var message = string.Format("Invalid string length: {0} (the length includes the null terminator so it must be greater than or equal to 1).", _entryLen);
                            throw new BsonSerializationException(message);
                        }

                        string sv = STRICT_ENCODING.GetString(_input.ReadBytes(_entryLen - 1));
                        _entryDataValue = new BsonValue(_ctype, (object) sv);
                        var finalByte = _input.ReadByte();

                        if (finalByte != 0x00)
                            throw new BsonSerializationException("String is missing null terminator.");

                        break;
                    }
                case BsonType.BOOL:
                    _entryDataValue = new BsonValue(_ctype, (object) _input.ReadBoolean());
                    break;
                case BsonType.INT:
                    _entryDataValue = new BsonValue(_ctype, (object) _input.ReadInt32());
                    break;
                case BsonType.OBJECT:
                case BsonType.ARRAY:
                    {
                        BsonDocument doc = (_ctype == BsonType.OBJECT ? new BsonDocument() : new BsonArray());
                        BsonIterator sit = new BsonIterator(this);
                        while (sit.Next() != BsonType.EOO)
                        {
                            doc.Add(sit.CurrentKey, sit.FetchCurrentValue());
                        }
                        _entryDataValue = new BsonValue(_ctype, doc);
                        break;
                    }
                case BsonType.DOUBLE:
                    _entryDataValue = new BsonValue(_ctype, _input.ReadDouble());
                    break;
                case BsonType.LONG:
                    _entryDataValue = new BsonValue(_ctype, _input.ReadInt64());
                    break;
                case BsonType.DATE:
			        var milliseconds = _input.ReadInt64();
			        var dateTime = BsonConstants.Epoch.AddMilliseconds(milliseconds);
			        _entryDataValue = new BsonValue(_ctype, dateTime);
                    break;
                case BsonType.TIMESTAMP:
                    {
                        int inc = _input.ReadInt32();
                        int ts = _input.ReadInt32();
                        _entryDataValue = new BsonValue(_ctype,
                                                        new BsonTimestamp(inc, ts));
                        break;
                    }
                case BsonType.REGEX:
                    {
                        string re = _input.ReadCString();
                        string opts = _input.ReadCString();
                        _entryDataValue = new BsonValue(_ctype,
                                                        new BsonRegexp(re, opts));
                        break;
                    }
                case BsonType.BINDATA:
                    {
                        byte subtype = _input.ReadByte();
                        BsonBinData bd = new BsonBinData(subtype, _entryLen - 1, _input);
                        _entryDataValue = new BsonValue(_ctype, bd);
                        break;
                    }
                case BsonType.DBREF:
                    {
                        //Unsupported DBREF!
                        SkipData(true);
                        _entryDataValue = new BsonValue(_ctype, _entryKey);
                        break;
                    }
                case BsonType.CODEWSCOPE:
                    {
                        int cwlen = _entryLen + 4;
                        Debug.Assert(cwlen > 5);
                        int clen = _input.ReadInt32(); //code length
                        string code = STRICT_ENCODING.GetString(_input.ReadBytes(clen));
                        BsonCodeWScope cw = new BsonCodeWScope(code);
                        BsonIterator sit = new BsonIterator(_input, _input.ReadInt32());
                        while (sit.Next() != BsonType.EOO)
                        {
                            cw.Add(sit.CurrentKey, sit.FetchCurrentValue());
                        }
                        _entryDataValue = new BsonValue(_ctype, (object) cw);
                        break;
                    }				
			}
			return _entryDataValue;
		}
		//.//////////////////////////////////////////////////////////////////
		// 							Private staff								  
		//.//////////////////////////////////////////////////////////////////
		internal void SkipData(bool force = false) {
			if (_entryDataSkipped && !force) {
				return;
			}
			_entryDataValue = null;
			_entryDataSkipped = true;
			if (_ctype == BsonType.REGEX) {
				_input.SkipCString();
				_input.SkipCString();
				Debug.Assert(_entryLen == 0);
			} else if (_entryLen > 0) {
				long cpos = _input.BaseStream.Position;
				if ((cpos + _entryLen) != _input.BaseStream.Seek(_entryLen, SeekOrigin.Current)) {
					throw new IOException("Inconsitent seek within input BSON stream");
				}
				_entryLen = 0;
			} 
		}

		string ReadKey() {
			_entryKey = _input.ReadCString();
			return _entryKey;
		}
	}
}

