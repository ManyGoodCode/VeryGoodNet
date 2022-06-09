using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections;

namespace TestExpressionStep5
{
    class Program
    {
        // 1. 表达式封装 委托
        static readonly Expression<Func<User, bool>> expression = u => u.Name.Contains("3");

        // 此处自己构造的提供者，不考虑 IQueryable<T1,T2,T3> 的情况；IQueryable<T1,T2,T3> 在数据库IQueryable中有使用
        static void Main(string[] args)
        {
            IQueryable<User> query = new Query<User>(new QueryProvider());

            // 1. IQueryable<T>的意义, Linq使用的类型
            // 2. 此处通过本质是间接通过 Provider.CreateQuery 生成新的 IQueryable<T>
            // 3. 新的 IQueryable<T> 与之前的 IQueryable<T> 的 Expression无关系
            IQueryable<User> query1_2 = from u in query
                                        where u.Age <= 1
                                        select u;
            // 1. 直接通过 Provider.CreateQuery 生成新的 IQueryable<T>
            // 2. 新的 IQueryable<T> 与之前的 IQueryable<T> 的 Expression无关系
            IQueryable<User> query1_3 = query1_2.Provider.CreateQuery<User>(expression);

            //
            List<User> u1 = query1_2.ToList();

            // 1. 直接通过 Provider.CreateQuery 生成新的 IQueryable,非泛型
            // 2. 并查询数据
            IQueryable query2_1 = new QueryProvider().CreateQuery(expression);
            List<User> u2 = (query2_1 as IQueryable<User>).ToList();
            if (u2.Count > 0)
                Console.WriteLine(u2[0].Name);


            IQueryable<User> query3_1 = from u in query
                                        where u.Age <= 1 && u.Score > 60
                                        select u;
            List<User> u3 = query3_1.ToList();
            while (true) ;
        }
    }

    // 继承IQueryable/IQueryable<T>接口实现自己的查询
    public class Query<T> : IQueryable, IQueryable<T>,
        IEnumerable<T>, IEnumerable,
        IOrderedQueryable<T>, IOrderedQueryable
    {
        public Type ElementType
        {
            get { return typeof(T); }
        }
        public Expression Expression { get; private set; }
        public IQueryProvider Provider { get; private set; }
        public System.Collections.IEnumerator GetEnumerator()
        {
            // 先获取接口
            System.Collections.IEnumerable able = this.Provider.Execute(this.Expression)
                as System.Collections.IEnumerable;
            // 再获取迭代器
            System.Collections.IEnumerator ator = able.GetEnumerator();
            return ator;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            // 先获取接口
            System.Collections.Generic.IEnumerable<T> able = this.Provider.Execute<IEnumerable<T>>(this.Expression)
                as System.Collections.Generic.IEnumerable<T>;
            // 再获取迭代器
            System.Collections.Generic.IEnumerator<T> ator = able.GetEnumerator() as
            System.Collections.Generic.IEnumerator<T>;
            return ator;
        }

        public Query(QueryProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this.Provider = provider;
            this.Expression = Expression.Constant(this);
        }

        public Query(IQueryProvider provider, Expression expression)
        {
            if (provider == null || expression == null)
                throw new ArgumentNullException("provider/expression");
            this.Provider = provider;
            this.Expression = expression;
        }
    }

    // 继承IQueryProvider接口实现查询提供器
    public class QueryProvider : IQueryProvider
    {
        // 此函数不可外部调用;外部调用将报错，系统自己调用
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Query<TElement>(this, expression);
        }

        // 根据 Type创建泛型实例
        public IQueryable CreateQuery(Expression expression)
        {
            Type genericType = TypeHelp.FindGenericType(expression.Type);
            Type elementType = (genericType == null) ? expression.Type : genericType;
            if (elementType == null)
                throw new ArgumentException("CreateQuery expression");
            Type genType = typeof(Query<>).MakeGenericType(elementType);
            return Activator.CreateInstance(genType, this, expression) as IQueryable;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            // 引用下面的Object 返回 重载函数
            return (TResult)Execute(expression);
        }

        public object Execute(Expression expression)
        {
            DTO dto = new Visitor().ProcessExpression(expression);
            Type genericType = TypeHelp.FindGenericType(expression.Type);
            Type elementType = (genericType == null) ? expression.Type : genericType;
            if (elementType == typeof(User))
            {
                if (!string.IsNullOrWhiteSpace(dto.Para1))
                {
                    string body = WebHelp.GetURL(string.Format("{0}?nameContain={1}", "http://localhost:65248/Home/GetProviceTV", dto.Para1.Replace("\"", string.Empty)));
                    User user = new User(){Name = body};
                    return new List<User>() { user };
                }

                return new List<User>() { };
            }

            return new List<User>();
        }
    }
}
