using System;
using System.Linq.Expressions;

namespace Fclp
{
    public interface ICommandLineCommandFluent<TBuildType>
    {
        ICommandLineCommandFluent<TBuildType> OnSuccess(Action<TBuildType> callback);
        ICommandLineOptionBuilderFluent<TProperty> Setup<TProperty>(Expression<Func<TBuildType, TProperty>> propertyPicker);
    }
}