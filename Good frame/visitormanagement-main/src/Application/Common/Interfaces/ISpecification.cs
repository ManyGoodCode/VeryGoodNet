using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{
    /// <summary>
    /// 规格接口【也就是定义满足条件的一系列规格】包括如下：
    /// 1. 标准规格表达式
    /// 2. 包含规格集表达式
    /// 3. 添加‘且’规格表达式的函数契约
    /// 4. 添加‘或’规格表达式的函数契约
    /// </summary>
    public interface ISpecification<T> where T : class, IEntity
    {
        /// <summary>
        /// 标准规格
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }

        /// <summary>
        /// 包含规格集
        /// </summary>
        List<Expression<Func<T, object>>> Includes { get; }

        List<string> IncludeStrings { get; }

        /// <summary>
        /// 添加‘且’规格的函数契约
        /// </summary>
        Expression<Func<T, bool>> And(Expression<Func<T, bool>> query);

        /// <summary>
        /// 添加‘或’规格的函数契约
        /// </summary>
        Expression<Func<T, bool>> Or(Expression<Func<T, bool>> query);
    }
}
