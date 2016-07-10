using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Reflection;

namespace XenForms.Core.Designer.Workflow
{
    /// <summary>
    /// Responsible for dispatching toolbox messages to the appropriate <see cref="Reaction"/>.
    /// </summary>
    public sealed class DefaultDesignWorkflow : DesignWorkflow
    {
        public int ConsumerThreads { get; private set; }
        public int QueuedMessages => _queue.Count;

        private readonly object _lock = new object();
        private readonly IXenMessageFinder _messageFinder;
        private CancellationTokenSource _cancellationSource = new CancellationTokenSource();
        private BlockingCollection<string> _queue = new BlockingCollection<string>();
        private Task _consumerThread;
        private const int AddWaitTimeMs = 500;
        private const int QueueDelayTimeMs = 250;
        private long _messageCount = 0;


        /// <summary>
        /// Instantiate a <see cref="DefaultDesignWorkflow"/>.
        /// </summary>
        /// <param name="messageFinder">Instantiates objects, using reflection.</param>
        /// <param name="server">When a message has been dispatched, the response is sent to the toolbox.</param>
        public DefaultDesignWorkflow(IXenMessageFinder messageFinder, DesignServer server) : base(server)
        {
            _messageFinder = messageFinder;
        }

        
        /// <summary>
        /// Queue a message to be executed.
        /// </summary>
        /// <param name="message"></param>
        public override void Queue(string message)
        {
            if (!_queue.IsCompleted)
            {
                _queue.TryAdd(message, AddWaitTimeMs, _cancellationSource.Token);
            }
        }


        /// <summary>
        /// Start the workflow engine.
        /// </summary>
        public override void Start()
        {
            _cancellationSource = new CancellationTokenSource();
            _queue = new BlockingCollection<string>();

            // create 1 new consumer thread
            if (_consumerThread == null)
            {
                Task.Run(() => Consumer());
            }
        }


        /// <summary>
        /// Stop the workflow and reset all internal settings.
        /// </summary>
        public override void Shutdown()
        {
            lock (_lock)
            {
                ConsumerThreads--;
            }

            _queue.CompleteAdding();
            _cancellationSource.Cancel();
            _consumerThread = null;
        }


        /// <summary>
        /// Execute the queued message.
        /// </summary>
        private void Consumer()
        {
            try
            {
                lock (_lock)
                {
                    ConsumerThreads++;
                }

                foreach (var message in _queue.GetConsumingEnumerable(_cancellationSource.Token))
                {
                    if (_queue.IsCompleted) return;

                    var request = Parse(message);
                    // skip null request
                    if (request == null) continue;

                    var context = new XenMessageContext
                    {
                        Message = message,
                        ShouldQueue = false,
                        Request = request,
                        Response = null
                    };

                    ExecuteDesignAction(context);

                    lock (_lock)
                    {
                        _messageCount++;
                    }

                    // add it back for later processing
                    if (context.ShouldQueue)
                    {
                        _queue.TryAdd(message, AddWaitTimeMs, _cancellationSource.Token);
                        Task.Delay(QueueDelayTimeMs).Wait();
                    }
                    else
                    {
                        try
                        {
                            var json = context.Response?.ToJson();

                            if (!string.IsNullOrWhiteSpace(json))
                            {
                                Server.Send(json);
                            }
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }

                    context.Response = null;
                }
            }
            catch (OperationCanceledException)
            {
                // ignored
            }
        }


        /// <summary>
        /// Return the corresponding XenMessage.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private XenMessage Parse(string message)
        {
            XenMessage request = null;

            if (!string.IsNullOrWhiteSpace(message))
            {
               XenMessage.TryParseFromAction(_messageFinder, message, out request);
            }

            return request;
        }


        private void ExecuteDesignAction(XenMessageContext ctx)
        {
            // Send request to designer.
            // A true response does not guarantee that a design action set the response object.
            // A false response means that an action hasn't been registerd to handle the request.

            bool executed;
            var unhandledException = false;
            string exceptionMessage = null;

            try
            {
                executed = Reaction.Execute(ctx);
                ctx.ShouldQueue = !executed;
            }
            catch (Exception ex)
            {
                executed = true;
                ctx.ShouldQueue = false;
                unhandledException = true;
                exceptionMessage = ex.ToString();
            }
            
            if (executed)
            {
                // If a specific response hasn't been created for a request type, send an OkResponse to the toolbox.
                if (ctx.Response == null || ctx.Response is UnknownXenMessage)
                {
                    ctx.SetResponse<OkResponse>(r =>
                    {
                        r.ReplyingTo = ctx.Request.MessageId;
                    });
                }

                var response = ctx.Response as Response;

                if (response != null)
                {
                    response.UnhandledExceptionOccurred = unhandledException;
                    response.ExceptionMessage = exceptionMessage;
                }
            }
        }
    }
}