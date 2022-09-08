using System;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{
    /// <summary>
    /// 时间接口。当前时间
    /// </summary>
    public interface IDateTime
    {
        DateTime Now { get; }
    }
}
