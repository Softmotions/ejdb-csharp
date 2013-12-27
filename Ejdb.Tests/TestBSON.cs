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
using NUnit.Framework;
using Ejdb.BSON;
using System.IO;

namespace Ejdb.Tests {

	[TestFixture]
	public class TestBSON {

		[Test]
		public void TestSerializeEmpty() {
			BsonDocument doc = new BsonDocument();
			Assert.AreEqual("05-00-00-00-00", doc.ToDebugDataString()); 
		}

		[Test]
		public void TestSerialize1() {
			byte[] bdata;
			BsonDocument doc = new BsonDocument();
			doc.Add("0", BsonValue.GetNumber(1));
			//0C-00-00-00 	len
			//10		  	type
			//30-00 		key
			//01-00-00-00	int val
			//00			zero term
			bdata = doc.ToByteArray();
			Assert.AreEqual("0C-00-00-00-10-30-00-01-00-00-00-00", doc.ToDebugDataString());
			Assert.AreEqual(bdata.Length, (int) Convert.ToByte(doc.ToDebugDataString().Substring(0, 2), 16));

			BsonDocument doc2 = new BsonDocument(doc.ToByteArray());
			Assert.AreEqual(1, doc2.KeysCount);
			int c = 0;
			foreach (var bv in doc2) {
				c++;		
				Assert.IsNotNull(bv);
				Assert.AreEqual(BsonType.INT, bv.BsonType);
				Assert.AreEqual("0", bv.Key);
				Assert.AreEqual(1, bv.Value);
			}
			Assert.That(c > 0);
            doc2.Add("0", BsonValue.GetNumber(2));
			Assert.AreEqual(1, doc2.KeysCount);
			object ival = doc2["0"];
			Assert.IsInstanceOf(typeof(int), ival);
			Assert.AreEqual(2, ival);
			doc2.Add("1", BsonValue.GetNumber(Int32.MaxValue));
			//13-00-00-00
			//10
			//30-00
			//02-00-00-00
			//10-31-00
			//FF-FF-FF-7F
			//00
			Assert.AreEqual("13-00-00-00-10-30-00-02-00-00-00-10-31-00-FF-FF-FF-7F-00", 
			                doc2.ToDebugDataString());

			doc2 = new BsonDocument(doc2);
			Assert.AreEqual("13-00-00-00-10-30-00-02-00-00-00-10-31-00-FF-FF-FF-7F-00", 
			                doc2.ToDebugDataString());

			doc2 = new BsonDocument(doc2.ToByteArray());
			Assert.AreEqual("13-00-00-00-10-30-00-02-00-00-00-10-31-00-FF-FF-FF-7F-00", 
			                doc2.ToDebugDataString());

			doc = new BsonDocument();
			doc["a"] = 1;
			Assert.AreEqual("0C-00-00-00-10-61-00-01-00-00-00-00", doc.ToDebugDataString());		
		}

		[Test]
		public void TestAnonTypes() {
			BsonDocument doc = BsonDocument.ValueOf(new {a = "b", c = 1});
			//15-00-00-00
			//02-61-00
			//02-00-00-00
			//62-00
			//10-63-00-01-00-00-00-00
			Assert.AreEqual("15-00-00-00-02-61-00-02-00-00-00-62-00-10-63-00-01-00-00-00-00", 
			                doc.ToDebugDataString());
			doc["d"] = new{e=new BsonRegexp("r1", "o2")}; //subdocument
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
			var doc = new BsonDocument();
			doc["a"] = "av";
			doc["bb"] = 24;
			//doc["ccc"] = BsonDocument.ValueOf(new{na1 = 1, nb = "2"});
			//doc["d"] = new BsonOid("51b9f3af98195c4600000000");

			//17-00-00-00 						+4
			//02-61-00-03-00-00-00-61-76-00		+10
			//10-62-62-00-18-00-00-00			+8
			//00								+1
			Assert.AreEqual("17-00-00-00-02-61-00-03-00-00-00-61-76-00-10-62-62-00-18-00-00-00-00", 
			                doc.ToDebugDataString());
			BsonIterator it = new BsonIterator(doc);
			Assert.AreEqual(doc.ToByteArray().Length, it.DocumentLength);
			var c = "";
			while (it.Next() != BsonType.EOO) {
				c += it.CurrentKey;
			}	
			Assert.AreEqual("abb", c);
			it.Dispose();

			it = new BsonIterator(doc);
			var cnt = 0;
			while (it.Next() != BsonType.EOO) {
				BsonValue bv = it.FetchCurrentValue();
				Assert.IsNotNull(bv);
				if (cnt == 0) {
					Assert.IsTrue(bv.BSONType == BsonType.STRING);										
					Assert.IsTrue(it.CurrentKey  == "a");										
					Assert.AreEqual("av", bv.Value);										
				} 
				if (cnt == 1) {
					Assert.IsTrue(bv.BSONType == BsonType.INT);
                    Assert.IsTrue(it.CurrentKey == "bb");										
					Assert.AreEqual(24, bv.Value);
				}
				cnt++;
			}
		}

		[Test]
		public void TestIterate2() {
			var doc = new BsonDocument();
			doc["a"] = "av";
			doc["b"] = BsonDocument.ValueOf(new{cc = 1});
			doc["d"] = new BsonOid("51b9f3af98195c4600000000");
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
			BsonIterator it = new BsonIterator(doc);
			int c = 0;
			foreach (var bt in it) {
				if (c == 0) {
					Assert.IsTrue(bt == BsonType.STRING);
				}
				if (c == 1) {
					Assert.IsTrue(bt == BsonType.OBJECT);
				}
				if (c == 2) {
					Assert.IsTrue(bt == BsonType.OID);
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
			it = new BsonIterator(doc);
			foreach (var bv in it.Values()) {
				if (c == 0) {
					Assert.AreEqual("a", it.CurrentKey);
					Assert.AreEqual("av", bv.Value);
				}
				if (c == 1) {
                    Assert.AreEqual("b", it.CurrentKey);
					BsonDocument sdoc = bv.Value as BsonDocument;
					Assert.IsNotNull(sdoc);
				    var it2 = new BsonIterator(sdoc);
				    foreach (var bv2 in it2.Values()) {
                        Assert.AreEqual("cc", it2.CurrentKey);
						Assert.AreEqual(1, bv2.Value);
						Assert.AreEqual(BsonType.INT, bv2.BSONType);
					}
				}
				if (c == 2) {
					Assert.AreEqual(BsonType.OID, bv.BSONType);
					Assert.IsInstanceOf(typeof(BsonOid), bv.Value);
					var oid = bv.Value as BsonOid;
					Assert.AreEqual("51b9f3af98195c4600000000", oid.ToString());
				}
				c++;
			}
		}

		[Test]
		public void TestIterateRE() {
			var doc = new BsonDocument();
			doc["a"] = new BsonRegexp("b", "c");
			doc["d"] = 1;
			doc["e"] = BsonDocument.ValueOf(new {f = new BsonRegexp("g", "")});
			doc["h"] = 2;
			//28-00-00-00
			//0B-61-00-62-00-63-00
			//10-64-00-01-00-00-00
			//03-65-00-0B-00-00-00
			//0B-66-00-67-00-00-00
			//10-68-00-02-00-00-00-00
			var cs = "";
			foreach (var bt in new BsonIterator(doc)) {
				cs += bt.ToString();
			}
			Assert.AreEqual("REGEXINTOBJECTINT", cs);
			cs = "";
		    
            var it = new BsonIterator(doc);
		    foreach (var bv in it.Values()) {
				if (it.CurrentKey == "a") {
					cs += ((BsonRegexp) bv.Value).Re;
					cs += ((BsonRegexp) bv.Value).Opts;
				} else {
					cs += bv.Value;
				}
			}
			Assert.AreEqual("bc1[BsonDocument: [BsonValue: BsonType=REGEX, Key=f, Value=[BsonRegexp: re=g, opts=]]]2", cs);
		}

		[Test]
		public void TestFilteredDoc() {
			var doc = new BsonDocument();
			doc["c"] = "d";
			doc["aaa"] = 11;
			doc["ndoc"] = BsonDocument.ValueOf(new {
				aaaa = "nv1",
				d = "nv2",
				nnd = BsonDocument.ValueOf(new {
					nnv = true,
					nns = "s"
				})
			});
			doc["ndoc2"] = BsonDocument.ValueOf(new {
				n = "v"
			});
			doc["f"] = "f";
			BsonIterator it = new BsonIterator(doc);
			BsonDocument doc2 = it.ToBsonDocument("c", "ndoc.d", "ndoc.nnd.nns", "f");
			Assert.AreEqual(3, doc2.KeysCount);
			Assert.AreEqual("d", doc2["c"]);
			Assert.AreEqual(2, ((BsonDocument) doc2["ndoc"]).KeysCount);
			Assert.AreEqual("nv2", ((BsonDocument) doc2["ndoc"])["d"]);
			Assert.AreEqual("s", ((BsonDocument) ((BsonDocument) doc2["ndoc"])["nnd"])["nns"]);
			Assert.AreEqual("s", doc2["ndoc.nnd.nns"]);
			Assert.AreEqual("f", "f");
			//Console.WriteLine("doc2=" + doc2);

		}
	}
}

