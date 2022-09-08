using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{
    /// <summary>
    /// 选择列表。 数据源 / 改变触发事件 / 初始化  / 刷新
    /// </summary>
    public interface IPicklistService
    {
        List<KeyValueDto> DataSource { get; }
        event Action? OnChange;
        Task Initialize();
        Task Refresh();
    }
}
