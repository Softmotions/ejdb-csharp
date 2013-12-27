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

using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Ejdb.IO {

	public class ExtBinaryReader : BinaryReader {

	    private static readonly Encoding DEFAULT_ENCODING = new UTF8Encoding(false, true);

		bool _leaveopen;

		public ExtBinaryReader(Stream input) 
            : this(input, DEFAULT_ENCODING) {
		}

		public ExtBinaryReader(Stream input, Encoding encoding) 
            : this(input, encoding, false) {
		}

		public ExtBinaryReader(Stream input, bool leaveOpen)
            : this(input, DEFAULT_ENCODING, leaveOpen) 
        {
		}

		public ExtBinaryReader(Stream input, Encoding encoding, bool leaveopen) 
            : base(input, encoding) 
        {
			_leaveopen = leaveopen;
		}

		protected override void Dispose(bool disposing) {
			base.Dispose(!_leaveopen);
		}

		public string ReadCString() {
			var bytes = new List<byte>(64);
			byte currentByte;
		    while ((currentByte = ReadByte()) != 0x00)
		        bytes.Add(currentByte);

            return DEFAULT_ENCODING.GetString(bytes.ToArray());
		}

		public void SkipCString() {
			while ((ReadByte()) != 0x00)
				;
		}
	}
}

