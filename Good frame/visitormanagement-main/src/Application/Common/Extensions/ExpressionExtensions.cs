using System.Linq.Expressions;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System;
using System.Linq;
using System.Collections.Generic;
using CleanArchitecture.Blazor.Application.Common.Models;

namespace CleanArchitecture.Blazor.Application.Common.Extensions
{
    /// <summary>
    /// 表达式创建组合等一系列操作
    /// </summary>
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> FromFilter<T>(string filters)
        {
            Expression<Func<T, bool>> any = x => true;
            if (!string.IsNullOrEmpty(filters))
            {
                JsonSerializerOptions opts = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                opts.Converters.Add(new AutoNumberToStringConverter());
                FilterRule[]? filterRules = JsonSerializer.Deserialize<FilterRule[]>(filters, opts);

                foreach (FilterRule filter in filterRules)
                {
                    if (Enum.TryParse(filter.op, out OperationExpressionKind op) && !string.IsNullOrEmpty(filter.value))
                    {
                        Expression<Func<T, bool>> expression = GetCriteriaWhere<T>(fieldName: filter.field, operationExpressionKind: op, fieldValue: filter.value);
                        any = any.And(expression);
                    }
                }
            }

            return any;
        }

        #region -- Public methods --
        private static Expression<Func<T, bool>> GetCriteriaWhere<T>(Expression<Func<T, object>> e, OperationExpressionKind operationExpressionKind, object fieldValue)
        {
            var name = GetOperand<T>(e);
            return GetCriteriaWhere<T>(name, operationExpressionKind, fieldValue);
        }

        private static Expression<Func<T, bool>> GetCriteriaWhere<T, T2>(Expression<Func<T, object>> e, OperationExpressionKind operationExpressionKind, object fieldValue)
        {
            string name = GetOperand<T>(e);
            return GetCriteriaWhere<T, T2>(name, operationExpressionKind, fieldValue);
        }

        private static Expression<Func<T, bool>> GetCriteriaWhere<T>(string fieldName, OperationExpressionKind operationExpressionKind, object fieldValue)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            PropertyDescriptor prop = GetProperty(props, fieldName, true);
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            MemberExpression expressionParameter = GetMemberExpression<T>(parameter, fieldName);
            if (prop != null && fieldValue != null)
            {
                BinaryExpression body = null;
                if (prop.PropertyType.IsEnum)
                {
                    if (Enum.IsDefined(prop.PropertyType, fieldValue))
                    {
                        object value = Enum.Parse(prop.PropertyType, fieldValue.ToString(), true);
                        body = Expression.Equal(expressionParameter, Expression.Constant(value));
                        return Expression.Lambda<Func<T, bool>>(body, parameter);
                    }
                    else
                    {
                        return x => false;
                    }
                }

                switch (operationExpressionKind)
                {
                    case OperationExpressionKind.equal:
                        body = Expression.Equal(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue.ToString() == "null" ? null : fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                        return Expression.Lambda<Func<T, bool>>(body, parameter);

                    case OperationExpressionKind.notequal:
                        body = Expression.NotEqual(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue.ToString() == "null" ? null : fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                        return Expression.Lambda<Func<T, bool>>(body, parameter);

                    case OperationExpressionKind.less:
                        body = Expression.LessThan(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                        return Expression.Lambda<Func<T, bool>>(body, parameter);

                    case OperationExpressionKind.lessorequal:
                        body = Expression.LessThanOrEqual(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                        return Expression.Lambda<Func<T, bool>>(body, parameter);

                    case OperationExpressionKind.greater:
                        body = Expression.GreaterThan(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                        return Expression.Lambda<Func<T, bool>>(body, parameter);

                    case OperationExpressionKind.greaterorequal:
                        body = Expression.GreaterThanOrEqual(expressionParameter, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                        return Expression.Lambda<Func<T, bool>>(body, parameter);

                    case OperationExpressionKind.contains:
                        MethodInfo? contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        MethodCallExpression bodyLike = Expression.Call(expressionParameter, contains, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                        return Expression.Lambda<Func<T, bool>>(bodyLike, parameter);

                    case OperationExpressionKind.endwith:
                        MethodInfo? endswith = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                        MethodCallExpression bodyendwith = Expression.Call(expressionParameter, endswith, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                        return Expression.Lambda<Func<T, bool>>(bodyendwith, parameter);

                    case OperationExpressionKind.beginwith:
                        MethodInfo? startswith = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                        MethodCallExpression bodystartswith = Expression.Call(expressionParameter, startswith, Expression.Constant(Convert.ChangeType(fieldValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType), prop.PropertyType));
                        return Expression.Lambda<Func<T, bool>>(bodystartswith, parameter);

                    case OperationExpressionKind.includes:
                        return Includes<T>(fieldValue, parameter, expressionParameter, prop.PropertyType);

                    case OperationExpressionKind.between:
                        return Between<T>(fieldValue, parameter, expressionParameter, prop.PropertyType);

                    default:
                        throw new ArgumentException("OperationExpression");
                }
            }
            else
            {
                return x => false;
            }
        }

        private static Expression<Func<T, bool>> GetCriteriaWhere<T, T2>(string fieldName, OperationExpressionKind selectedOperator, object fieldValue)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            PropertyDescriptor prop = GetProperty(props, fieldName, true);

            ParameterExpression parameter = Expression.Parameter(typeof(T));
            MemberExpression expressionParameter = GetMemberExpression<T>(parameter, fieldName);

            if (prop != null && fieldValue != null)
            {
                switch (selectedOperator)
                {
                    case OperationExpressionKind.any:
                        return Any<T, T2>(fieldValue, parameter, expressionParameter);

                    default:
                        throw new Exception("Not implement Operation");
                }
            }
            else
            {
                Expression<Func<T, bool>> filter = x => true;
                return filter;
            }
        }



        #endregion
        #region -- Private methods --

        private static string GetOperand<T>(Expression<Func<T, object>> exp)
        {
            if (!(exp.Body is MemberExpression body))
            {
                UnaryExpression ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression;
            }

            string operand = body.ToString();
            return operand.Substring(2);

        }

        private static MemberExpression GetMemberExpression<T>(ParameterExpression parameter, string propName)
        {
            if (string.IsNullOrEmpty(propName))
            {
                return null;
            }

            string[] propertiesName = propName.Split('.');
            if (propertiesName.Length == 2)
            {
                return Expression.Property(Expression.Property(parameter, propertiesName[0]), propertiesName[1]);
            }

            return Expression.Property(parameter, propName);
        }

        private static Expression<Func<T, bool>> Includes<T>(object fieldValue, ParameterExpression parameterExpression, MemberExpression memberExpression, Type type)
        {
            Type? safetype = Nullable.GetUnderlyingType(type) ?? type;

            switch (safetype.Name.ToLower())
            {
                case "string":
                    List<string>? strlist = fieldValue.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (strlist == null || strlist.Count == 0)
                    {
                        return x => true;
                    }

                    MethodInfo? strmethod = typeof(List<string>).GetMethod("Contains", new Type[] { typeof(string) });
                    MethodCallExpression strcallexp = Expression.Call(Expression.Constant(strlist.ToList()), strmethod, memberExpression);
                    return Expression.Lambda<Func<T, bool>>(strcallexp, parameterExpression);


                case "int32":
                    List<int> intlist = fieldValue.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToList();
                    if (intlist == null || intlist.Count == 0)
                    {
                        return x => true;
                    }

                    MethodInfo? intmethod = typeof(List<int>).GetMethod("Contains", new Type[] { typeof(int) });
                    MethodCallExpression intcallexp = Expression.Call(Expression.Constant(intlist.ToList()), intmethod, memberExpression);
                    return Expression.Lambda<Func<T, bool>>(intcallexp, parameterExpression);

                case "float":
                case "decimal":
                    List<decimal> floatlist = fieldValue.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Decimal.Parse).ToList();
                    if (floatlist == null || floatlist.Count == 0)
                    {
                        return x => true;
                    }
                    MethodInfo? floatmethod = typeof(List<decimal>).GetMethod("Contains", new Type[] { typeof(decimal) });
                    MethodCallExpression floatcallexp = Expression.Call(Expression.Constant(floatlist.ToList()), floatmethod, memberExpression);
                    return Expression.Lambda<Func<T, bool>>(floatcallexp, parameterExpression);

                default:
                    return x => true;
            }

        }
        private static Expression<Func<T, bool>> Between<T>(object fieldValue, ParameterExpression parameterExpression, MemberExpression memberExpression, Type type)
        {

            Type safetype = Nullable.GetUnderlyingType(type) ?? type;
            switch (safetype.Name.ToLower())
            {
                case "datetime":
                    string[] datearray = ((string)fieldValue).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    DateTime start = Convert.ToDateTime(datearray[0] + " 00:00:00", CultureInfo.CurrentCulture);
                    DateTime end = Convert.ToDateTime(datearray[1] + " 23:59:59", CultureInfo.CurrentCulture);
                    BinaryExpression greater = Expression.GreaterThanOrEqual(memberExpression, Expression.Constant(start, type));
                    BinaryExpression less = Expression.LessThanOrEqual(memberExpression, Expression.Constant(end, type));
                    return Expression.Lambda<Func<T, bool>>(greater, parameterExpression)
                      .And(Expression.Lambda<Func<T, bool>>(less, parameterExpression));

                case "int":
                case "int32":
                    string[] intarray = ((string)fieldValue).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    int min = Convert.ToInt32(intarray[0], CultureInfo.CurrentCulture);
                    int max = Convert.ToInt32(intarray[1], CultureInfo.CurrentCulture);
                    BinaryExpression maxthen = Expression.GreaterThanOrEqual(memberExpression, Expression.Constant(min, type));
                    BinaryExpression minthen = Expression.LessThanOrEqual(memberExpression, Expression.Constant(max, type));
                    return Expression.Lambda<Func<T, bool>>(maxthen, parameterExpression)
                      .And(Expression.Lambda<Func<T, bool>>(minthen, parameterExpression));

                case "decimal":
                    string[] decarray = ((string)fieldValue).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    decimal dmin = Convert.ToDecimal(decarray[0], CultureInfo.CurrentCulture);
                    decimal dmax = Convert.ToDecimal(decarray[1], CultureInfo.CurrentCulture);
                    BinaryExpression dmaxthen = Expression.GreaterThanOrEqual(memberExpression, Expression.Constant(dmin, type));
                    BinaryExpression dminthen = Expression.LessThanOrEqual(memberExpression, Expression.Constant(dmax, type));
                    return Expression.Lambda<Func<T, bool>>(dmaxthen, parameterExpression)
                      .And(Expression.Lambda<Func<T, bool>>(dminthen, parameterExpression));

                case "float":
                    string[] farray = ((string)fieldValue).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    decimal fmin = Convert.ToDecimal(farray[0], CultureInfo.CurrentCulture);
                    decimal fmax = Convert.ToDecimal(farray[1], CultureInfo.CurrentCulture);
                    BinaryExpression fmaxthen = Expression.GreaterThanOrEqual(memberExpression, Expression.Constant(fmin, type));
                    BinaryExpression fminthen = Expression.LessThanOrEqual(memberExpression, Expression.Constant(fmax, type));
                    return Expression.Lambda<Func<T, bool>>(fmaxthen, parameterExpression)
                      .And(Expression.Lambda<Func<T, bool>>(fminthen, parameterExpression));

                case "string":
                    string[] strarray = ((string)fieldValue).Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    string strstart = strarray[0];
                    string strend = strarray[1];
                    MethodInfo? strmethod = typeof(string).GetMethod("CompareTo", new[] { typeof(string) });
                    MethodCallExpression callcomparetostart = Expression.Call(memberExpression, strmethod, Expression.Constant(strstart, type));
                    MethodCallExpression callcomparetoend = Expression.Call(memberExpression, strmethod, Expression.Constant(strend, type));
                    BinaryExpression strgreater = Expression.GreaterThanOrEqual(callcomparetostart, Expression.Constant(0));
                    BinaryExpression strless = Expression.LessThanOrEqual(callcomparetoend, Expression.Constant(0));
                    return Expression.Lambda<Func<T, bool>>(strgreater, parameterExpression)
                      .And(Expression.Lambda<Func<T, bool>>(strless, parameterExpression));

                default:
                    return x => true;
            }

        }



        private static Expression<Func<T, bool>> Any<T, T2>(object fieldValue, ParameterExpression parameterExpression, MemberExpression memberExpression)
        {
            Expression<Func<T2, bool>> lambda = (Expression<Func<T2, bool>>)fieldValue;
            MethodInfo? anyMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Any" && m.GetParameters().Length == 2).MakeGenericMethod(typeof(T2));

            MethodCallExpression body = Expression.Call(anyMethod, memberExpression, lambda);
            return Expression.Lambda<Func<T, bool>>(body, parameterExpression);
        }

        private static PropertyDescriptor GetProperty(PropertyDescriptorCollection props, string fieldName, bool ignoreCase)
        {
            if (!fieldName.Contains('.'))
            {
                return props.Find(fieldName, ignoreCase);
            }

            string[] fieldNameProperty = fieldName.Split('.');
            return props.Find(fieldNameProperty[0], ignoreCase).GetChildProperties().Find(fieldNameProperty[1], ignoreCase);

        }
        #endregion

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            ParameterExpression p = left.Parameters.First();
            SubstExpressionVisitor visitor = new SubstExpressionVisitor
            {
                Subst = { [right.Parameters.First()] = p }
            };

            Expression body = Expression.AndAlso(left.Body, visitor.Visit(right.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {

            ParameterExpression p = left.Parameters.First();
            SubstExpressionVisitor visitor = new SubstExpressionVisitor
            {
                Subst = { [right.Parameters.First()] = p }
            };

            Expression body = Expression.OrElse(left.Body, visitor.Visit(right.Body));
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
    }

    internal class SubstExpressionVisitor : ExpressionVisitor
    {
        public Dictionary<Expression, Expression> Subst = new Dictionary<Expression, Expression>();

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (Subst.TryGetValue(node, out var newValue))
            {
                return newValue;
            }
            return node;
        }
    }

    internal class SwapVisitor : ExpressionVisitor
    {
        private readonly Expression from, to;
        public SwapVisitor(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }
        public override Expression Visit(Expression node) => node == from ? to : base.Visit(node);
    }

    internal sealed class AutoNumberToStringConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(string) == typeToConvert;
        }
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.TryGetInt64(out long l) ?
                    l.ToString() :
                    reader.GetDouble().ToString();
            }
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                return document.RootElement.Clone().ToString();
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    internal enum OperationExpressionKind
    {
        equal,
        notequal,
        less,
        lessorequal,
        greater,
        greaterorequal,
        contains,
        beginwith,
        endwith,
        includes,
        between,
        any
    }
}