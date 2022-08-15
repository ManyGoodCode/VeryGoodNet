using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Fclp.Internals;
using Fclp.Internals.Validators;

namespace Fclp
{
    class CommandLineCommand<TBuildType> : ICommandLineOptionSetupFactory, ICommandLineCommandT<TBuildType>, ICommandLineCommandFluent<TBuildType> where TBuildType : new()
    {
        List<ICommandLineOption> _options;
        ICommandLineOptionFactory _optionFactory;
        ICommandLineOptionValidator _optionValidator;

        public CommandLineCommand(IFluentCommandLineParser parser)
        {
            Parser = parser;
            Object = new TBuildType();
        }

        public IFluentCommandLineParser Parser { get; set; }
        public TBuildType Object { get; private set; }
        public Action<TBuildType> SuccessCallback { get; set; }
        public string Name { get; set; }
        public IEnumerable<ICommandLineOption> Options
        {
            get { return _options ?? (_options = new List<ICommandLineOption>()); }
        }

        public bool HasSuccessCallback
        {
            get { return SuccessCallback != null; }
        }

        public void ExecuteOnSuccess()
        {
            if (HasSuccessCallback)
                SuccessCallback(Object);
        }

        public ICommandLineOptionValidator OptionValidator
        {
            get { return _optionValidator ?? (_optionValidator = new CommandLineOptionValidator(this, Parser.SpecialCharacters)); }
            set { _optionValidator = value; }
        }

        public ICommandLineOptionFactory OptionFactory
        {
            get { return _optionFactory ?? (_optionFactory = new CommandLineOptionFactory()); }
            set { _optionFactory = value; }
        }

        public ICommandLineCommandFluent<TBuildType> OnSuccess(Action<TBuildType> callback)
        {
            SuccessCallback = callback;
            return this;
        }

        public ICommandLineOptionBuilderFluent<TProperty> Setup<TProperty>(Expression<Func<TBuildType, TProperty>> propertyPicker)
        {
            return new CommandLineOptionBuilderFluent<TBuildType, TProperty>(this, Object, propertyPicker, this);
        }

        public ICommandLineOptionFluent<T> Setup<T>(char shortOption, string longOption)
        {
            return SetupInternal<T>(shortOption.ToString(CultureInfo.InvariantCulture), longOption);
        }

        public ICommandLineOptionFluent<T> Setup<T>(char shortOption)
        {
            return SetupInternal<T>(shortOption.ToString(CultureInfo.InvariantCulture), null);
        }

        public ICommandLineOptionFluent<T> Setup<T>(string longOption)
        {
            return SetupInternal<T>(null, longOption);
        }

        private ICommandLineOptionFluent<T> SetupInternal<T>(string shortOption, string longOption)
        {
            ICommandLineOptionResult<T> argOption = this.OptionFactory.CreateOption<T>(shortOption, longOption);
            if (argOption == null)
                throw new InvalidOperationException("OptionFactory is producing unexpected results.");
            OptionValidator.Validate(argOption, Parser.IsCaseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
            _options.Add(argOption);
            return argOption;
        }
    }

    public interface ICommandLineCommandT<TBuildType> : ICommandLineCommand, ICommandLineOptionContainer
    {
        TBuildType Object { get; }
        Action<TBuildType> SuccessCallback { get; set; }
    }

    public interface ICommandLineCommand
    {
        IFluentCommandLineParser Parser { get; set; }
        string Name { get; set; }
        IEnumerable<ICommandLineOption> Options { get; }
        bool HasSuccessCallback { get; }
        void ExecuteOnSuccess();
    }
}