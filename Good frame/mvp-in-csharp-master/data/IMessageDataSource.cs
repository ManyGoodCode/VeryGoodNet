using System.Collections.Generic;
namespace mvp_in_csharp.data
{
    /// <summary>
    /// 信息数据库接口实现如下契约
    /// 1. 加载信息集合
    /// 2. 添加信息对象
    /// 3. 移除指定Id的信息
    /// </summary>
    public interface IMessageDataSource
    {
        IList<Message> LoadMessages();

        void AddMessage(Message message);

        void RemoveMessage(long messageId);
    }
}
