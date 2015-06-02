// ============================================================================================
//   .NET API for EJDB database library http://ejdb.org
//   Copyright (C) 2012-2015 Softmotions Ltd <info@softmotions.com>
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
using NUnit.Framework;
using Ejdb.BSON;
using System.IO;

namespace Ejdb.Tests {

	[TestFixture]
	public class TestBSON {

		[Test]
		public void TestSerializeEmpty() {
			BSONDocument doc = new BSONDocument();
			Assert.AreEqual("05-00-00-00-00", doc.ToDebugDataString()); 
		}

		[Test]
		public void TestSerialize1() {
			byte[] bdata;
			BSONDocument doc = new BSONDocument();
			doc.SetNumber("0", 1);
			//0C-00-00-00 	len
			//10		  	type
			//30-00 		key
			//01-00-00-00	int val
			//00			zero term
			bdata = doc.ToByteArray();
			Assert.AreEqual("0C-00-00-00-10-30-00-01-00-00-00-00", doc.ToDebugDataString());
			Assert.AreEqual(bdata.Length, (int) Convert.ToByte(doc.ToDebugDataString().Substring(0, 2), 16));

			BSONDocument doc2 = new BSONDocument(doc.ToByteArray());
			Assert.AreEqual(1, doc2.KeysCount);
			int c = 0;
			foreach (BSONValue bv in doc2) {
				c++;		
				Assert.IsNotNull(bv);
				Assert.AreEqual(BSONType.INT, bv.BSONType);
				Assert.AreEqual("0", bv.Key);
				Assert.AreEqual(1, bv.Value);
			}
			Assert.That(c > 0);
			doc2.SetNumber("0", 2);
			Assert.AreEqual(1, doc2.KeysCount);
			object ival = doc2["0"];
			Assert.IsInstanceOf(typeof(int), ival);
			Assert.AreEqual(2, ival);
			doc2.SetNumber("1", Int32.MaxValue);
			//13-00-00-00
			//10
			//30-00
			//02-00-00-00
			//10-31-00
			//FF-FF-FF-7F
			//00
			Assert.AreEqual("13-00-00-00-10-30-00-02-00-00-00-10-31-00-FF-FF-FF-7F-00", 
			                doc2.ToDebugDataString());

			doc2 = new BSONDocument(doc2);
			Assert.AreEqual("13-00-00-00-10-30-00-02-00-00-00-10-31-00-FF-FF-FF-7F-00", 
			                doc2.ToDebugDataString());

			doc2 = new BSONDocument(doc2.ToByteArray());
			Assert.AreEqual("13-00-00-00-10-30-00-02-00-00-00-10-31-00-FF-FF-FF-7F-00", 
			                doc2.ToDebugDataString());

			doc = new BSONDocument();
			doc["a"] = 1;
			Assert.AreEqual("0C-00-00-00-10-61-00-01-00-00-00-00", doc.ToDebugDataString());		
		}

		[Test]
		public void TestAnonTypes() {
			BSONDocument doc = BSONDocument.ValueOf(new {a = "b", c = 1});
			//15-00-00-00
			//02-61-00
			//02-00-00-00
			//62-00
			//10-63-00-01-00-00-00-00
			Assert.AreEqual("15-00-00-00-02-61-00-02-00-00-00-62-00-10-63-00-01-00-00-00-00", 
			                doc.ToDebugDataString());
			doc["d"] = new{e=new BSONRegexp("r1", "o2")}; //subdocument
			//26-00-00-00-02-61-00-02-00-00-00-62-00-10-63-00-01-00-00-00-
			//03
			//64-00
			//0E-00-00-00
			//0B
			//65-00
			//72-31-00-6F-32-00-00-00
			Assert.AreEqual("26-00-00-00-02-61-00-02-00-00-00-62-00-10-63-00-01-00-00-00-" +
				"03-64-00-0E-00-00-00-0B-65-00-72-31-00-6F-32-00-00-00", 
			                doc.ToDebugDataString());
		}

		[Test]
		public void TestIterate1() {
			var doc = new BSONDocument();
			doc["a"] = "av";
			doc["bb"] = 24;
			//doc["ccc"] = BSONDocument.ValueOf(new{na1 = 1, nb = "2"});
			//doc["d"] = new BSONOid("51b9f3af98195c4600000000");

			//17-00-00-00 						+4
			//02-61-00-03-00-00-00-61-76-00		+10
			//10-62-62-00-18-00-00-00			+8
			//00								+1
			Assert.AreEqual("17-00-00-00-02-61-00-03-00-00-00-61-76-00-10-62-62-00-18-00-00-00-00", 
			                doc.ToDebugDataString());
			BSONIterator it = new BSONIterator(doc);
			Assert.AreEqual(doc.ToByteArray().Length, it.DocumentLength);
			var c = "";
			while (it.Next() != BSONType.EOO) {
				c += it.CurrentKey;
			}	
			Assert.AreEqual("abb", c);
			it.Dispose();

			it = new BSONIterator(doc);
			var cnt = 0;
			while (it.Next() != BSONType.EOO) {
				BSONValue bv = it.FetchCurrentValue();
				Assert.IsNotNull(bv);
				if (cnt == 0) {
					Assert.IsTrue(bv.BSONType == BSONType.STRING);										
					Assert.IsTrue(bv.Key == "a");										
					Assert.AreEqual("av", bv.Value);										
				} 
				if (cnt == 1) {
					Assert.IsTrue(bv.BSONType == BSONType.INT);										
					Assert.IsTrue(bv.Key == "bb");										
					Assert.AreEqual(24, bv.Value);
				}
				cnt++;
			}
		}

		[Test]
		public void TestIterate2() {
			var doc = new BSONDocument();
			doc["a"] = "av";
			doc["b"] = BSONDocument.ValueOf(new{cc = 1});
			doc["d"] = new BSONOid("51b9f3af98195c4600000000");
			Assert.AreEqual(3, doc.KeysCount);
			//Console.WriteLine(doc.KeysCount);
			//Console.WriteLine(doc.ToDebugDataString());
			//2E-00-00-00					   	+4
			//02-61-00-03-00-00-00-61-76-00		+10 (14)
			//03-62-00							+3  (17) "d" = 
			//0D-00-00-00						+4  (21) doc len = 13
			//10-63-63-00-01-00-00-00 -00		+9 	(30)	
			//07-64-00							+3 	(33)
			//51-B9-F3-AF-98-19-5C-46-00-00-00-00	 +12 (45)
			//00									+1 (46)
			Assert.AreEqual("2E-00-00-00-" +
				"02-61-00-03-00-00-00-61-76-00-" +
				"03-62-00-" +
				"0D-00-00-00-" +
				"10-63-63-00-01-00-00-00-00-" +
				"07-64-00-" +
				"51-B9-F3-AF-98-19-5C-46-00-00-00-00-" +
				"00", doc.ToDebugDataString());
			BSONIterator it = new BSONIterator(doc);
			int c = 0;
			foreach (var bt in it) {
				if (c == 0) {
					Assert.IsTrue(bt == BSONType.STRING);
				}
				if (c == 1) {
					Assert.IsTrue(bt == BSONType.OBJECT);
				}
				if (c == 2) {
					Assert.IsTrue(bt == BSONType.OID);
				}
				++c;
			}
			bool thrown = false;
			Assert.IsTrue(it.Disposed);
			try {
				it.Next();
			} catch (ObjectDisposedException) {
				thrown = true;
			}
			Assert.IsTrue(thrown);

			c = 0;
			it = new BSONIterator(doc);
			foreach (var bv in it.Values()) {
				if (c == 0) {
					Assert.AreEqual("a", bv.Key);
					Assert.AreEqual("av", bv.Value);
				}
				if (c == 1) {
					Assert.AreEqual("b", bv.Key);
					BSONDocument sdoc = bv.Value as BSONDocument;
					Assert.IsNotNull(sdoc);
					foreach (var bv2 in new BSONIterator(sdoc).Values()) {
						Assert.AreEqual("cc", bv2.Key);
						Assert.AreEqual(1, bv2.Value);
						Assert.AreEqual(BSONType.INT, bv2.BSONType);
					}
				}
				if (c == 2) {
					Assert.AreEqual(BSONType.OID, bv.BSONType);
					Assert.IsInstanceOf(typeof(BSONOid), bv.Value);
					var oid = bv.Value as BSONOid;
					Assert.AreEqual("51b9f3af98195c4600000000", oid.ToString());
				}
				c++;
			}
		}

		[Test]
		public void TestIterateRE() {
			var doc = new BSONDocument();
			doc["a"] = new BSONRegexp("b", "c");
			doc["d"] = 1;
			doc["e"] = BSONDocument.ValueOf(new {f = new BSONRegexp("g", "")});
			doc["h"] = 2;
			//28-00-00-00
			//0B-61-00-62-00-63-00
			//10-64-00-01-00-00-00
			//03-65-00-0B-00-00-00
			//0B-66-00-67-00-00-00
			//10-68-00-02-00-00-00-00
			var cs = "";
			foreach (var bt in new BSONIterator(doc)) {
				cs += bt.ToString();
			}
			Assert.AreEqual("REGEXINTOBJECTINT", cs);
			cs = "";
			foreach (var bv in new BSONIterator(doc).Values()) {
				if (bv.Key == "a") {
					cs += ((BSONRegexp) bv.Value).Re;
					cs += ((BSONRegexp) bv.Value).Opts;
				} else {
					cs += bv.Value;
				}
			}
			Assert.AreEqual("bc1[BSONDocument: [BSONValue: BSONType=REGEX, Key=f, Value=[BSONRegexp: re=g, opts=]]]2", cs);
		}

		[Test]
		public void TestFilteredDoc() {
			var doc = new BSONDocument();
			doc["c"] = "d";
			doc["aaa"] = 11;
			doc["ndoc"] = BSONDocument.ValueOf(new {
				aaaa = "nv1",
				d = "nv2",
				nnd = BSONDocument.ValueOf(new {
					nnv = true,
					nns = "s"
				})
			});
			doc["ndoc2"] = BSONDocument.ValueOf(new {
				n = "v"
			});
			doc["f"] = "f";
			BSONIterator it = new BSONIterator(doc);
			BSONDocument doc2 = it.ToBSONDocument("c", "ndoc.d", "ndoc.nnd.nns", "f");
			Assert.AreEqual(3, doc2.KeysCount);
			Assert.AreEqual("d", doc2["c"]);
			Assert.AreEqual(2, ((BSONDocument) doc2["ndoc"]).KeysCount);
			Assert.AreEqual("nv2", ((BSONDocument) doc2["ndoc"])["d"]);
			Assert.AreEqual("s", ((BSONDocument) ((BSONDocument) doc2["ndoc"])["nnd"])["nns"]);
			Assert.AreEqual("s", doc2["ndoc.nnd.nns"]);
			Assert.AreEqual("f", "f");
			//Console.WriteLine("doc2=" + doc2);

		}
	}
}

