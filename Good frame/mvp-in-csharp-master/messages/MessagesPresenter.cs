using System;
using System.Collections.Generic;
using mvp_in_csharp.data;

namespace mvp_in_csharp.messages
{
    /// <summary>
    /// Presenter 发言人的意思
    /// MessagesPresenter 通过 注入 实体仓库MessageRepository 和  用户视图IMessagesView 
    /// 将同步执行仓库的操作和执行效果的用户视图展示
    /// </summary>
    public class MessagesPresenter : IMessagesPresenter
    {
        private readonly MessageRepository repository;
        private readonly IMessagesView view;

        public MessagesPresenter(MessageRepository repository, IMessagesView view)
        {
            this.repository = repository;
            this.view = view;
            this.view.SetPresenter(this);
        }

        /// <summary>
        /// 从 仓库 加载所有实体 , 视图对象展示执行结果 , 展示完后视图对象回到主界面
        /// </summary>
        public void LoadMessages()
        {
            IList<Message> results = repository.LoadMessages();
            if (results == null || results.Count == 0)
            {
                view.ShowNotification("NO MESSAGES");
            }
            else
            {
                view.ShowMessages(results);
            }

            view.ShowInitialScreen();
        }

        /// <summary>
        /// 往仓库添加信息对象 , 视图对象展示执行结果 , 展示完后视图对象回到主界面
        /// </summary>
        public void AddMessage(Message message)
        {
            repository.AddMessage(message);
            view.ShowNotification("Add message successfully.");
            view.ShowInitialScreen();
        }

        /// <summary>
        /// 从仓库删除指定Id的对象 , 视图对象展示执行结果 , 展示完后视图对象回到主界面
        /// </summary>
        public void RemoveMessage(long id)
        {
            repository.RemoveMessage(id);
            view.ShowNotification($"Remove message( id = {id}) succefully.");
            view.ShowInitialScreen();
        }

        /// <summary>
        /// 视图对象回到主界面
        /// </summary>
        public void ShowMenuScreen()
        {
            view.ShowInitialScreen();
        }
    }
}
