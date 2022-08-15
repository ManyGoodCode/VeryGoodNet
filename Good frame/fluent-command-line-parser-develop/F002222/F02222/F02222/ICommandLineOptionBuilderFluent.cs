namespace Fclp
{
	public interface ICommandLineOptionBuilderFluent<TProperty>
	{
		ICommandLineOptionFluent<TProperty> As(char shortOption, string longOption);
		ICommandLineOptionFluent<TProperty> As(char shortOption);
		ICommandLineOptionFluent<TProperty> As(string longOption);
	}
}