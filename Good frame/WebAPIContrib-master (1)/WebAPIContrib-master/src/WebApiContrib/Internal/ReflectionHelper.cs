using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WebApiContrib.Internal
{
    internal static class ReflectionHelper
    {
        public static MethodCallExpression GetMethodCall<T>(Expression<T>  action )
        {
            MethodCallExpression call = action.Body as MethodCallExpression;
            return call;
        }

        public static string GetTypeName<T>()
        {
            return typeof (T).Name;
        }

        public static IEnumerable<Tuple<ParameterInfo, object>> GetArgumentValues(MethodCallExpression methodCall)
        {
            ParameterInfo[] parameters = methodCall.Method.GetParameters();
            if(parameters.Any())
            {
                for(int i = 0; i < parameters.Length; i++)
                {
                    Expression arg = methodCall.Arguments[i];
                    ConstantExpression ceValue = arg as ConstantExpression;
                    if (ceValue != null)
                        yield return new Tuple<ParameterInfo, object>(parameters[i], ceValue.Value);
                    else
                        yield return new Tuple<ParameterInfo, object>(parameters[i], GetExpressionValue(arg));
                }
            }
        }

        private static object GetExpressionValue(Expression expression)
        {
            Expression<Func<object>> lambda = Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof (object)));
            Func<object> func = lambda.Compile();
            return func();
        }
    }
}