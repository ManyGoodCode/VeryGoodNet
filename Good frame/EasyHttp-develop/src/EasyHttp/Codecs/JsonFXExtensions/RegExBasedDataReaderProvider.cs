using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JsonFx.Serialization;
using JsonFx.Serialization.Providers;

namespace EasyHttp.Codecs.JsonFXExtensions
{
    public class RegExBasedDataReaderProvider : IDataReaderProvider
    {
        readonly IDictionary<string, IDataReader> readersByMime = new Dictionary<string, IDataReader>(StringComparer.OrdinalIgnoreCase);

        public RegExBasedDataReaderProvider(IEnumerable<IDataReader> dataReaders)
        {
            if (dataReaders == null)
                return;
            foreach (IDataReader reader in dataReaders)
            {
                foreach (string contentType in reader.ContentType)
                {
                    if (string.IsNullOrEmpty(contentType) ||
                        readersByMime.ContainsKey(contentType))
                    {
                        continue;
                    }

                    readersByMime[contentType] = reader;
                }
            }

        }

        public IDataReader Find(string contentTypeHeader)
        {
            string type = DataProviderUtility.ParseMediaType(contentTypeHeader);
            var readers = readersByMime.Where(reader => Regex.Match(type, reader.Key, RegexOptions.Singleline).Success);

            return readers.Any() ? readers.First().Value : null;
        }
    }
}