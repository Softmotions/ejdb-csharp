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
using Ejdb.DB;
using Ejdb.BSON;

namespace Ejdb.Tests {

	[TestFixture]
	public class TestEJDB {
	
		[Test]
		public void Test1OpenClose() {
			EJDB jb = new EJDB("testdb1", EJDB.DEFAULT_OPEN_MODE | EJDB.JBOTRUNC);
			Assert.IsTrue(jb.IsOpen);
			Assert.AreEqual(0, jb.LastDBErrorCode);
			Assert.AreEqual("success", jb.LastDBErrorMsg);
			jb.Dispose();
			Assert.IsFalse(jb.IsOpen);
			jb.Dispose(); //double dispose
		}

		[Test]
		public void Test2EnsureCollection() {
			EJDB jb = new EJDB("testdb1", EJDB.DEFAULT_OPEN_MODE | EJDB.JBOTRUNC);
			EJDBCollectionOptionsN co = new EJDBCollectionOptionsN();
			co.large = true;
			co.compressed = false;
			co.records = 50000;
			Assert.IsTrue(jb.EnsureCollection("mycoll2", co));
			jb.Dispose(); 
		}

		[Test]
		public void Test3SaveLoad() {
			EJDB jb = new EJDB("testdb1", EJDB.DEFAULT_OPEN_MODE | EJDB.JBOTRUNC);
			Assert.IsTrue(jb.IsOpen);
		    BsonDocument doc = new BsonDocument();
            doc.Add("age", BsonValue.Create(33));
            Assert.IsNull(doc[BsonConstants.Id]);
			bool rv = jb.Save("mycoll", doc);
			Assert.IsTrue(rv);
			Assert.IsNotNull(doc["_id"]);
			Assert.IsInstanceOf(typeof(BsonOid), doc["_id"]); 
			rv = jb.Save("mycoll", doc);
			Assert.IsTrue(rv);

			BsonIterator it = jb.Load("mycoll", doc["_id"] as BsonOid);
			Assert.IsNotNull(it);

			BsonDocument doc2 = it.ToBsonDocument();
			Assert.AreEqual(doc.ToDebugDataString(), doc2.ToDebugDataString());
			Assert.IsTrue(doc == doc2);

			Assert.AreEqual(1, jb.CreateQueryFor("mycoll").Count());
			Assert.IsTrue(jb.Remove("mycoll", doc["_id"] as BsonOid));
			Assert.AreEqual(0, jb.CreateQueryFor("mycoll").Count());

			jb.Save("mycoll", doc);
			Assert.AreEqual(1, jb.CreateQueryFor("mycoll").Count());
			Assert.IsTrue(jb.DropCollection("mycoll"));
			Assert.AreEqual(0, jb.CreateQueryFor("mycoll").Count());

			Assert.IsTrue(jb.Sync());
			jb.Dispose(); 
		}

		[Test]
		public void Test4Q1() {
			EJDB jb = new EJDB("testdb1", EJDB.DEFAULT_OPEN_MODE | EJDB.JBOTRUNC);
			Assert.IsTrue(jb.IsOpen);
		    var doc = new BsonDocument();
		    doc.Add("age", BsonValue.Create(33));
			Assert.IsNull(doc["_id"]);
			bool rv = jb.Save("mycoll", doc);
			Assert.IsTrue(rv);
			Assert.IsNotNull(doc["_id"]);
			EJDBQuery q = jb.CreateQuery(BsonDocument.ValueOf(new{age = 33}), "mycoll");
			Assert.IsNotNull(q);
			using (EJDBQCursor cursor = q.Find()) {
				Assert.IsNotNull(cursor);
				Assert.AreEqual(1, cursor.Length);
				int c = 0;
				foreach (BsonIterator oit in cursor) {
					c++;
					Assert.IsNotNull(oit);
					BsonDocument rdoc = oit.ToBsonDocument();
					Assert.IsTrue(rdoc.HasKey("_id"));
					Assert.AreEqual(33, rdoc["age"]);
				}
				Assert.AreEqual(1, c);
			}
			using (EJDBQCursor cursor = q.Find(null, EJDBQuery.EXPLAIN_FLAG)) {
				Assert.IsNotNull(cursor);
				Assert.AreEqual(1, cursor.Length);
				Assert.IsTrue(cursor.Log.IndexOf("MAX: 4294967295") != -1);
				Assert.IsTrue(cursor.Log.IndexOf("SKIP: 0") != -1);
				Assert.IsTrue(cursor.Log.IndexOf("RS SIZE: 1") != -1);
			}
			q.Max(10);
			using (EJDBQCursor cursor = q.Find(null, EJDBQuery.EXPLAIN_FLAG)) {
				Assert.IsTrue(cursor.Log.IndexOf("MAX: 10") != -1);
			}

			q.Dispose();
			jb.Dispose(); 
		}

		[Test]
		public void Test4Q2() 
        {
			EJDB jb = new EJDB("testdb1", EJDB.DEFAULT_OPEN_MODE | EJDB.JBOTRUNC);
			Assert.IsTrue(jb.IsOpen);

			var parrot1 = BsonDocument.ValueOf(new{
				name = "Grenny",
				type = "African Grey",
				male = true, 
				age = 1,
				birthdate = DateTime.Now,
				likes = new string[] { "green color", "night", "toys" },
				extra1 = BsonNull.VALUE
			});

			var parrot2 = BsonDocument.ValueOf(new{
				name = "Bounty",
				type = "Cockatoo",
				male = false,
				age = 15,
				birthdate = DateTime.Now,
				likes = new string[] { "sugar cane" },
				extra1 = BsonNull.VALUE
			});
			Assert.IsTrue(jb.Save("parrots", parrot1, parrot2));
			Assert.AreEqual(2, jb.CreateQueryFor("parrots").Count()); 

			var q = jb.CreateQuery(new{
					name = new BsonRegexp("(grenny|bounty)", "i")
			}).SetDefaultCollection("parrots").OrderBy("name");

			using (var cur = q.Find()) {
				Assert.AreEqual(2, cur.Length);

				var doc = cur[0].ToBsonDocument();
				Assert.AreEqual("Bounty", doc["name"]);
				Assert.AreEqual(15, doc["age"]);

				doc = cur[1].ToBsonDocument();
				Assert.AreEqual("Grenny", doc["name"]);
				Assert.AreEqual(1, doc["age"]);
			}
			q.Dispose();

			q = jb.CreateQueryFor("parrots");
			Assert.AreEqual(2, q.Count());
			q.AddOR(new{
				name = "Grenny"
			});
			Assert.AreEqual(1, q.Count());
			q.AddOR(new{
				name = "Bounty"
			});
			Assert.AreEqual(2, q.Count());
			q.AddOR(new{
				name = "Bounty2"
			});
			Assert.AreEqual(2, q.Count());

			//Console.WriteLine(jb.DBMeta);
			//[BsonDocument: [BsonValue: BsonType=STRING, Key=file, Value=testdb1], 
			//[BsonValue: BsonType=ARRAY, Key=collections, Value=[BsonArray: [BsonValue: BsonType=OBJECT, Key=0, Value=[BsonDocument: [BsonValue: BsonType=STRING, Key=name, Value=parrots], [BsonValue: BsonType=STRING, Key=file, Value=testdb1_parrots], [BsonValue: BsonType=LONG, Key=records, Value=2], [BsonValue: BsonType=OBJECT, Key=options, Value=[BsonDocument: [BsonValue: BsonType=LONG, Key=buckets, Value=131071], [BsonValue: BsonType=LONG, Key=cachedrecords, Value=0], [BsonValue: BsonType=BOOL, Key=large, Value=False], [BsonValue: BsonType=BOOL, Key=compressed, Value=False]]], [BsonValue: BsonType=ARRAY, Key=indexes, Value=[BsonArray: ]]]]]]]

			q.Dispose();

			//Test command execution
			BsonDocument cmret = jb.Command(BsonDocument.ValueOf(new {
				ping = ""
			}));
			Assert.IsNotNull(cmret);
			Assert.AreEqual("pong", cmret["log"]);

			jb.Dispose();
		}
	}
}

