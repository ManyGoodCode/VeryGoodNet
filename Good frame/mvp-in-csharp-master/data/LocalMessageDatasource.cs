using System;
using System.Collections.Generic;

namespace mvp_in_csharp.data
{
    /// <summary>
    /// 通过 构造器注入文件与集合之间的操作对象InFileSavingHelper  实现:信息数据库接口
    /// 1. 加载信息集合
    /// 2. 添加信息对象
    /// 3. 移除指定Id的信息
    /// </summary>
    public class LocalMessageDataSource : IMessageDataSource
    {
        private InFileSavingHelper fileHelper;
        public LocalMessageDataSource(InFileSavingHelper fileHelper)
        {
            this.fileHelper = fileHelper;
        }

        public void AddMessage(Message message)
        {
            fileHelper.AppendMessageToFile(message);
        }

        public IList<Message> LoadMessages()
        {
            return fileHelper.LoadMessagesFromFile();
        }

        public void RemoveMessage(long messageId)
        {
            fileHelper.RemoveMessageFromFile(messageId);
        }
    }
}
