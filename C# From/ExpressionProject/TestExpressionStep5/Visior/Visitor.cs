using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TestExpressionStep5
{
    // 主要作用生成 具体需要的DTO
    public class Visitor
    {
        const string Where = "Where";
        const string Contains = "Contains";
        const string IsNullOrWhiteSpace = "IsNullOrWhiteSpace";
        private readonly Dictionary<string, string> KeyValues = new Dictionary<string, string>();
        public DTO ProcessExpression(Expression expression)
        {
            KeyValues.Clear();
            VisitExpression(expression);
            string text = string.Join(",", KeyValues.Select(kv => string.Format("[{0},{1}]", kv.Key, kv.Value)));
            Console.WriteLine(string.Format("Expression:{0} \r\nValue:{1}", expression.ToString(), text));
            Console.WriteLine();
            Console.WriteLine();
            string parm = string.Empty;
            KeyValues.TryGetValue(Contains,out parm);
            return new DTO() { Para1 = parm};
        }

        private void VisitExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                // 访问 &&
                case ExpressionType.AndAlso:
                    VisitAndAlso((BinaryExpression)expression);
                    break;

                case ExpressionType.Equal:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    BinaryExpression binary = (BinaryExpression)expression;
                    CompareExpression(binary);
                    break;

                // 访问调用方法，主要有于解析Contains方法，我们的Title会用到
                case ExpressionType.Call:
                    VisitMethodCall((MethodCallExpression)expression);
                    break;

                // 访问Lambda表达式
                case ExpressionType.Lambda:
                    LambdaExpression lambda = (LambdaExpression)expression;
                    // 1.1 GreaterThan = u.Age > 1
                    VisitExpression(lambda.Body);
                    break;
            }
        }

        private void VisitMethodCall(MethodCallExpression expression)
        {
            string methodName = expression.Method.Name;
            Type declaringType = expression.Method.DeclaringType;
            if ((declaringType == typeof(Queryable)) && (methodName == Where))
            {
                UnaryExpression uExpression = (UnaryExpression)expression.Arguments[1];
                // 1.1 ExpressionType.Lambda = u.Age > 1
                VisitExpression(uExpression.Operand);
            }
            else if (declaringType == typeof(string))
            {
                if (expression.Object.NodeType != ExpressionType.MemberAccess)
                    throw new NotSupportedException("Method not supported: " + expression.Method.Name);
                MemberExpression member = expression.Object as MemberExpression;
                KeyValues.Add(key: string.Format("{0}",  methodName), value: expression.Arguments[0].ToString());
            }
        }

        private void VisitAndAlso(BinaryExpression andAlso)
        {
            VisitExpression(andAlso.Left);
            VisitExpression(andAlso.Right);
        }

        private void CompareExpression(BinaryExpression expression)
        {
            if (expression.Left.NodeType != ExpressionType.MemberAccess || expression.Right.NodeType != ExpressionType.Constant)
                throw new NotSupportedException("BinaryExpression not support:");
            MemberExpression member = expression.Left as MemberExpression;
            ConstantExpression constant = expression.Right as ConstantExpression;
            KeyValues.Add(key: string.Format("{0} {1}", member.Member.Name, expression.NodeType), value: constant.Value.ToString());
        }
    }
}
