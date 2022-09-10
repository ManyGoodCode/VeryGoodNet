using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Application.Common.Specification
{
    /// <summary>
    /// 规格接口【也就是定义满足条件的一系列规格】包括如下：
    /// 1. 标准规格表达式
    /// 2. 包含规格集表达式
    /// 3. 添加‘且’规格表达式的函数契约
    /// 4. 添加‘或’规格表达式的函数契约
    /// </summary>
    public abstract class Specification<T> : ISpecification<T> where T : class, IEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        public Expression<Func<T, bool>> And(Expression<Func<T, bool>> query)
        {
            return Criteria = Criteria == null ? query : Criteria.And(query);
        }

        public Expression<Func<T, bool>> Or(Expression<Func<T, bool>> query)
        {
            return Criteria = Criteria == null ? query : Criteria.Or(query);
        }
    }
}
