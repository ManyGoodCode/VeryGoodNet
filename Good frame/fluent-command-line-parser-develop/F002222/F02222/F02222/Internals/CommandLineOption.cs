using System;
using System.Collections.Generic;
using System.Linq;
using Fclp.Internals.Extensions;
using Fclp.Internals.Parsing;
using Fclp.Internals.Parsing.OptionParsers;

namespace Fclp.Internals
{
    public class CommandLineOption<T> : ICommandLineOptionResult<T>
    {
        public CommandLineOption(string shortName, string longName, ICommandLineOptionParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            this.ShortName = shortName;
            this.LongName = longName;
            this.Parser = parser;
        }

        ICommandLineOptionParser<T> Parser { get; set; }
        public string Description { get; set; }
        internal Action<T> ReturnCallback { get; set; }
        internal Action<IEnumerable<string>> AdditionalArgumentsCallback { get; set; }
        internal T Default { get; set; }
        public bool IsRequired { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public bool HasDefault { get; set; }
        public ICommandLineCommand Command { get; set; }
        public Type SetupType
        {
            get
            {
                Type type = typeof(T);
                Type[] genericArgs = type.GetGenericArguments();
                return genericArgs.Any() ? genericArgs.First() : type;
            }
        }

        public bool HasLongName
        {
            get { return this.LongName.IsNullOrWhiteSpace() == false; }
        }

        public bool HasShortName
        {
            get { return this.ShortName.IsNullOrWhiteSpace() == false; }
        }

        public bool HasCallback
        {
            get { return this.ReturnCallback != null; }
        }

        public bool HasAdditionalArgumentsCallback
        {
            get { return this.AdditionalArgumentsCallback != null; }
        }


        public void Bind(ParsedOption value)
        {
            if (this.Parser.CanParse(value) == false)
                throw new OptionSyntaxException();
            T input = this.Parser.Parse(value);
            if (typeof(T).IsClass && object.Equals(input, default(T)) && this.HasDefault)
                BindDefault();
            else
            {
                this.Bind(input);
                this.BindAnyAdditionalArgs(value);
            }
        }

        public void BindDefault()
        {
            if (this.HasDefault)
                this.Bind(this.Default);
        }

        void Bind(T value)
        {
            if (this.HasCallback)
                this.ReturnCallback(value);
        }

        void BindAnyAdditionalArgs(ParsedOption option)
        {
            if (!this.HasAdditionalArgumentsCallback) return;

            if (option.AdditionalValues.Any())
            {
                this.AdditionalArgumentsCallback(option.AdditionalValues);
            }
        }

        public ICommandLineOptionFluent<T> WithDescription(string description)
        {
            this.Description = description;
            return this;
        }

        public ICommandLineOptionFluent<T> Required()
        {
            this.IsRequired = true;
            return this;
        }

        public ICommandLineOptionFluent<T> Callback(Action<T> callback)
        {
            this.ReturnCallback = callback;
            return this;
        }

        public ICommandLineOptionFluent<T> SetDefault(T value)
        {
            this.Default = value;
            this.HasDefault = true;
            return this;
        }

        public ICommandLineOptionFluent<T> CaptureAdditionalArguments(Action<IEnumerable<string>> callback)
        {
            this.AdditionalArgumentsCallback = callback;
            return this;
        }

        public ICommandLineOptionFluent<T> AssignToCommand(ICommandLineCommand command)
        {
            this.Command = command;
            return this;
        }

	    public ICommandLineOptionFluent<T> UseForOrphanArguments()
        {
            this.UseForOrphanArgs = true;
            return this;
        }

	    public bool UseForOrphanArgs { get; set; }
        public bool HasCommand
        {
            get { return Command != null; }
        }

        public object GetDefaultValue()
        {
            if (HasDefault)
            {
                return Default;
            }
            return null;
        }
    }
}
