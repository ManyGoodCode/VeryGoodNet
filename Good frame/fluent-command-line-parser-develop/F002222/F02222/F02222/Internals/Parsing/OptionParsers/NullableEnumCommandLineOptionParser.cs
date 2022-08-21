using System;

namespace Fclp.Internals.Parsing.OptionParsers
{
    public class NullableEnumCommandLineOptionParser<TEnum> 
        : ICommandLineOptionParser<TEnum?> where TEnum : struct
    {
        private readonly ICommandLineOptionParserFactory parserFactory;
        public NullableEnumCommandLineOptionParser(ICommandLineOptionParserFactory parserFactory)
        {
            Type type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException(string.Format("T must be an System.Enum but is '{0}'", type));
            this.parserFactory = parserFactory;
        }


        public TEnum? Parse(ParsedOption parsedOption)
        {
            if (parsedOption.HasValue == false) 
                return null;
            ICommandLineOptionParser<TEnum> parser = parserFactory.CreateParser<TEnum>();
            return parser.Parse(parsedOption);
        }

        public bool CanParse(ParsedOption parsedOption)
        {
            if (parsedOption == null) 
                return false;
            if (parsedOption.HasValue == false) 
                return true;

            ICommandLineOptionParser<TEnum> parser = parserFactory.CreateParser<TEnum>();
            return parser.CanParse(parsedOption);
        }
    }
}