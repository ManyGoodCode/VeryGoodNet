
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JsonFx.Serialization;
using JsonFx.Serialization.Providers;

namespace EasyHttp.Codecs.JsonFXExtensions
{
    public class RegExBasedDataWriterProvider : IDataWriterProvider
    {
        readonly IDataWriter defaultWriter;
        readonly IDictionary<string, IDataWriter> writersByExt = new Dictionary<string, IDataWriter>(StringComparer.OrdinalIgnoreCase);
        readonly IDictionary<string, IDataWriter> writersByMime = new Dictionary<string, IDataWriter>(StringComparer.OrdinalIgnoreCase);


        public RegExBasedDataWriterProvider(IEnumerable<IDataWriter> writers)
        {
            if (writers == null)
                return;
            foreach (IDataWriter writer in writers)
            {
                if (defaultWriter == null)
                    defaultWriter = writer;

                foreach (string contentType in writer.ContentType)
                {
                    if (string.IsNullOrEmpty(contentType) ||
                        writersByMime.ContainsKey(contentType))
                    {
                        continue;
                    }

                    writersByMime[contentType] = writer;
                }

                foreach (string fileExt in writer.FileExtension)
                {
                    if (string.IsNullOrEmpty(fileExt) ||
                        writersByExt.ContainsKey(fileExt))
                    {
                        continue;
                    }

                    string ext = NormalizeExtension(fileExt);
                    writersByExt[ext] = writer;
                }
            }
        }


        public IDataWriter DefaultDataWriter
        {
            get { return defaultWriter; }
        }


        public IDataWriter Find(string extension)
        {
            extension = NormalizeExtension(extension);
            IDataWriter writer;
            if (writersByExt.TryGetValue(extension, out writer))
            {
                return writer;
            }

            return null;
        }

        public IDataWriter Find(string acceptHeader, string contentTypeHeader)
        {
            foreach (string type in ParseHeaders(acceptHeader, contentTypeHeader))
            {
                IEnumerable<KeyValuePair<string, IDataWriter>> readers = from writer in writersByMime
                                                                         where Regex.Match(type, writer.Key, RegexOptions.Singleline).Success
                                                                         select writer;

                if (readers.Count() > 0)
                {
                    return readers.First().Value;
                }
            }

            return null;
        }


        public static IEnumerable<string> ParseHeaders(string accept, string contentType)
        {
            string mime;
            foreach (string type in SplitTrim(accept, ','))
            {
                mime = DataProviderUtility.ParseMediaType(type);
                if (!string.IsNullOrEmpty(mime))
                {
                    yield return mime;
                }
            }

            mime = DataProviderUtility.ParseMediaType(contentType);
            if (!string.IsNullOrEmpty(mime))
            {
                yield return mime;
            }
        }

        private static IEnumerable<string> SplitTrim(string source, char ch)
        {
            if (string.IsNullOrEmpty(source))
            {
                yield break;
            }

            int length = source.Length;
            for (int prev = 0, next = 0; prev < length && next >= 0; prev = next + 1)
            {
                next = source.IndexOf(ch, prev);
                if (next < 0)
                {
                    next = length;
                }

                string part = source.Substring(prev, next - prev).Trim();
                if (part.Length > 0)
                {
                    yield return part;
                }
            }
        }

        private static string NormalizeExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                return string.Empty;
            }

            return Path.GetExtension(extension);
        }
    }
}