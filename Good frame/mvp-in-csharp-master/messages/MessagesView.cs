using System;
using System.Collections.Generic;
using mvp_in_csharp.data;

namespace mvp_in_csharp.messages
{
    /// <summary>
    /// 界面。界面对象通过注入 发言人 IMessagesPresenter 
    /// 之后所有的界面展示都交给发言人来调用
    /// </summary>
    public class MessagesView : IMessagesView
    {
        private IMessagesPresenter presenter;

        public void SetPresenter(IMessagesPresenter presenter)
        {
            this.presenter = presenter;
        }

        /// <summary>
        /// 主菜单执行序号【交给发言人来发言】
        /// 1. 加载所有信息
        /// 2. 添加信息
        /// 3. 退出
        /// </summary>
        public void OnRequestedToMode(int mode)
        {
            if (mode == 1)
            {
                Console.WriteLine("Load messages ... ");
                OnRequestedToLoadMessages();
            }
            else if (mode == 2)
            {
                Console.WriteLine("Add new message: ");
                OnRequestedToAddMessage();
            }
            else if (mode == 3)
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 通过发言人 执行 回到主菜单
        /// </summary>
        public void OnRequestedToShowMenu()
        {
            presenter.ShowMenuScreen();
        }

        /// <summary>
        /// 通过 发言人 执行加载所有信息
        /// </summary>
        public void OnRequestedToLoadMessages()
        {
            if (presenter != null)
                presenter.LoadMessages();
        }

        /// <summary>
        /// 通过 发言人 执行添加信息
        /// </summary>
        public void OnRequestedToAddMessage()
        {
            Console.Write("id: ");
            long id = Convert.ToInt64(Console.ReadLine());
            Console.Write("content: ");
            string content = Console.ReadLine();
            DateTime created = DateTime.Now;
            Message message = new Message(id, content, created);
            presenter.AddMessage(message);
        }

        /// <summary>
        /// 此视图对象返回到主菜单。询问用户执行的步骤序号
        /// </summary>
        public void ShowInitialScreen()
        {
            Console.WriteLine();
            Console.WriteLine("*** Choose Mode ***");
            Console.WriteLine("1. Print out all messages.");
            Console.WriteLine("2. Add a new message.");
            Console.WriteLine("3. Exit program.");

            Console.Write("Enter your choice: ");
            int mode = Convert.ToInt16(Console.ReadLine());
            OnRequestedToMode(mode);
        }

        /// <summary>
        /// 此视图对象显示 实体集合 信息
        /// </summary>
        public void ShowMessages(IList<Message> messages)
        {
            Console.WriteLine($"{messages.Count} message(s):");
            PrintMessages(messages);
        }

        /// <summary>
        /// 此视图对象显示提醒信息
        /// </summary>
        public void ShowNotification(string msg)
        {
            Console.WriteLine($"{msg}");
        }

        /// <summary>
        /// 具体显示信息的方法
        /// </summary>
        private void PrintMessages(IList<Message> messages)
        {
            foreach (Message message in messages)
            {
                Console.WriteLine($" - {message}");
            }
        }
    }
}
