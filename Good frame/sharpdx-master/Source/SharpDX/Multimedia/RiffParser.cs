using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SharpDX.Multimedia
{
    public class RiffParser : IEnumerator<RiffChunk>, IEnumerable<RiffChunk>
    {
        private readonly Stream input;
        private readonly long startPosition;
        private readonly BinaryReader reader;
        private readonly Stack<RiffChunk> chunckStack;
        private bool descendNext;
        private bool isEndOfRiff;
        private bool isErrorState;
        private RiffChunk current;

        public RiffParser(Stream input)
        {
            this.input = input;
            this.startPosition = input.Position;
            this.reader = new BinaryReader(input);
            this.chunckStack = new Stack<RiffChunk>();
        }


        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            CheckState();

            if (current != null)
            {
                long nextOffset = current.DataPosition;
                if (descendNext)
                {
                    descendNext = false;
                } else
                {
                    nextOffset += current.Size;
                    if ((nextOffset & 1) != 0)
                        nextOffset++;
                }
                input.Position = nextOffset;
                var currentChunkContainer = chunckStack.Peek();
                long endOfOuterChunk = currentChunkContainer.DataPosition + currentChunkContainer.Size;
                if (input.Position >= endOfOuterChunk)
                    chunckStack.Pop();
                if (chunckStack.Count == 0)
                {
                    isEndOfRiff = true;
                    return false;
                }
            }

            var fourCC = ((FourCC) reader.ReadUInt32());
            bool isList = (fourCC == "LIST");
            bool isHeader = (fourCC == "RIFF");
            uint chunkSize = 0;

            if (input.Position == (startPosition+4) && !isHeader)
            {
                isErrorState = true;
                throw new InvalidOperationException("Invalid RIFF file format");
            }

            chunkSize = reader.ReadUInt32();
            if (isList || isHeader)
            {
                if (isHeader && chunkSize > (input.Length - 8))
                {
                    isErrorState = true;
                    throw new InvalidOperationException("Invalid RIFF file format");
                }
                chunkSize -= 4;
                fourCC = reader.ReadUInt32();
            }

            current = new RiffChunk(input, fourCC, chunkSize, (uint)input.Position, isList, isHeader);
            return true;
        }

        private void CheckState()
        {
            if (isEndOfRiff)
                throw new InvalidOperationException("End of Riff. Cannot MoveNext");

            if (isErrorState)
                throw new InvalidOperationException("The enumerator is in an error state");
        }

        public Stack<RiffChunk> ChunkStack { get { return chunckStack; } }

        public void Reset()
        {
            CheckState();
            current = null;
            input.Position = startPosition;
        }
 
        public void Ascend()
        {
            CheckState();
            var outerChunk = chunckStack.Pop();
            input.Position = outerChunk.DataPosition + outerChunk.Size;
        }

        public void Descend()
        {
            CheckState();
            chunckStack.Push(current);
            descendNext = true;
        }

        public IList<RiffChunk> GetAllChunks()
        {
            var chunks = new List<RiffChunk>();
            foreach (var riffChunk in this)
                chunks.Add(riffChunk);
            return chunks;
        }

        public RiffChunk Current { 
            get
            {
                CheckState();
                return current;
            }         
        }

        public IEnumerator<RiffChunk> GetEnumerator()
        {
            return this;
        }

        object IEnumerator.Current
        {
            get
            {
                CheckState();
                return Current;
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}