using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestExpressionStep4
{
    public abstract class QueryProvider : System.Linq.IQueryProvider
    {
        public abstract string GetQueryText(Expression expression);
        public abstract object Execute(Expression expression);

        protected QueryProvider() { }

        IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
        {
            return new Query<T>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType),
                    new object[] 
                    { 
                        this, expression 
                    });
            }
            catch (TargetInvocationException tie) { throw tie.InnerException; }
        }

        T IQueryProvider.Execute<T>(Expression expression)
        {
            return (T)this.Execute(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return this.Execute(expression);
        }
    }
}
