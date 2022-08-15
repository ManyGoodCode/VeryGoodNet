using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Infrastructure;

namespace CommandLine.Text
{
    public class TextWrapper
    {
        private string[] lines;
        public TextWrapper(string input)
        {
            lines = input
                .Replace("\r","")
                .Split(new[] {'\n'}, StringSplitOptions.None);
        }

        public TextWrapper WordWrap(int columnWidth)
        {
            columnWidth = Math.Max(1, columnWidth);
            lines= lines
                .SelectMany(line => WordWrapLine(line, columnWidth))
                .ToArray();
            return this;
        }

        public TextWrapper Indent(int numberOfSpaces)
        {
            lines = lines
                .Select(line => numberOfSpaces.Spaces() + line)
                .ToArray();
            return this;
        }

        public string ToText()
        {
            return string.Join(Environment.NewLine,lines);
        }

        public static string WrapAndIndentText(string input, int indentLevel,int columnWidth)
        {
            return new TextWrapper(input)
                .WordWrap(columnWidth)
                .Indent(indentLevel)
                .ToText();
        }


        private string [] WordWrapLine(string line,int columnWidth)
        {
            var unindentedLine = line.TrimStart();
            var currentIndentLevel = Math.Min(line.Length - unindentedLine.Length,columnWidth-1) ;
            columnWidth -= currentIndentLevel;

            return unindentedLine.Split(' ')
                .Aggregate(
                    new List<StringBuilder>(),
                    (lineList, word) => AddWordToLastLineOrCreateNewLineIfNecessary(lineList, word, columnWidth)
                )
                .Select(builder => currentIndentLevel.Spaces()+builder.ToString().TrimEnd())
                .ToArray();
        }

        private static List<StringBuilder> AddWordToLastLineOrCreateNewLineIfNecessary(List<StringBuilder> lines, string word,int columnWidth)
        {
            var previousLine = lines.LastOrDefault()?.ToString() ??string.Empty;
            var wouldWrap = !lines.Any() || (word.Length>0 && previousLine.Length + word.Length > columnWidth);
          
            if (!wouldWrap)
            {
                lines.Last().Append(word + ' ');
            }
            else
            {
                do
                {
                    var availableCharacters = Math.Min(columnWidth, word.Length);
                    var segmentToAdd = LeftString(word,availableCharacters) + ' ';
                    lines.Add(new StringBuilder(segmentToAdd));
                    word = RightString(word,availableCharacters);
                } while (word.Length > 0);
            }
            return lines;
        }

        private static string RightString(string str,int n)
        {
            return (n >= str.Length || str.Length==0) 
                ? string.Empty 
                : str.Substring(n);
        }

        private static string LeftString(string str,int n)
        {
            
            return  (n >= str.Length || str.Length==0)
                ? str 
                : str.Substring(0,n);
        }
    }
}
