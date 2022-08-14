namespace OxyPlot
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    public abstract class XmlWriterBase : IDisposable
    {
        private readonly XmlWriter w;
        private bool disposed;
        protected XmlWriterBase()
        {
        }


        protected XmlWriterBase(Stream stream)
        {
            this.w = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 });
        }


        public virtual void Close()
        {
            this.Flush();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        public void Flush()
        {
            this.w.Flush();
        }

        protected void WriteAttributeString(string name, string value)
        {
            this.w.WriteAttributeString(name, value);
        }

        protected void WriteAttributeString(string prefix, string name, string ns, string value)
        {
            this.w.WriteAttributeString(prefix, name, ns, value);
        }

        protected void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            this.w.WriteDocType(name, pubid, sysid, subset);
        }

        protected void WriteElementString(string name, string text)
        {
            this.w.WriteElementString(name, text);
        }

        protected void WriteEndDocument()
        {
            this.w.WriteEndDocument();
        }

        protected void WriteEndElement()
        {
            this.w.WriteEndElement();
        }


        protected void WriteRaw(string text)
        {
            this.w.WriteRaw(text);
        }


        protected void WriteStartDocument(bool standalone)
        {
            this.w.WriteStartDocument(standalone);
        }

        protected void WriteStartElement(string name)
        {
            this.w.WriteStartElement(name);
        }

        protected void WriteStartElement(string name, string ns)
        {
            this.w.WriteStartElement(name, ns);
        }

        protected void WriteString(string text)
        {
            this.w.WriteString(text);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Close();
                }
            }

            this.disposed = true;
        }
    }
}