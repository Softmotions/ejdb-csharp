using System.IO;

namespace Ejdb.Tests
{
    public class StreamWithoutSeek : MemoryStream
    {
        public override bool CanSeek
        {
            get { return false; }
        }
    }
}