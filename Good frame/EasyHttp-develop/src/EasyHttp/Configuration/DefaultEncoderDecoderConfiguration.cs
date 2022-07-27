using System.Collections.Generic;
using EasyHttp.Codecs;
using EasyHttp.Codecs.JsonFXExtensions;
using JsonFx.Json;
using JsonFx.Json.Resolvers;
using JsonFx.Model.Filters;
using JsonFx.Serialization;
using JsonFx.Serialization.Resolvers;
using JsonFx.Xml;
using JsonFx.Xml.Resolvers;

namespace EasyHttp.Configuration
{
    public class DefaultEncoderDecoderConfiguration : IEncoderDecoderConfiguration
    {
        public IEncoder GetEncoder()
        {
            JsonWriter jsonWriter = new JsonWriter(new DataWriterSettings(CombinedResolverStrategy()),
                                            new[] { "application/.*json", "text/.*json" });
            XmlWriter xmlWriter = new XmlWriter(new DataWriterSettings(CombinedResolverStrategy()),
                                          new[] { "application/xml", "text/.*xhtml", "text/xml", "text/html" });

            UrlEncoderWriter urlEncoderWriter = new UrlEncoderWriter(new DataWriterSettings(CombinedResolverStrategy()),
                                                        new[] { "application/x-www-form-urlencoded" });

            List<IDataWriter> writers = new List<IDataWriter> { jsonWriter, xmlWriter, urlEncoderWriter };
            RegExBasedDataWriterProvider dataWriterProvider = new RegExBasedDataWriterProvider(writers);
            return new DefaultEncoder(dataWriterProvider);
        }

        public IDecoder GetDecoder()
        {
            JsonReader jsonReader = new JsonReader(new DataReaderSettings(CombinedResolverStrategy(), new Iso8601DateFilter()), new[] { "application/.*json", "text/.*json" });
            XmlReader xmlReader = new XmlReader(new DataReaderSettings(CombinedResolverStrategy(), new Iso8601DateFilter()),
                                          new[] { "application/.*xml", "text/.*xhtml", "text/xml", "text/html" });

            List<IDataReader> readers = new List<IDataReader> { jsonReader, xmlReader };
            RegExBasedDataReaderProvider dataReaderProvider = new RegExBasedDataReaderProvider(readers);
            return new DefaultDecoder(dataReaderProvider);
        }

        public static CombinedResolverStrategy CombinedResolverStrategy()
        {
            return new CombinedResolverStrategy(
                new JsonResolverStrategy(),
                new DataContractResolverStrategy(),
                new XmlResolverStrategy(),
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.PascalCase),
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.CamelCase),
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.NoChange),
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.NoChange, "_"),
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.NoChange, "-"),
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.Lowercase, "-"),
                new ConventionResolverStrategy(ConventionResolverStrategy.WordCasing.Uppercase, "_"));
        }
    }
}