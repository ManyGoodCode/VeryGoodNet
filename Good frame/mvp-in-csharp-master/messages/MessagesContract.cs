using System.Collections.Generic;
using mvp_in_csharp.data;
namespace mvp_in_csharp.messages
{
    /// <summary>
    /// 视图对象
    /// </summary>
    public interface IMessagesView
    {
        /// <summary>
        /// 界面。界面对象通过注入 发言人 IMessagesPresenter 
        /// 之后所有的界面展示都交给发言人来调用
        /// </summary>
        void SetPresenter(IMessagesPresenter presenter);

        /// <summary>
        /// 通过发言人 执行 回到主菜单
        /// </summary>
        void OnRequestedToShowMenu();


        /// <summary>
        /// 主菜单执行序号【交给发言人来发言】
        /// 1. 加载所有信息
        /// 2. 添加信息
        /// 3. 退出
        /// </summary>
        void OnRequestedToMode(int mode);

        /// <summary>
        /// 通过 发言人 执行加载所有信息
        /// </summary>
        void OnRequestedToLoadMessages();

        /// <summary>
        /// 通过 发言人 执行添加信息
        /// </summary>
        void OnRequestedToAddMessage();

        /// <summary>
        /// 视图对象返回到主菜单
        /// </summary>
        void ShowInitialScreen();

        /// <summary>
        /// 视图对象显示 实体集合 信息
        /// </summary>
        void ShowMessages(IList<Message> messages);

        /// <summary>
        /// 视图对象显示提醒信息
        /// </summary>
        void ShowNotification(string msg);
    }

    public interface IMessagesPresenter
    {
        /// <summary>
        /// 视图对象回到主界面
        /// </summary>
        void ShowMenuScreen();

        /// <summary>
        /// 从 仓库 加载所有实体 , 视图对象展示执行结果 , 展示完后视图对象回到主界面
        /// </summary>
        void LoadMessages();

        /// <summary>
        /// 往仓库添加信息对象 , 视图对象展示执行结果 , 展示完后视图对象回到主界面
        /// </summary>
        void AddMessage(Message message);

        /// <summary>
        /// 从仓库删除指定Id的对象 , 视图对象展示执行结果 , 展示完后视图对象回到主界面
        /// </summary>
        void RemoveMessage(long id);
    }
}
