using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Fclp.Internals
{
    public interface ICommandLineOptionSetupFactory
    {
        ICommandLineOptionFluent<T> Setup<T>(char shortOption, string longOption);
        ICommandLineOptionFluent<T> Setup<T>(char shortOption);
        ICommandLineOptionFluent<T> Setup<T>(string longOption);
    }


	public class CommandLineOptionBuilderFluent<TBuildType, TProperty> : ICommandLineOptionBuilderFluent<TProperty>
	{
	    private readonly ICommandLineCommandT<TBuildType> _command;
        private readonly ICommandLineOptionSetupFactory _setupFactory;
		private readonly TBuildType _buildObject;
		private readonly Expression<Func<TBuildType, TProperty>> _propertyPicker;

	    public CommandLineOptionBuilderFluent(
            ICommandLineOptionSetupFactory setupFactory, 
			TBuildType buildObject,
			Expression<Func<TBuildType, TProperty>> propertyPicker,
            ICommandLineCommandT<TBuildType> command)
		{
			_setupFactory = setupFactory;
			_buildObject = buildObject;
			_propertyPicker = propertyPicker;
	        _command = command;
		}

	    public CommandLineOptionBuilderFluent(
            ICommandLineOptionSetupFactory setupFactory, 
			TBuildType buildObject,
			Expression<Func<TBuildType, TProperty>> propertyPicker)
		{
			_setupFactory = setupFactory;
			_buildObject = buildObject;
			_propertyPicker = propertyPicker;
		}

		public ICommandLineOptionFluent<TProperty> As(char shortOption, string longOption)
		{
			return _setupFactory.Setup<TProperty>(shortOption, longOption)
                          .AssignToCommand(_command)
			              .Callback(AssignValueToPropertyCallback);
		}

		public ICommandLineOptionFluent<TProperty> As(char shortOption)
		{
			return _setupFactory.Setup<TProperty>(shortOption)
                          .AssignToCommand(_command)
			              .Callback(AssignValueToPropertyCallback);
		}

		public ICommandLineOptionFluent<TProperty> As(string longOption)
		{
			return _setupFactory.Setup<TProperty>(longOption)
                          .AssignToCommand(_command)
			              .Callback(AssignValueToPropertyCallback);
		}

		private void AssignValueToPropertyCallback(TProperty value)
		{
			PropertyInfo prop = (PropertyInfo)((MemberExpression)_propertyPicker.Body).Member;
			prop.SetValue(_buildObject, value, null);
		}
	}
}