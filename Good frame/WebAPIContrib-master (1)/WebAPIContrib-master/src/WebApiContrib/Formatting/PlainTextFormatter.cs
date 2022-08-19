using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace WebApiContrib.Formatting
{
    public class PlainTextFormatter : System.Net.Http.Formatting.MediaTypeFormatter
    {
        private readonly Encoding encoding;

        public PlainTextFormatter(Encoding encoding = null)
        {
            this.encoding = encoding ?? Encoding.Default;
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }

    	public override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }

    	public override bool CanWriteType(Type type)
        {
            return type == typeof(string);
        }

    	public override Task<object> ReadFromStreamAsync(
            Type type, 
            Stream stream,
            HttpContent content, 
            IFormatterLogger formatterLogger)
        {
            StreamReader reader = new StreamReader(stream, encoding);
            string value = reader.ReadToEnd();

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            tcs.SetResult(value);
            return tcs.Task;
        }

    	public override Task WriteToStreamAsync(
            Type type, 
            object value, 
            Stream stream, 
            HttpContent content, 
            TransportContext transportContext)
        {
            StreamWriter writer = new StreamWriter(stream, encoding);
            writer.Write((string) value);
            writer.Flush();

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }
    }
}
