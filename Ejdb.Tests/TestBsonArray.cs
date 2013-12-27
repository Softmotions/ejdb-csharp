using System;
using Ejdb.BSON;
using NUnit.Framework;

namespace Ejdb.Tests
{
    [TestFixture]
    public class TestBsonArray
    {
         [Test]
         public void TestAdd()
         {
             var array = new BsonArray();
             var value = BsonValue.Create(1);
             array.Add(value);
             Assert.AreEqual(1, array.Count);
             Assert.AreEqual(value.Value, array[0]);
         }

         [Test]
         public void TestAddNull()
         {
             var array = new BsonArray();
             array.Add(null);
             Assert.AreEqual(0, array.Count);
         }


         [Test]
         public void TestAddRangeNull()
         {
             var array = new BsonArray();
             array.AddRange(null);
             Assert.AreEqual(0, array.Count);
         }

         [Test]
         public void TestClone()
         {
             var array = new BsonArray();
             array.Add(1);
             array.Add(2);
             array.Add(new BsonArray());
             array.Add(3);
             array.Add(4);

             var clone = new BsonArray((BsonDocument) array.Clone());
             Assert.AreEqual(5, clone.Count);
             Assert.AreEqual(1, (int)clone[0]);
             Assert.AreEqual(2, (int)clone[1]);
             

             Assert.AreEqual(array[2], clone[2]);
             
             Assert.AreEqual(3, (int)clone[3]);
             Assert.AreEqual(4, (int)clone[4]);
         }


         [Test]
         public void TestClear()
         {
             var array = new BsonArray { 1, 2 };
             Assert.AreEqual(2, array.Count);
             array.Clear();
             Assert.AreEqual(0, array.Count);
         }

         [Test]
         public void TestCreateObjectArray()
         {
             var values = new object[] { true, 1, 1.5, null }; // null will be mapped to BsonNull.Value
             var array = new BsonArray(values);
             Assert.AreEqual(4, array.Count);
             Assert.AreEqual(true, array[0]);
             Assert.AreEqual(1, array[1]);
             Assert.AreEqual(1.5, array[2]);
             Assert.AreEqual(null, array[3]);
         }

         [Test]
         public void TestCreateObjectIdArray()
         {
             var value1 = new BsonOid(Guid.NewGuid().ToByteArray());
             var value2 = new BsonOid(Guid.NewGuid().ToByteArray());
             var values = new[] { value1, value2 };
             var array = new BsonArray(values);
             Assert.AreEqual(2, array.Count);
             Assert.AreEqual(value1, array[0]);
             Assert.AreEqual(value2, array[1]);
         }

         [Test]
         public void TestCreateStringArray()
         {
             var values = new string[] { "a", "b", null }; // null will be mapped to BsonNull.Value
             var array = new BsonArray(values);
             Assert.AreEqual(3, array.Count);
             Assert.AreEqual("a", array[0]);
             Assert.AreEqual("b", array[1]);
             Assert.AreEqual(null, array[2]);
         }

         [Test]
         public void TestEquals()
         {
             var a = new BsonArray { 1, 2 };
             var b = new BsonArray { 1, 2 };
             var c = new BsonArray { 3, 4 };
             Assert.IsTrue(a.Equals((object)a));
             Assert.IsTrue(a.Equals((object)b));
             Assert.IsFalse(a.Equals((object)c));
             Assert.IsFalse(a.Equals((object)null));
             Assert.IsFalse(a.Equals((object)1)); // types don't match
         }

         [Test]
         public void TestGetHashCode()
         {
             var a = new BsonArray { 1, 2 };
             var hashCode = a.GetHashCode();
             Assert.AreEqual(hashCode, a.GetHashCode());
         }

         [Test]
         public void TestIndexer()
         {
             var array = new BsonArray { 1 };
             Assert.AreEqual(1, array[0]);
         }


    }
}