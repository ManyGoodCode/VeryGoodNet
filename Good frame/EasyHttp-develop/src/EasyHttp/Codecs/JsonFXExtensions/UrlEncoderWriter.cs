using System.Collections.Generic;
using EasyHttp.Http;
using JsonFx.Model;
using JsonFx.Serialization;

namespace EasyHttp.Codecs.JsonFXExtensions
{
    public class UrlEncoderWriter: ModelWriter 
    {
        readonly string[] contentTypes;

        public UrlEncoderWriter(DataWriterSettings settings, params string[] contentTypes) : base(settings)
        {
            this.contentTypes = contentTypes;
        }

        protected override ITextFormatter<ModelTokenType> GetFormatter()
        {
            return new UrlEncoderTextFormatter();
        }

        public override IEnumerable<string> ContentType
        {
            get
            {
				if (contentTypes != null)
				{
					foreach (string contentType in contentTypes)
					{
						yield return contentType;
					}

					yield break;
				}


                yield return HttpContentTypes.ApplicationXWwwFormUrlEncoded;
            }
        }

        public override IEnumerable<string> FileExtension
        {
            get { return new List<string>();  }
        }
    }
}