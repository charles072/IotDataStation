using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Iot.Common.ClassLogger;
using Newtonsoft.Json;
using WebSocketSharp;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace IotDataServer.Common.Notification
{
    public class NotificationClient : IDisposable
    {
        private static readonly ClassLogger Logger = ClassLogManager.GetCurrentClassLogger();
        private static readonly Lazy<NotificationClient> Lazy = new Lazy<NotificationClient>(() => new NotificationClient());
        public static NotificationClient Instance => Lazy.Value;

        private bool _isSubscribed = false;
        private Task _notificationMessageWorkTask = null;
        private volatile bool _isNotificationMessageWorkRunnung = false;
        private volatile bool _shouldStopNotificationMessageWork = false;
        private readonly ConcurrentQueue<NotificationMessage> _notificationMessageQueue = new ConcurrentQueue<NotificationMessage>();

        private WebSocket _webSocket = null;
        private readonly Dictionary<string, List<INotificationObserver>> _observerDictionary = new Dictionary<string, List<INotificationObserver>>();

        private volatile bool _isConnecting = false;
        private volatile bool _isStartedConnect = false;
        public string ProgramType { get; set; }
        public string NotificationServerUri { get; private set; }
        public string ChannelId { get; private set; } = "";
        public bool IsAlive => _webSocket != null && _webSocket.IsAlive;
        public bool IsSubscribed
        {
            get
            {
                return _isSubscribed;
            }

        }

        private NotificationClient()
        {
            _isNotificationMessageWorkRunnung = false;
            _shouldStopNotificationMessageWork = false;
            _notificationMessageWorkTask = null;
        }

        public void Close()
        {
            _isStartedConnect = false;
            _isSubscribed = false;
            StopNotificationMessageWork();
            _webSocket?.Close();
        }

        public void Dispose()
        {
            Close();
            _webSocket = null;
        }


        public void Init(string programType, string notificationServerUri)
        {
            ProgramType = programType;
            NotificationServerUri = notificationServerUri;
        }

        public bool Connect(string notificationServerUri = "")
        {
            _isConnecting = true;
            if (!string.IsNullOrWhiteSpace(notificationServerUri))
            {
                NotificationServerUri = notificationServerUri;
            }

            if (string.IsNullOrWhiteSpace(NotificationServerUri))
            {
                Logger.Warn("Cannot find NotificationServerUri.");
                return false;
            }

            Close();

            try
            {
                _isStartedConnect = true;
                _webSocket = new WebSocket(NotificationServerUri);
                _webSocket.WaitTime = TimeSpan.FromMilliseconds(1000);
                _webSocket.OnOpen += WebSocketOnOpen;

                _webSocket.OnMessage += WebSocketOnMessage;
                _webSocket.OnError += WebSocketOnError;
                _webSocket.OnClose += WebSocketOnClose;
                //_webSocket.
#if DEBUG
                //_webSocket.Log.Level = LogLevel.Trace;
                _webSocket.WaitTime = TimeSpan.FromSeconds(10);
                _webSocket.EmitOnPing = true;
#endif
                _webSocket.Connect();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error in 'Connect'.");
                _isConnecting = false;
                return false;
            }

            return true;
        }

        private void WebSocketOnOpen(object sender, EventArgs eventArgs)
        {
            _isConnecting = false;
            StartNotificationMessageWork();

            string target = "";
            string messageType = "subscribe";
            string messageFunction = "";
            string message = "";
            SendMessage(target, messageType, messageFunction, message);
        }

        private void WebSocketOnClose(object o, CloseEventArgs closeEventArgs)
        {
            _isConnecting = false;
            Logger.Warn($"WebSocketOnClose: [{closeEventArgs.Code}]{closeEventArgs.Reason}");
            _isSubscribed = false;
        }

        private void WebSocketOnError(object o, ErrorEventArgs errorEventArgs)
        {
            _isConnecting = false;
            Logger.Error(errorEventArgs.Exception, $"WebSocketOnError: {errorEventArgs.Message}");
            _isSubscribed = false;
        }

        private void WebSocketOnMessage(object o, MessageEventArgs messageEventArgs)
        {
            if (messageEventArgs.IsPing)
            {
                return;
            }
            else if (messageEventArgs.IsText)
            {
                string message = messageEventArgs.Data;
                if (string.IsNullOrWhiteSpace(message))
                    Logger.Trace(messageEventArgs.Data);
                NotificationMessage notificationMessage = null;
                try
                {
                    notificationMessage = JsonConvert.DeserializeObject<NotificationMessage>(message);
                }
                catch (Exception e)
                {
                    notificationMessage = null;
                    Logger.Error(e, "Cannot JsonConvert.DeserializeObject<NotificationMessage>\n" + message);
                }

				if (notificationMessage != null && notificationMessage.Sender != ChannelId)
                {
                    switch (notificationMessage.MessageType)
                    {
                        case "subscribe":
                            OnGotSubscribeMessage(notificationMessage);
                            break;

                        case "sync":
                            OnGotSyncMessage(notificationMessage);
                            break;
						case "event":
							OnGotSyncMessage(notificationMessage);
							break;

                        default:
                            Logger.Warn("Not supported message type: " + notificationMessage.MessageType);
                            break;
                    }
                }
            }
            else
            {
                Logger.Error("NotSupported bin typ.");
            }
        }

        private void OnGotSyncMessage(NotificationMessage notificationMessage)
        {
            _notificationMessageQueue.Enqueue(notificationMessage);
        }

        private void OnGotSubscribeMessage(NotificationMessage notificationMessage)
        {
            ChannelId = notificationMessage.Sender;
            _isSubscribed = true;
        }

        private void SendMessage(string target, string messageType, string messageFunction, string message)
        {
            if (IsAlive)
            {
                var notificationMessage = new NotificationMessage(ChannelId, target, messageType, messageFunction, message);

                _webSocket.SendAsync(JsonConvert.SerializeObject(notificationMessage), (b =>
                {
                    SendMessageCompleted(b, notificationMessage);
                }));
            }
        }

		public void SendEventMessage(string target, string messageFunction, string message)
		{
			if (IsAlive)
			{
				SendMessage(target, "event", messageFunction, message);
			}
		}

		private void SendMessageCompleted(bool isSucceeded, NotificationMessage notificationMessage)
        {
            Logger.Info($"{isSucceeded}:{notificationMessage.MessageType}");
        }

        public void SendSyncMessage(string target, string messageFunction, string message)
        {
            if (IsAlive)
            {
                SendMessage(target, "sync", messageFunction, message);
            }
        }

        public void RegisterObserver(string id, INotificationObserver notificationObserver)
        {
            List<INotificationObserver> observers = null;
            if (_observerDictionary.TryGetValue(id, out observers))
            {
                if (!observers.Exists((x) => x == notificationObserver))
                {
                    observers.Add(notificationObserver);
                }
            }
            else
            {
                observers = new List<INotificationObserver>();
                observers.Add(notificationObserver);
                _observerDictionary[id] = observers;
            }

        }
        public void UnregisterObserver(string id, INotificationObserver notificationObserver)
        {
            List<INotificationObserver> observers = null;
            if (_observerDictionary.TryGetValue(id, out observers))
            {
                observers.Remove(notificationObserver);

                if (observers.Count == 0)
                {
                    _observerDictionary.Remove(id);
                }
            }
        }

        private void StartNotificationMessageWork()
        {
            _isNotificationMessageWorkRunnung = false;
            _shouldStopNotificationMessageWork = false;
            _notificationMessageWorkTask = Task.Factory.StartNew(DoNotificationMessageWork);
        }
        private void StopNotificationMessageWork()
        {
            if (_isNotificationMessageWorkRunnung && !_shouldStopNotificationMessageWork)
            {
                _shouldStopNotificationMessageWork = true;
                Thread.Sleep(200);
            }
            _notificationMessageWorkTask?.Dispose();
            _notificationMessageWorkTask = null;
        }

        private void DoNotificationMessageWork()
        {
            _isNotificationMessageWorkRunnung = true;
            try
            {
                while (!_shouldStopNotificationMessageWork)
                {
                    if (_notificationMessageQueue.TryDequeue(out NotificationMessage notificationMessage))
                    {
                        if (_observerDictionary.TryGetValue(notificationMessage.Target, out List<INotificationObserver> observers))
                        {
                            Parallel.ForEach(observers, observer =>
                            {
                                observer.OnReceiveNotificationMessage(notificationMessage);
                            });
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            finally
            {
                _isNotificationMessageWorkRunnung = false;
            }
        }

        public void WatchDog()
        {
            if (!IsAlive)
            {
                if (_isStartedConnect && !_isConnecting)
                {
                    Connect();
                }
            }
        }
    }
}