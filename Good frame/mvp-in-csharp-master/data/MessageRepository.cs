using System;
using System.Collections.Generic;

namespace mvp_in_csharp.data
{
    /// <summary>
    /// 通过 构造器注入信息数据库接口IMessageDataSource对象   实现:信息数据库接口
    /// 1. 加载信息集合
    /// 2. 添加信息对象
    /// 3. 移除指定Id的信息
    /// </summary>
    public class MessageRepository : IMessageDataSource
    {
        readonly IMessageDataSource localDataSource;
        public MessageRepository(IMessageDataSource localDataSource)
        {
            this.localDataSource = localDataSource;
        }

        public IList<Message> LoadMessages()
        {
            return localDataSource.LoadMessages();
        }

        public void AddMessage(Message message)
        {
            localDataSource.AddMessage(message);
        }

        public void RemoveMessage(long messageId)
        {
            localDataSource.RemoveMessage(messageId);
        }
    }
}
