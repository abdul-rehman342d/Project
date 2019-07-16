using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;

namespace SyncApi
{
    public class DeflateCompressor : Compressor
    {
        private const string DefleteEncoding = "deflate";

        public override string EncodingType
        {
            get { return DefleteEncoding; }
        }

        public override Stream CreateCompressionStream(Stream output)
        {
            return new DeflateStream(output, CompressionMode.Compress);
        }

        public override Stream CreateDecompressionStream(Stream input)
        {
            return new DeflateStream(input, CompressionMode.Decompress);
        }
    }
}