using System;
using System.IO;
using System.Linq;
using System.Text;
using Ejdb.BSON;
using Ejdb.IO;
using NUnit.Framework;

namespace Ejdb.Tests
{
    [TestFixture]
    public class TestBsonStringSerialization
    {
        [Test]
        public void ReadCStringEmpty()
        {
            var bytes = new byte[] { 8, 0, 0, 0, (byte)BsonType.BOOL, 0, 0, 0 };
            Assert.AreEqual(8, bytes.Length);

            var document = new BsonDocument(bytes);
            Assert.AreEqual("", document.First().Key);
        }

        [Test]
        public void TestReadCStringOneCharacter()
        {
            var bytes = new byte[] { 9, 0, 0, 0, (byte)BsonType.BOOL, (byte)'b', 0, 0, 0 };
            Assert.AreEqual(9, bytes.Length);
            var document = new BsonDocument(bytes);
            Assert.AreEqual("b", document.First().Key);
        }

        [Test]
        public void TestReadCStringOneCharacterDecoderException()
        {
            var bytes = new byte[] { 9, 0, 0, 0, (byte)BsonType.BOOL, 0x80, 0, 0, 0 };
            Assert.AreEqual(9, bytes.Length);
            Assert.Throws<DecoderFallbackException>(() => new BsonDocument(bytes));
        }

        [Test]
        public void TestReadCStringTwoCharacters()
        {
            var bytes = new byte[] { 10, 0, 0, 0, (byte)BsonType.BOOL, (byte)'b', (byte)'b', 0, 0, 0 };
            Assert.AreEqual(10, bytes.Length);
            var document = new BsonDocument(bytes);
            Assert.AreEqual("bb", document.First().Key);
        }

        [Test]
        public void TestReadCStringTwoCharactersDecoderException()
        {
            var bytes = new byte[] { 10, 0, 0, 0, (byte)BsonType.BOOL, (byte)'b', 0x80, 0, 0, 0 };
            Assert.AreEqual(10, bytes.Length);
            var ex = Assert.Throws<DecoderFallbackException>(() => { new BsonDocument(bytes); });
        }

        [Test]
        public void TestReadStringEmpty()
        {
            var bytes = new byte[] { 13, 0, 0, 0, (byte)BsonType.STRING, (byte)'s', 0, 1, 0, 0, 0, 0, 0 };
            Assert.AreEqual(13, bytes.Length);
            var document = new BsonDocument(bytes);
            Assert.AreEqual("", document["s"]);
        }

        [Test]
        public void TestReadStringInvalidLength()
        {
            var bytes = new byte[] { 13, 0, 0, 0, (byte)BsonType.STRING, (byte)'s', 0, 0, 0, 0, 0, 0, 0 };
            Assert.AreEqual(13, bytes.Length);
            var ex = Assert.Throws<BsonSerializationException>(() => { new BsonDocument(bytes); });
            Assert.AreEqual("Invalid string length: 0 (the length includes the null terminator so it must be greater than or equal to 1).", ex.Message);
        }

        [Test]
        public void TestReadStringMissingNullTerminator()
        {
            var bytes = new byte[] { 13, 0, 0, 0, (byte)BsonType.STRING, (byte)'s', 0, 1, 0, 0, 0, 123, 0 };
            Assert.AreEqual(13, bytes.Length);
            var ex = Assert.Throws<BsonSerializationException>(() => { new BsonDocument(bytes); });
            Assert.AreEqual("String is missing null terminator.", ex.Message);
        }

        [Test]
        public void TestReadStringOneCharacter()
        {
            var bytes = new byte[] { 14, 0, 0, 0, (byte)BsonType.STRING, (byte)'s', 0, 2, 0, 0, 0, (byte)'x', 0, 0 };
            Assert.AreEqual(14, bytes.Length);
            var document = new BsonDocument(bytes);
            Assert.AreEqual("x", document["s"]);
        }

        [Test]
        public void TestReadStringOneCharacterDecoderException()
        {
            var bytes = new byte[] { 14, 0, 0, 0, (byte)BsonType.STRING, (byte)'s', 0, 2, 0, 0, 0, 0x80, 0, 0 };
            Assert.AreEqual(14, bytes.Length);
            var ex = Assert.Throws<DecoderFallbackException>(() => { new BsonDocument(bytes); });
        }

        [Test]
        public void TestReadStringTwoCharacters()
        {
            var bytes = new byte[] { 15, 0, 0, 0, (byte)BsonType.STRING, (byte)'s', 0, 3, 0, 0, 0, (byte)'x', (byte)'y', 0, 0 };
            Assert.AreEqual(15, bytes.Length);
            var document = new BsonDocument(bytes);
            Assert.AreEqual("xy", document["s"]);
        }

        [Test]
        public void TestReadStringTwoCharactersDecoderException()
        {
            var bytes = new byte[] { 15, 0, 0, 0, (byte)BsonType.STRING, (byte)'s', 0, 3, 0, 0, 0, (byte)'x', 0x80, 0, 0 };
            Assert.AreEqual(15, bytes.Length);
            var ex = Assert.Throws<DecoderFallbackException>(() => { new BsonDocument(bytes); });
        }

        [Test]
        public void TestWriteCStringThrowsWhenValueContainsNulls()
        {
            var writer = new ExtBinaryWriter(new MemoryStream());
            Assert.Throws<ArgumentException>(() => writer.WriteCString("a\0b"));
        }

        [Test]
        public void TestWriteCStringThrowsWhenValueIsNull()
        {
            var writer = new ExtBinaryWriter(new MemoryStream());
            Assert.Throws<ArgumentNullException>(() => writer.WriteCString(null));
        }
    }
}