using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using VCDiff.Decoders;
using VCDiff.Encoders;
using VCDiff.Includes;
using Xunit;

namespace VCDiff.Tests
{
    public class FileDiffTests
    {
        [Fact]
        public void NoChecksumNoInterleaved_Test()
        {
            using var srcStream = File.OpenRead("a.test");
            using var targetStream = File.OpenRead("b.test");
            using var deltaStream = new MemoryStream();
            using var outputStream = new MemoryStream();
            using VcEncoder coder = new VcEncoder(srcStream, targetStream, deltaStream);
            VCDiffResult result = coder.Encode(); //encodes with no checksum and not interleaved
            Assert.Equal(VCDiffResult.SUCCESS, result);

            srcStream.Position = 0;
            targetStream.Position = 0;
            deltaStream.Position = 0;

            using VcDecoder decoder = new VcDecoder(srcStream, deltaStream, outputStream);
            Assert.Equal(VCDiffResult.SUCCESS, decoder.Decode(out long bytesWritten));
            Assert.NotEqual(0, bytesWritten);
        }

        [Fact]
        public void NoChecksumEmptyHash_Test()
        {
            using var srcStream = File.OpenRead("empty.test");
            using var targetStream = File.OpenRead("b.test");
            using var md5 = MD5.Create();
            var originalHash = md5.ComputeHash(targetStream);
            targetStream.Position = 0;

            using var deltaStream = new MemoryStream();
            using var outputStream = new MemoryStream();
            using VcEncoder coder = new VcEncoder(srcStream, targetStream, deltaStream);
            VCDiffResult result = coder.Encode(checksum: false); 
            Assert.Equal(VCDiffResult.SUCCESS, result);

            srcStream.Position = 0;
            targetStream.Position = 0;
            deltaStream.Position = 0;

            using VcDecoder decoder = new VcDecoder(srcStream, deltaStream, outputStream);
            Assert.Equal(VCDiffResult.SUCCESS, decoder.Decode(out long bytesWritten));
            outputStream.Position = 0;
            var outputHash = md5.ComputeHash(outputStream);
            Assert.Equal(originalHash, outputHash);
        }

        [Fact]
        public void NoChecksumGoogle_Test()
        {
            using var srcStream = File.OpenRead("size-overflow-32");
            using var targetStream = File.OpenRead("size-overflow-64");
            using var md5 = MD5.Create();
            var originalHash = md5.ComputeHash(targetStream);
            targetStream.Position = 0;

            using var deltaStream = new MemoryStream();
            using var outputStream = new MemoryStream();
            using VcEncoder coder = new VcEncoder(srcStream, targetStream, deltaStream);
            VCDiffResult result = coder.Encode(checksum: false);
            Assert.Equal(VCDiffResult.SUCCESS, result);

            srcStream.Position = 0;
            targetStream.Position = 0;
            deltaStream.Position = 0;

            using VcDecoder decoder = new VcDecoder(srcStream, deltaStream, outputStream);
            Assert.Equal(VCDiffResult.SUCCESS, decoder.Decode(out long bytesWritten));
            outputStream.Position = 0;
            var outputHash = md5.ComputeHash(outputStream);
            Assert.Equal(originalHash, outputHash);
        }

        [Fact]
        public void NoChecksumGoogleTo_Test()
        {
            using var srcStream = File.OpenRead("size-overflow-64");
            using var targetStream = File.OpenRead("size-overflow-32");
            using var md5 = MD5.Create();
            var originalHash = md5.ComputeHash(targetStream);
            targetStream.Position = 0;

            using var deltaStream = new MemoryStream();
            using var outputStream = new MemoryStream();
            using VcEncoder coder = new VcEncoder(srcStream, targetStream, deltaStream);
            VCDiffResult result = coder.Encode(checksum: false);
            Assert.Equal(VCDiffResult.SUCCESS, result);

            srcStream.Position = 0;
            targetStream.Position = 0;
            deltaStream.Position = 0;

            using VcDecoder decoder = new VcDecoder(srcStream, deltaStream, outputStream);
            Assert.Equal(VCDiffResult.SUCCESS, decoder.Decode(out long bytesWritten));
            outputStream.Position = 0;
            var outputHash = md5.ComputeHash(outputStream);
            Assert.Equal(originalHash, outputHash);
        }

        [Fact]
        public void NoChecksumGoogleSame_Test()
        {
            using var srcStream = File.OpenRead("size-overflow-64");
            using var targetStream = File.OpenRead("size-overflow-64");
            using var md5 = MD5.Create();
            var originalHash = md5.ComputeHash(targetStream);
            targetStream.Position = 0;

            using var deltaStream = new MemoryStream();
            using var outputStream = new MemoryStream();
            using VcEncoder coder = new VcEncoder(srcStream, targetStream, deltaStream);
            VCDiffResult result = coder.Encode(checksum: false);
            Assert.Equal(VCDiffResult.SUCCESS, result);

            srcStream.Position = 0;
            targetStream.Position = 0;
            deltaStream.Position = 0;

            using VcDecoder decoder = new VcDecoder(srcStream, deltaStream, outputStream);
            Assert.Equal(VCDiffResult.SUCCESS, decoder.Decode(out long bytesWritten));
            outputStream.Position = 0;
            var outputHash = md5.ComputeHash(outputStream);
            Assert.Equal(originalHash, outputHash);
        }

        [Fact]
        public void NoChecksumEmptyToHash_Test()
        {
            using var srcStream = File.OpenRead("b.test");
            using var targetStream = File.OpenRead("empty.test");
            using var md5 = MD5.Create();
            var originalHash = md5.ComputeHash(targetStream);
            targetStream.Position = 0;

            using var deltaStream = new MemoryStream();
            using var outputStream = new MemoryStream();
            using VcEncoder coder = new VcEncoder(srcStream, targetStream, deltaStream);
            VCDiffResult result = coder.Encode(checksum: false);
            Assert.Equal(VCDiffResult.SUCCESS, result);

            srcStream.Position = 0;
            targetStream.Position = 0;
            deltaStream.Position = 0;

            using VcDecoder decoder = new VcDecoder(srcStream, deltaStream, outputStream);
            Assert.Equal(VCDiffResult.SUCCESS, decoder.Decode(out long bytesWritten));
            outputStream.Position = 0;
            var outputHash = md5.ComputeHash(outputStream);
            Assert.Equal(originalHash, outputHash);
        }

        [Fact]
        public void ChecksumEmptyHash_Test()
        {
            using var srcStream = File.OpenRead("empty.test");
            using var targetStream = File.OpenRead("b.test");
            using var md5 = MD5.Create();
            var originalHash = md5.ComputeHash(targetStream);
            targetStream.Position = 0;

            using var deltaStream = new MemoryStream();
            using var outputStream = new MemoryStream();
            using VcEncoder coder = new VcEncoder(srcStream, targetStream, deltaStream);
            VCDiffResult result = coder.Encode(checksum: true);
            Assert.Equal(VCDiffResult.SUCCESS, result);

            srcStream.Position = 0;
            targetStream.Position = 0;
            deltaStream.Position = 0;

            using VcDecoder decoder = new VcDecoder(srcStream, deltaStream, outputStream);
            Assert.Equal(VCDiffResult.SUCCESS, decoder.Decode(out long bytesWritten));
            outputStream.Position = 0;
            var outputHash = md5.ComputeHash(outputStream);
            Assert.Equal(originalHash, outputHash);
        }



        [Fact]
        public void Checksum_Test()
        {
            using var srcStream = File.OpenRead("a.test");
            using var targetStream = File.OpenRead("b.test");
            using var deltaStream = new MemoryStream();
            using var outputStream = new MemoryStream();
            using VcEncoder coder = new VcEncoder(srcStream, targetStream, deltaStream);
            VCDiffResult result = coder.Encode(checksum: true); //encodes with no checksum and not interleaved
            Assert.Equal(VCDiffResult.SUCCESS, result);

            srcStream.Position = 0;
            targetStream.Position = 0;
            deltaStream.Position = 0;

            using VcDecoder decoder = new VcDecoder(srcStream, deltaStream, outputStream);
            Assert.Equal(VCDiffResult.SUCCESS, decoder.Decode(out long bytesWritten));
        }

        [Fact]
        public void ChecksumHash_Test()
        {
            using var srcStream = File.OpenRead("a.test");
            using var targetStream = File.OpenRead("b.test");
            using var md5 = MD5.Create();
            var originalHash = md5.ComputeHash(targetStream);
            targetStream.Position = 0;

            using var deltaStream = new MemoryStream();
            using var outputStream = new MemoryStream();
            using VcEncoder coder = new VcEncoder(srcStream, targetStream, deltaStream);
            VCDiffResult result = coder.Encode(checksum: true); //encodes with no checksum and not interleaved
            Assert.Equal(VCDiffResult.SUCCESS, result);

            srcStream.Position = 0;
            targetStream.Position = 0;
            deltaStream.Position = 0;

            using VcDecoder decoder = new VcDecoder(srcStream, deltaStream, outputStream);
            Assert.Equal(VCDiffResult.SUCCESS, decoder.Decode(out long bytesWritten));
            outputStream.Position = 0;
            var outputHash = md5.ComputeHash(outputStream);
            Assert.Equal(originalHash, outputHash);
            File.WriteAllBytes("patch1.new", deltaStream.ToArray());
        }


        [Fact]
        public void NoChecksumHash_Test()
        {
            using var srcStream = File.OpenRead("a.test");
            using var targetStream = File.OpenRead("b.test");
            using var md5 = MD5.Create();
            var originalHash = md5.ComputeHash(targetStream);
            targetStream.Position = 0;

            using var deltaStream = new MemoryStream();
            using var outputStream = new MemoryStream();
            using VcEncoder coder = new VcEncoder(srcStream, targetStream, deltaStream);
            VCDiffResult result = coder.Encode(checksum: false); //encodes with no checksum and not interleaved
            Assert.Equal(VCDiffResult.SUCCESS, result);

            srcStream.Position = 0;
            targetStream.Position = 0;
            deltaStream.Position = 0;

            using VcDecoder decoder = new VcDecoder(srcStream, deltaStream, outputStream);
            Assert.Equal(VCDiffResult.SUCCESS, decoder.Decode(out long bytesWritten));
            outputStream.Position = 0;
            var outputHash = md5.ComputeHash(outputStream);
            Assert.Equal(originalHash, outputHash);
        }

        [Fact]
        public void ChecksumHashSmall_block_Test()
        {
            using var srcStream = File.OpenRead("a.test");
            using var targetStream = File.OpenRead("b.test");
            using var md5 = MD5.Create();
            var originalHash = md5.ComputeHash(targetStream);
            targetStream.Position = 0;

            using var deltaStream = new MemoryStream();
            using var outputStream = new MemoryStream();
            using VcEncoder coder = new VcEncoder(srcStream, targetStream, deltaStream, blockSize: 8);
            VCDiffResult result = coder.Encode(checksum: true); //encodes with no checksum and not interleaved
            Assert.Equal(VCDiffResult.SUCCESS, result);

            srcStream.Position = 0;
            targetStream.Position = 0;
            deltaStream.Position = 0;

            using VcDecoder decoder = new VcDecoder(srcStream, deltaStream, outputStream);
            Assert.Equal(VCDiffResult.SUCCESS, decoder.Decode(out long bytesWritten));
            outputStream.Position = 0;
            var outputHash = md5.ComputeHash(outputStream);
            Assert.Equal(originalHash, outputHash);
        }

        [Fact]
        public void ChecksumHashLarge_block_Test()
        {
            using var srcStream = File.OpenRead("a.test");
            using var targetStream = File.OpenRead("b.test");
            using var md5 = MD5.Create();
            var originalHash = md5.ComputeHash(targetStream);
            targetStream.Position = 0;

            using var deltaStream = new MemoryStream();
            using var outputStream = new MemoryStream();
            using VcEncoder coder = new VcEncoder(srcStream, targetStream, deltaStream, blockSize: 32);
            VCDiffResult result = coder.Encode(checksum: true); //encodes with no checksum and not interleaved
            Assert.Equal(VCDiffResult.SUCCESS, result);

            srcStream.Position = 0;
            targetStream.Position = 0;
            deltaStream.Position = 0;

            using VcDecoder decoder = new VcDecoder(srcStream, deltaStream, outputStream);
            Assert.Equal(VCDiffResult.SUCCESS, decoder.Decode(out long bytesWritten));
            outputStream.Position = 0;
            var outputHash = md5.ComputeHash(outputStream);
            Assert.Equal(originalHash, outputHash);
        }


        [Fact]
        public void Interleaved_Test()
        {
            using var srcStream = File.OpenRead("a.test");
            using var targetStream = File.OpenRead("b.test");
            using var deltaStream = new MemoryStream();
            using var outputStream = new MemoryStream();
            using var md5 = MD5.Create();
            var originalHash = md5.ComputeHash(targetStream);
            targetStream.Position = 0;

            using VcEncoder coder = new VcEncoder(srcStream, targetStream, deltaStream);
            VCDiffResult result = coder.Encode(interleaved: true); //encodes with no checksum and not interleaved
            Assert.Equal(VCDiffResult.SUCCESS, result);

            srcStream.Position = 0;
            targetStream.Position = 0;
            deltaStream.Position = 0;

            using VcDecoder decoder = new VcDecoder(srcStream, deltaStream, outputStream);

            long bytesWritten = 0;

            while (bytesWritten < targetStream.Length)
            {
                Assert.Equal(VCDiffResult.SUCCESS, decoder.Decode(out long chunk));
                bytesWritten += chunk;
            }

            outputStream.Position = 0;
            var outputHash = md5.ComputeHash(outputStream);
            Assert.Equal(originalHash, outputHash);
        }
    }
}
