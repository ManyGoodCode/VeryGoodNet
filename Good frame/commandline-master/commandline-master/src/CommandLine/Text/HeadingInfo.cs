using System;
using System.IO;
using System.Reflection;
using System.Text;
using CommandLine.Infrastructure;
using CSharpx;

namespace CommandLine.Text
{
    public class HeadingInfo
    {
        private readonly string programName;
        private readonly string version;

        public HeadingInfo(string programName, string version = null)
        {
            if (string.IsNullOrWhiteSpace("programName")) throw new ArgumentException("programName");

            this.programName = programName;
            this.version = version;
        }

        public static HeadingInfo Empty
        {
            get
            {
                return new HeadingInfo("");
            }
        }

        public static HeadingInfo Default
        {
            get
            {
                var title = ReflectionHelper.GetAttribute<AssemblyTitleAttribute>()
                    .MapValueOrDefault(
                        titleAttribute => titleAttribute.Title,
                        ReflectionHelper.GetAssemblyName());

                var version = ReflectionHelper.GetAttribute<AssemblyInformationalVersionAttribute>()
                    .MapValueOrDefault(
                        versionAttribute => versionAttribute.InformationalVersion,
                        ReflectionHelper.GetAssemblyVersion());
                return new HeadingInfo(title, version);
            }
        }

        public static implicit operator string(HeadingInfo info)
        {
            return info.ToString();
        }

        public override string ToString()
        {
            var isVersionNull = string.IsNullOrEmpty(version);
            return new StringBuilder(programName.Length +
                    (!isVersionNull ? version.Length + 1 : 0))
                .Append(programName)
                .AppendWhen(!isVersionNull, " ", version)
                .ToString();
        }

        public void WriteMessage(string message, TextWriter writer)
        {
            if (string.IsNullOrWhiteSpace("message")) throw new ArgumentException("message");
            if (writer == null) throw new ArgumentNullException("writer");

            writer.WriteLine(
                new StringBuilder(programName.Length + message.Length + 2)
                    .Append(programName)
                    .Append(": ")
                    .Append(message)
                    .ToString());
        }

        public void WriteMessage(string message)
        {
            WriteMessage(message, Console.Out);
        }

        public void WriteError(string message)
        {
            WriteMessage(message, Console.Error);
        }
    }
}
