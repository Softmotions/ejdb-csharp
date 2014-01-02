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
using System.Diagnostics;
using Ejdb.BSON;
using Ejdb.DB;

namespace sample
{
	internal class MainClass
	{
		public static void Main(string[] args)
		{
			_PerformanceDemo();
			_ZooDemo();
		}

		private static void _PerformanceDemo()
		{
			var jb = new EJDB("performance-demo", EJDB.DEFAULT_OPEN_MODE | EJDB.JBOTRUNC);
			jb.ThrowExceptionOnFail = true;

			var posts = new BsonDocument[5002];
			var text = new string('-', 2000);

			for (int i = 0; i < posts.Length; i++)
			{
				posts[i] = new BsonDocument()
				{
					{ "Text", text },
					{ "CreationDate", BsonValue.Create(DateTime.Now) },
					{ "LastChangeDate", BsonValue.Create(DateTime.Now) },
				};
			}

			var collectionName = "posts";
			jb.Save(collectionName, posts);

			var tests = new Tests();
			tests.Add(i => jb.Load(collectionName, (BsonOid) posts[i]["_id"]), "Load without serialization");
			tests.Add(i =>
				{
					var iterator = jb.Load(collectionName, (BsonOid) posts[i]["_id"]);
					iterator.ToBsonDocument();
				}, "Load with deserialization as BSON document");

			tests.Add(i =>
			{
				var iterator = jb.Load(collectionName, (BsonOid)posts[i]["_id"]);
				iterator.To<Post>();
			}, "Load with deserialization as Post object");
			
			tests.Run(500);
			Console.ReadLine();
		}

		private static void _ZooDemo()
		{
			var jb = new EJDB("zoo", EJDB.DEFAULT_OPEN_MODE | EJDB.JBOTRUNC);
			jb.ThrowExceptionOnFail = true;

			BsonDocument parrot1 = BsonDocument.ValueOf(new
			{
				name = "Grenny",
				type = "African Grey",
				male = true,
				age = 1,
				birthdate = DateTime.Now,
				likes = new[] {"green color", "night", "toys"},
				extra = BsonNull.VALUE
			});

			BsonDocument parrot2 = BsonDocument.ValueOf(new
				{
					name = "Bounty",
					type = "Cockatoo",
					male = false,
					age = 15,
					birthdate = DateTime.Now,
					likes = new[] {"sugar cane"}
				});

			jb.Save("parrots", parrot1, parrot2);

			Console.WriteLine("Grenny OID: " + parrot1[BsonConstants.Id]);
			Console.WriteLine("Bounty OID: " + parrot2[BsonConstants.Id]);

			EJDBQuery q = jb.CreateQuery(new
				{
					likes = "toys"
				}, "parrots").OrderBy("name");

			using (EJDBQCursor cur = q.Find())
			{
				Console.WriteLine("Found " + cur.Length + " parrots");
				foreach (BsonIterator e in cur)
				{
					//fetch  the `name` and the first element of likes array from the current BSON iterator.
					//alternatively you can fetch whole document from the iterator: `e.ToBsonDocument()`
					BsonDocument rdoc = e.ToBsonDocument("name", "likes.0");
					Console.WriteLine(string.Format("{0} likes the '{1}'", rdoc["name"], rdoc["likes.0"]));
				}
			}
			q.Dispose();
			jb.Dispose();

			Console.ReadKey();
		}
	}

	public class Post
	{
		public BsonOid _id { get; set; }
		public string Text { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime LastChangeDate { get; set; }
	}
}