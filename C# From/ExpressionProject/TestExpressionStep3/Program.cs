using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;

namespace TestExpressionStep3
{
    class Program
    {
        static void Main(string[] args)
        {
            // 进程1；集合的Linq
            Console.WriteLine("----------------------------进程1----------------------------");
            List<User> users = new List<User>();
            IQueryable<User> userSql = users.AsQueryable().Where(predicate: u => u.Age > 2);
            Console.WriteLine(userSql);

            IQueryable<User> userSq2 = users.AsQueryable().Where(predicate: u => u.Name == "Jesse");       // 返回IQueryable<User> 非 string
            Console.WriteLine(userSq2);


            // 进程1；自定义QueryTranslator
            Console.WriteLine("----------------------------进程2----------------------------");
            List<User> users1 = new List<User>();
            string userSq3 = users1.AsQueryable().MyWhere(predicate: u => u.Age > 2);                       // 返回string 非 IQueryable<User> 
            Console.WriteLine(userSq3);

            //string userSq4 = users1.AsQueryable().MyWhere(predicate: u => u.Name == "Jesse");
            //Console.WriteLine(userSq4);


            //string userSq5 = users1.AsQueryable().MyWhere(predicate: u => u.Name == "Jesse" && u.Age > 2);
            //Console.WriteLine(userSq5);
            while (true) ;
        }
    }

    public class User
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class QueryTranslator : ExpressionVisitor
    {
        private StringBuilder sb;

        // 供外界调用
        internal string Translator(Expression expression)
        {
            this.sb = new StringBuilder();
            this.Visit(expression);
            return this.sb.ToString();
        }

        private static Expression StripQuotes(Expression exp)
        {
            while (exp.NodeType == ExpressionType.Quote)
            {
                exp = ((UnaryExpression)exp).Operand;
            }

            return exp;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(QueryExtensions) && node.Method.Name.EndsWith("Where",StringComparison.OrdinalIgnoreCase))
            {
                sb.Append("SELECT * FROM (");
                this.Visit(node.Arguments[0]);
                sb.Append(") AS T WHERE");
                LambdaExpression lamda = (LambdaExpression)StripQuotes(node.Arguments[1]);
                this.Visit(lamda.Body);
                return node;
            }

            throw new NotSupportedException(string.Format("方法{0}不支持", node.Method.Name));
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            sb.Append("(");
            this.Visit(node.Left);
            switch (node.NodeType)
            {
                case ExpressionType.Add:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.AndAlso:
                    sb.Append(" AND ");
                    break;

                case ExpressionType.Or:
                    sb.Append(" OR");
                    break;
                case ExpressionType.Equal:
                    sb.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    sb.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("二元运算符{0}不支持", node.NodeType));
            }

            this.Visit(node.Right);
            sb.Append(")");
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            IQueryable q = node.Value as IQueryable;
            if (q != null)
            {
                // 我们假设我们那个Queryable就是对应的表
                sb.Append("SELECT * FROM ");
                sb.Append(q.ElementType.Name);
            }
            else if (node.Value == null)
            {
                sb.Append("NULL");
            }
            else
            {
                switch (Type.GetTypeCode(node.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        sb.Append(((bool)node.Value) ? 1 : 0);
                        break;
                    case TypeCode.String:
                        sb.Append("'");
                        sb.Append(node.Value);
                        sb.Append("'");
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", node.Value));
                    default:
                        sb.Append(node.Value);
                        break;
                }
            }

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                sb.Append(node.Member.Name);
                return node;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", node.Member.Name));
        }
    }

    public static class QueryExtensions
    {
        public static string MyWhere<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            MethodCallExpression expression = Expression.Call(
                instance: null,
                method: ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }),
                arguments: new Expression[] { source.Expression, Expression.Quote(predicate) });

            QueryTranslator translator = new QueryTranslator();
            return translator.Translator(expression);
        }
    }
}
