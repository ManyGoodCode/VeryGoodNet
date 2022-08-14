namespace OxyPlot
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    internal class PdfWriter : IDisposable
    {
        private BinaryWriter w;

        public PdfWriter(Stream s)
        {
            this.w = new BinaryWriter(s);
        }

        internal enum ObjectType
        {
            Catalog,
            Pages,
            Page,
            Font,
            XObject,
            ExtGState,
            FontDescriptor
        }

        internal interface IPortableDocumentObject
        {
            int ObjectNumber { get; }
        }

        public long Position
        {
            get { return this.w.BaseStream.Position; }
        }

        public void Write(string format, params object[] args)
        {
            this.w.Write(
                Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, format,
                args)));
        }


        public void WriteLine(string format, params object[] args)
        {
            this.Write(format + "\n", args);
        }

        public void Write(Dictionary<string, object> dictionary)
        {
            this.WriteLine("<<");
            foreach (KeyValuePair<string, object> kvp in dictionary)
            {
                this.Write(kvp.Key);
                this.Write(" ");
                this.WriteCore(kvp.Value);
                this.WriteLine();
            }

            this.Write(">>");
        }

        public void Write(byte[] bytes)
        {
            this.w.Write(bytes);
        }

        public void WriteLine()
        {
            this.WriteLine(string.Empty);
        }

        public void Dispose()
        {
            this.w.Dispose();
        }

        private void WriteCore(object o)
        {
            IPortableDocumentObject pdfObject = o as IPortableDocumentObject;
            if (pdfObject != null)
            {
                this.Write("{0} 0 R", pdfObject.ObjectNumber);
                return;
            }

            if (o is ObjectType)
            {
                this.Write("/{0}", o);
                return;
            }

            if (o is int || o is double)
            {
                this.Write("{0}", o);
                return;
            }

            if (o is bool)
            {
                this.Write((bool)o ? "true" : "false");
                return;
            }

            if (o is DateTime)
            {
                DateTime dt = (DateTime)o;
                string dts = "(D:" + dt.ToString("yyyyMMddHHmmsszz") + "'00)";
                this.Write(dts);
                return;
            }

            string s = o as string;
            if (s != null)
            {
                this.Write(s);
                return;
            }

            IList list = o as IList;
            if (list != null)
            {
                this.WriteList(list);
                return;
            }

            Dictionary<string, object> dictionary = o as Dictionary<string, object>;
            if (dictionary != null)
            {
                this.Write(dictionary);
            }
        }

        private void WriteList(IList list)
        {
            this.Write("[");
            bool first = true;

            foreach (var o in list)
            {
                if (!first)
                {
                    this.Write(" ");
                }
                else
                {
                    first = false;
                }

                this.WriteCore(o);
            }

            this.Write("]");
        }
    }
}