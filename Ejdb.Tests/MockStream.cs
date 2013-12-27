using System.IO;

namespace Ejdb.Tests
{
    public class MockStream : MemoryStream
    {
        private readonly bool _canSeek;

        public MockStream(bool canSeek)
        {
            _canSeek = canSeek;
        }

        public override bool CanSeek
        {
            get { return _canSeek; }
        }
    }
}