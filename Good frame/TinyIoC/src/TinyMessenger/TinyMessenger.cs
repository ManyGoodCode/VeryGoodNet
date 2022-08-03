using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
#if NETSTANDARD
using System.Threading.Tasks;
#endif

namespace TinyMessenger
{
    public interface ITinyMessage
    {
        object Sender { get; }
    }

    public abstract class TinyMessageBase : ITinyMessage
    {
        private WeakReference sender;
        public object Sender
        {
            get
            {
                return (sender == null) ? null : sender.Target;
            }
        }

        public TinyMessageBase(object sender)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");

            sender = new WeakReference(sender);
        }
    }

    public class GenericTinyMessage<TContent> : TinyMessageBase
    {
        public TContent Content { get; protected set; }

        public GenericTinyMessage(object sender, TContent content)
            : base(sender)
        {
            Content = content;
        }
    }


    public class CancellableGenericTinyMessage<TContent> : TinyMessageBase
    {
        /// <summary>
        /// 取消动作
        /// </summary>
        public Action Cancel { get; protected set; }

        public TContent Content { get; protected set; }

        public CancellableGenericTinyMessage(object sender, TContent content, Action cancelAction)
            : base(sender)
        {
            if (cancelAction == null)
                throw new ArgumentNullException("cancelAction");

            Content = content;
            Cancel = cancelAction;
        }
    }

    public sealed class TinyMessageSubscriptionToken : IDisposable
    {
        private WeakReference hub;
        private Type messageType;

        public TinyMessageSubscriptionToken(ITinyMessengerHub hub, Type messageType)
        {
            if (hub == null)
                throw new ArgumentNullException("hub");

            if (!typeof(ITinyMessage).IsAssignableFrom(messageType))
                throw new ArgumentOutOfRangeException("messageType");

            this.hub = new WeakReference(hub);
            this.messageType = messageType;
        }

        public void Dispose()
        {
            if (this.hub.IsAlive)
            {
                ITinyMessengerHub iHub = this.hub.Target as ITinyMessengerHub;
                if (iHub == null)
                {
                    return;
                }

                MethodInfo unsubscribeMethod = typeof(ITinyMessengerHub).GetMethod(
                    "Unsubscribe",
                    new Type[]
                    {
                            typeof(TinyMessageSubscriptionToken)
                    });

                unsubscribeMethod = unsubscribeMethod.MakeGenericMethod(messageType);
                unsubscribeMethod.Invoke(hub, new object[]
                {
                    this 
                });
            }

            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 订阅
    /// </summary>
    public interface ITinyMessageSubscription
    {
        TinyMessageSubscriptionToken SubscriptionToken { get; }
        bool ShouldAttemptDelivery(ITinyMessage message);
        void Deliver(ITinyMessage message);
    }

    public interface ITinyMessageProxy
    {
        void Deliver(ITinyMessage message, ITinyMessageSubscription subscription);
    }


    public sealed class DefaultTinyMessageProxy : ITinyMessageProxy
    {
        private static readonly DefaultTinyMessageProxy instance = new DefaultTinyMessageProxy();

        // 静态构造函数
        static DefaultTinyMessageProxy()
        {
        }

        public static DefaultTinyMessageProxy Instance
        {
            get
            {
                return instance;
            }
        }

        // 构造函数
        private DefaultTinyMessageProxy()
        {
        }

        public void Deliver(ITinyMessage message, ITinyMessageSubscription subscription)
        {
            subscription.Deliver(message);
        }
    }


    public class TinyMessengerSubscriptionException : Exception
    {
        private const string ERROR_TEXT = "Unable to add subscription for {0} : {1}";

        public TinyMessengerSubscriptionException(Type messageType, string reason)
            : base(string.Format(ERROR_TEXT, messageType, reason))
        {

        }

        public TinyMessengerSubscriptionException(Type messageType, string reason, Exception innerException)
            : base(string.Format(ERROR_TEXT, messageType, reason), innerException)
        {

        }
    }

    public interface ITinyMessengerHub
    {
        TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction) where TMessage : class, ITinyMessage;

        TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, ITinyMessageProxy proxy) 
            where TMessage : class, ITinyMessage;

        TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, bool useStrongReferences)
            where TMessage : class, ITinyMessage;

        TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, bool useStrongReferences, ITinyMessageProxy proxy)
            where TMessage : class, ITinyMessage;

        TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter)
            where TMessage : class, ITinyMessage;

        TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, ITinyMessageProxy proxy) 
            where TMessage : class, ITinyMessage;


        TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, bool useStrongReferences) 
            where TMessage : class, ITinyMessage;


        TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, bool useStrongReferences, ITinyMessageProxy proxy) 
            where TMessage : class, ITinyMessage;

        void Unsubscribe<TMessage>(TinyMessageSubscriptionToken subscriptionToken) where TMessage : class, ITinyMessage;

        void Publish<TMessage>(TMessage message) where TMessage : class, ITinyMessage;

        void PublishAsync<TMessage>(TMessage message) where TMessage : class, ITinyMessage;

        void PublishAsync<TMessage>(TMessage message, AsyncCallback callback) where TMessage : class, ITinyMessage;
    }


    public sealed class TinyMessengerHub : ITinyMessengerHub
    {
        private class WeakTinyMessageSubscription<TMessage> : ITinyMessageSubscription
            where TMessage : class, ITinyMessage
        {
            protected TinyMessageSubscriptionToken _SubscriptionToken;
            protected WeakReference _DeliveryAction;
            protected WeakReference _MessageFilter;

            public TinyMessageSubscriptionToken SubscriptionToken
            {
                get { return _SubscriptionToken; }
            }

            public bool ShouldAttemptDelivery(ITinyMessage message)
            {
                if (!(message is TMessage))
                    return false;

                if (!_DeliveryAction.IsAlive)
                    return false;

                if (!_MessageFilter.IsAlive)
                    return false;

                return ((Func<TMessage, bool>)_MessageFilter.Target).Invoke(message as TMessage);
            }

            public void Deliver(ITinyMessage message)
            {
                if (!(message is TMessage))
                    throw new ArgumentException("Message is not the correct type");

                if (!_DeliveryAction.IsAlive)
                    return;

                ((Action<TMessage>)_DeliveryAction.Target).Invoke(message as TMessage);
            }

            public WeakTinyMessageSubscription(TinyMessageSubscriptionToken subscriptionToken, Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter)
            {
                if (subscriptionToken == null)
                    throw new ArgumentNullException("subscriptionToken");

                if (deliveryAction == null)
                    throw new ArgumentNullException("deliveryAction");

                if (messageFilter == null)
                    throw new ArgumentNullException("messageFilter");

                _SubscriptionToken = subscriptionToken;
                _DeliveryAction = new WeakReference(deliveryAction);
                _MessageFilter = new WeakReference(messageFilter);
            }
        }

        private class StrongTinyMessageSubscription<TMessage> : ITinyMessageSubscription
            where TMessage : class, ITinyMessage
        {
            protected TinyMessageSubscriptionToken _SubscriptionToken;
            protected Action<TMessage> _DeliveryAction;
            protected Func<TMessage, bool> _MessageFilter;

            public TinyMessageSubscriptionToken SubscriptionToken
            {
                get { return _SubscriptionToken; }
            }

            public bool ShouldAttemptDelivery(ITinyMessage message)
            {
                if (!(message is TMessage))
                    return false;

                return _MessageFilter.Invoke(message as TMessage);
            }

            public void Deliver(ITinyMessage message)
            {
                if (!(message is TMessage))
                    throw new ArgumentException("Message is not the correct type");

                _DeliveryAction.Invoke(message as TMessage);
            }


            public StrongTinyMessageSubscription(TinyMessageSubscriptionToken subscriptionToken, Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter)
            {
                if (subscriptionToken == null)
                    throw new ArgumentNullException("subscriptionToken");

                if (deliveryAction == null)
                    throw new ArgumentNullException("deliveryAction");

                if (messageFilter == null)
                    throw new ArgumentNullException("messageFilter");

                _SubscriptionToken = subscriptionToken;
                _DeliveryAction = deliveryAction;
                _MessageFilter = messageFilter;
            }
        }
        private class SubscriptionItem
        {
            public ITinyMessageProxy Proxy { get; private set; }
            public ITinyMessageSubscription Subscription { get; private set; }

            public SubscriptionItem(ITinyMessageProxy proxy, ITinyMessageSubscription subscription)
            {
                Proxy = proxy;
                Subscription = subscription;
            }
        }

        private readonly object _SubscriptionsPadlock = new object();
        private readonly Dictionary<Type, List<SubscriptionItem>> _Subscriptions = new Dictionary<Type, List<SubscriptionItem>>();

        public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction) where TMessage : class, ITinyMessage
        {
            return AddSubscriptionInternal<TMessage>(deliveryAction, (m) => true, true, DefaultTinyMessageProxy.Instance);
        }

        public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, ITinyMessageProxy proxy) 
            where TMessage : class, ITinyMessage
        {
            return AddSubscriptionInternal<TMessage>(deliveryAction, (m) => true, true, proxy);
        }


        public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, bool useStrongReferences) 
            where TMessage : class, ITinyMessage
        {
            return AddSubscriptionInternal<TMessage>(deliveryAction, (m) => true, useStrongReferences, DefaultTinyMessageProxy.Instance);
        }

        public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, bool useStrongReferences, ITinyMessageProxy proxy) 
            where TMessage : class, ITinyMessage
        {
            return AddSubscriptionInternal<TMessage>(deliveryAction, (m) => true, useStrongReferences, proxy);
        }

        public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter) 
            where TMessage : class, ITinyMessage
        {
            return AddSubscriptionInternal<TMessage>(deliveryAction, messageFilter, true, DefaultTinyMessageProxy.Instance);
        }



        public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, ITinyMessageProxy proxy) 
            where TMessage : class, ITinyMessage
        {
            return AddSubscriptionInternal<TMessage>(deliveryAction, messageFilter, true, proxy);
        }


        public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, bool useStrongReferences)
            where TMessage : class, ITinyMessage
        {
            return AddSubscriptionInternal<TMessage>(deliveryAction, messageFilter, useStrongReferences, DefaultTinyMessageProxy.Instance);
        }


        public TinyMessageSubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, bool useStrongReferences, ITinyMessageProxy proxy)
            where TMessage : class, ITinyMessage
        {
            return AddSubscriptionInternal<TMessage>(deliveryAction, messageFilter, useStrongReferences, proxy);
        }


        public void Unsubscribe<TMessage>(TinyMessageSubscriptionToken subscriptionToken) 
            where TMessage : class, ITinyMessage
        {
            RemoveSubscriptionInternal<TMessage>(subscriptionToken);
        }


        public void Publish<TMessage>(TMessage message) where TMessage : class, ITinyMessage
        {
            PublishInternal<TMessage>(message);
        }

        public void PublishAsync<TMessage>(TMessage message) where TMessage : class, ITinyMessage
        {
            PublishAsyncInternal<TMessage>(message, null);
        }

        public void PublishAsync<TMessage>(TMessage message, AsyncCallback callback) where TMessage : class, ITinyMessage
        {
            PublishAsyncInternal<TMessage>(message, callback);
        }

        #region Internal Methods
        private TinyMessageSubscriptionToken AddSubscriptionInternal<TMessage>(Action<TMessage> deliveryAction, Func<TMessage, bool> messageFilter, bool strongReference, ITinyMessageProxy proxy)
                where TMessage : class, ITinyMessage
        {
            if (deliveryAction == null)
                throw new ArgumentNullException("deliveryAction");

            if (messageFilter == null)
                throw new ArgumentNullException("messageFilter");

            if (proxy == null)
                throw new ArgumentNullException("proxy");

            lock (_SubscriptionsPadlock)
            {
                List<SubscriptionItem> currentSubscriptions;

                if (!_Subscriptions.TryGetValue(typeof(TMessage), out currentSubscriptions))
                {
                    currentSubscriptions = new List<SubscriptionItem>();
                    _Subscriptions[typeof(TMessage)] = currentSubscriptions;
                }

                var subscriptionToken = new TinyMessageSubscriptionToken(this, typeof(TMessage));

                ITinyMessageSubscription subscription;
                if (strongReference)
                    subscription = new StrongTinyMessageSubscription<TMessage>(subscriptionToken, deliveryAction, messageFilter);
                else
                    subscription = new WeakTinyMessageSubscription<TMessage>(subscriptionToken, deliveryAction, messageFilter);

                currentSubscriptions.Add(new SubscriptionItem(proxy, subscription));

                return subscriptionToken;
            }
        }

        private void RemoveSubscriptionInternal<TMessage>(TinyMessageSubscriptionToken subscriptionToken)
                where TMessage : class, ITinyMessage
        {
            if (subscriptionToken == null)
                throw new ArgumentNullException("subscriptionToken");

            lock (_SubscriptionsPadlock)
            {
                List<SubscriptionItem> currentSubscriptions;
                if (!_Subscriptions.TryGetValue(typeof(TMessage), out currentSubscriptions))
                    return;

                var currentlySubscribed = (from sub in currentSubscriptions
                                           where object.ReferenceEquals(sub.Subscription.SubscriptionToken, subscriptionToken)
                                           select sub).ToList();

                currentlySubscribed.ForEach(sub => currentSubscriptions.Remove(sub));
            }
        }

        private void PublishInternal<TMessage>(TMessage message)
                where TMessage : class, ITinyMessage
        {
            if (message == null)
                throw new ArgumentNullException("message");

            List<SubscriptionItem> currentlySubscribed;
            lock (_SubscriptionsPadlock)
            {
                List<SubscriptionItem> currentSubscriptions;
                if (!_Subscriptions.TryGetValue(typeof(TMessage), out currentSubscriptions))
                    return;

                currentlySubscribed = (from sub in currentSubscriptions
                                       where sub.Subscription.ShouldAttemptDelivery(message)
                                       select sub).ToList();
            }

            currentlySubscribed.ForEach(sub =>
            {
                try
                {
                    sub.Proxy.Deliver(message, sub.Subscription);
                }
                catch (Exception)
                {

                }
            });
        }

#if !NETSTANDARD
        private void PublishAsyncInternal<TMessage>(TMessage message, AsyncCallback callback) where TMessage : class, ITinyMessage
        {
            Action publishAction = () => { PublishInternal<TMessage>(message); };

            publishAction.BeginInvoke(callback, null);
        }
#else
        private void PublishAsyncInternal<TMessage>(TMessage message, AsyncCallback callback) where TMessage : class, ITinyMessage
        {
            Task.Run(() => PublishInternal<TMessage>(message))
                .ContinueWith(t => callback?.Invoke(t));
        }
#endif

    }
}
