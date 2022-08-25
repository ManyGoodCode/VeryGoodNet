using System;
using System.Collections.Generic;
using System.IO;

namespace SharpDX.Multimedia
{
    public class SoundStream : Stream
    {
        private Stream input;
        private long startPositionOfData;
        private long length;

        public SoundStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            input = stream;
            Initialize(stream);
        }

        private unsafe void Initialize(Stream stream)
        {
            RiffParser parser = new RiffParser(stream);

            FileFormatName = "Unknown";

            // Parse Header
            if (!parser.MoveNext() || parser.Current == null)
            {
                ThrowInvalidFileFormat();
                return;
            }

            // Check that WAVE or XWMA header is present
            FileFormatName = parser.Current.Type;
            if (FileFormatName != "WAVE" && FileFormatName != "XWMA")
                throw new InvalidOperationException("Unsupported " + FileFormatName + " file format. Only WAVE or XWMA");

            // Parse inside the first chunk
            parser.Descend();

            // Get all the chunk
            var chunks = parser.GetAllChunks();

            // Get "fmt" chunk
            var fmtChunk = Chunk(chunks, "fmt ");
            if (fmtChunk.Size < sizeof(WaveFormat.__PcmNative))
                ThrowInvalidFileFormat();

            try
            {
                Format = WaveFormat.MarshalFrom(fmtChunk.GetData());
            }
            catch (InvalidOperationException ex)
            {
                ThrowInvalidFileFormat(ex);
            }

            if (FileFormatName == "XWMA")
            {
                if (Format.Encoding != WaveFormatEncoding.Wmaudio2 && Format.Encoding != WaveFormatEncoding.Wmaudio3)
                    ThrowInvalidFileFormat();

                var dpdsChunk = Chunk(chunks, "dpds");
                DecodedPacketsInfo = dpdsChunk.GetDataAsArray<uint>();
            }
            else
            {
                switch (Format.Encoding)
                {
                    case WaveFormatEncoding.Pcm:
                    case WaveFormatEncoding.IeeeFloat:
                    case WaveFormatEncoding.Extensible:
                    case WaveFormatEncoding.Adpcm:
                        break;
                    default:
                        ThrowInvalidFileFormat();
                        break;
                }
            }

            var dataChunk = Chunk(chunks, "data");
            startPositionOfData = dataChunk.DataPosition;
            length = dataChunk.Size;

            input.Position = startPositionOfData;
        }

        protected void ThrowInvalidFileFormat(Exception nestedException = null)
        {
            throw new InvalidOperationException("Invalid " + FileFormatName + " file format", nestedException);
        }

        public uint[] DecodedPacketsInfo { get; private set; }
        public WaveFormat Format { get; protected set; }
        public DataStream ToDataStream()
        {
            var buffer = new byte[Length];
            if (Read(buffer, 0, (int)Length) != Length)
                throw new InvalidOperationException("Unable to get a valid DataStream");

            return DataStream.Create(buffer, true, true);
        }

        public static implicit operator DataStream(SoundStream stream)
        {
            return stream.ToDataStream();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Position
        {
            get
            {
                return input.Position - startPositionOfData;
            }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (input != null)
            {
                input.Dispose();
                input = null;
            }

            base.Dispose(disposing);
        }

        protected RiffChunk Chunk(IEnumerable<RiffChunk> chunks, string id)
        {
            RiffChunk chunk = null;
            foreach (var riffChunk in chunks)
            {
                if (riffChunk.Type == id)
                {
                    chunk = riffChunk;
                    break;
                }
            }
            if (chunk == null || chunk.Type != id)
                throw new InvalidOperationException("Invalid " + FileFormatName + " file format");
            return chunk;
        }

        private string FileFormatName { get; set; }
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var newPosition = input.Position;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    newPosition = startPositionOfData + offset;
                    break;
                case SeekOrigin.Current:
                    newPosition = input.Position + offset;
                    break;
                case SeekOrigin.End:
                    newPosition = startPositionOfData + length + offset;
                    break;
            }

            if (newPosition < startPositionOfData || newPosition > (startPositionOfData + length))
                throw new InvalidOperationException("Cannot seek outside the range of this stream");

            return input.Seek(newPosition, SeekOrigin.Begin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return input.Read(buffer, offset, Math.Min(count, (int)Math.Max(startPositionOfData + length - input.Position, 0)));
        }

        public override long Length
        {
            get { return length; }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}