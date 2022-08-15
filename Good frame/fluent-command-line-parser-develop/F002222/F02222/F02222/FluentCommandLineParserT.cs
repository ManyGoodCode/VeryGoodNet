using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Fclp.Internals;

namespace Fclp
{
	public class FluentCommandLineParser<TBuildType> : IFluentCommandLineParser<TBuildType> where TBuildType : class
	{
		public IFluentCommandLineParser Parser { get; private set; }
		public TBuildType Object { get; private set; }

        public FluentCommandLineParser()
            : this(CreateArgsObject)
		{
        }

        public FluentCommandLineParser(Func<TBuildType> creator)
	    {
	        Object = creator();
            if(Object == null) throw new ArgumentNullException(nameof(creator));
	        Parser = new FluentCommandLineParser();
        }

	    private static TBuildType CreateArgsObject()
	    {
	        Type theType = typeof(TBuildType);
	        if (theType.GetConstructor(Type.EmptyTypes) == null)
	            throw new MissingMethodException(typeof(TBuildType).Name, "Parameterless constructor");
	        return Activator.CreateInstance<TBuildType>();
	    }

        public ICommandLineOptionBuilderFluent<TProperty> Setup<TProperty>(Expression<Func<TBuildType, TProperty>> propertyPicker)
		{
			return new CommandLineOptionBuilderFluent<TBuildType, TProperty>(Parser, Object, propertyPicker);
		}

		public ICommandLineParserResult Parse(string[] args)
		{
			return Parser.Parse(args);
		}

		public IHelpCommandLineOptionFluent SetupHelp(params string[] helpArgs)
		{
			return Parser.SetupHelp(helpArgs);
		}

		public bool IsCaseSensitive
		{
			get { return Parser.IsCaseSensitive; }
			set { Parser.IsCaseSensitive = value; }
		}

        public IHelpCommandLineOption HelpOption
        {
            get { return Parser.HelpOption; }
            set { Parser.HelpOption = value; }
        }

        public IEnumerable<ICommandLineOption> Options
        {
            get { return Parser.Options; }
        }

        public IFluentCommandLineParser<TBuildType> MakeCaseInsensitive()
	    {
	        Parser.MakeCaseInsensitive();
	        return this;
	    }

        public IFluentCommandLineParser<TBuildType> DisableShortOptions()
	    {
	        Parser.DisableShortOptions();
            return this;
        }

        public IFluentCommandLineParser<TBuildType> UseOwnOptionPrefix(params string[] prefix)
	    {
	        Parser.UseOwnOptionPrefix(prefix);
	        return this;
	    }

	    public IFluentCommandLineParser<TBuildType> SkipFirstArg()
	    {
	        Parser.SkipFirstArg();
	        return this;
        }
	}
}