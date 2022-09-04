using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EasyHttp.Infrastructure;

namespace EasyHttp.Http
{
    public class MultiPartStreamer
    {
        readonly string boundary;
        readonly string boundaryCode;
        readonly IList<FileData> multipartFileData;
        readonly IDictionary<string, object> multipartFormData;

        public MultiPartStreamer(IDictionary<string, object> multipartFormData, IList<FileData> multipartFileData)
        {
            boundaryCode = DateTime.Now.Ticks.GetHashCode() + "548130";
            boundary = string.Format("\r\n----------------{0}", boundaryCode);

            this.multipartFormData = multipartFormData;
            this.multipartFileData = multipartFileData;
        }

        public void StreamMultiPart(Stream stream)
        {
            stream.WriteString(boundary);
            if (multipartFormData != null)
            {
                foreach (KeyValuePair<string, object> entry in multipartFormData)
                {
                    stream.WriteString(CreateFormBoundaryHeader(entry.Key, entry.Value));
                    stream.WriteString(boundary);
                }
            }

            if (multipartFileData != null)
            {
                foreach (FileData fileData in multipartFileData)
                {
                    using (FileStream file = new FileStream(fileData.Filename, FileMode.Open))
                    {
                        stream.WriteString(CreateFileBoundaryHeader(fileData));
                        StreamFileContents(file, fileData, stream);
                        stream.WriteString(boundary);
                    }
                }
            }

            stream.WriteString("--");
        }

        static void StreamFileContents(Stream file, FileData fileData, Stream requestStream)
        {
            byte[] buffer = new byte[8192];
            int count;
            while ((count = file.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (fileData.ContentTransferEncoding == HttpContentTransferEncoding.Base64)
                {
                    string str = Convert.ToBase64String(buffer, 0, count);
                    requestStream.WriteString(str);
                }
                else if (fileData.ContentTransferEncoding == HttpContentTransferEncoding.Binary)
                {
                    requestStream.Write(buffer, 0, count);
                }
            }
        }

        public string GetContentType()
        {
            return string.Format("multipart/form-data; boundary=--------------{0}", boundaryCode);

        }

        public long GetContentLength()
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            long contentLength = ascii.GetBytes(boundary).Length;
            if (multipartFormData != null)
            {
                foreach (KeyValuePair<string, object> entry in multipartFormData)
                {
                    contentLength += ascii.GetBytes(CreateFormBoundaryHeader(entry.Key, entry.Value)).Length;
                    contentLength += ascii.GetBytes(boundary).Length;
                }
            }


            if (multipartFileData != null)
            {
                foreach (FileData fileData in multipartFileData)
                {
                    contentLength += ascii.GetBytes(CreateFileBoundaryHeader(fileData)).Length;
                    contentLength += new FileInfo(fileData.Filename).Length;
                    contentLength += ascii.GetBytes(boundary).Length;
                }
            }

            contentLength += ascii.GetBytes("--").Length;
            return contentLength;
        }

        static string CreateFileBoundaryHeader(FileData fileData)
        {
            return string.Format(
                "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                "Content-Type: {2}\r\n" +
                "Content-Transfer-Encoding: {3}\r\n\r\n"
                , fileData.FieldName
                , Path.GetFileName(fileData.Filename)
                , fileData.ContentType
                , fileData.ContentTransferEncoding);
        }

        static string CreateFormBoundaryHeader(string name, object value)
        {
            return string.Format("\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}"
                , name
                , value);
        }
    }
}