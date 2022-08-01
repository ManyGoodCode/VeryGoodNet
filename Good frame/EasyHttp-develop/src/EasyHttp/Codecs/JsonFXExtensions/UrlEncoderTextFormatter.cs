using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using JsonFx.Model;
using JsonFx.Serialization;

namespace EasyHttp.Codecs.JsonFXExtensions
{
    using System.Globalization;

    public class UrlEncoderTextFormatter : ITextFormatter<ModelTokenType>
    {
        public void Format(IEnumerable<Token<ModelTokenType>> tokens, TextWriter writer)
        {
            bool firstProperty = true;
            foreach (Token<ModelTokenType> token in tokens)
            {
                switch (token.TokenType)
                {
                    case ModelTokenType.None:
                        break;
                    case ModelTokenType.ObjectBegin:
                        break;
                    case ModelTokenType.ObjectEnd:
                        break;
                    case ModelTokenType.ArrayBegin:
                        break;
                    case ModelTokenType.ArrayEnd:
                        break;
                    case ModelTokenType.Property:
                        if (!firstProperty)
                        {
                            writer.Write("&");
                        }
                        firstProperty = false;
                        writer.Write(token.Name);
                        continue;
                    case ModelTokenType.Primitive:
                        if (token.Value != null)
                        {
                            string urlEncode = HttpUtility.UrlEncode(token.Value.ToString());
                            writer.Write("={0}", urlEncode);
                        }
                        else
                        {
                            writer.Write("=");
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public string Format(IEnumerable<Token<ModelTokenType>> tokens)
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                Format(tokens, writer);
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}