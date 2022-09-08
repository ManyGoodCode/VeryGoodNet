using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity
{
    /// <summary>
    /// 用户状态容器。存储连接的用户Id【线程安全】/ 更新用户信息 / 断开用户连接 / 用户状态发生变化的触发事件
    /// </summary>
    public interface IUsersStateContainer
    {
        ConcurrentDictionary<string, string> UsersByConnectionId { get; }
        event Action? OnChange;
        void Update(string connectionId, string? name);
        void Remove(string connectionId);
    }
}
