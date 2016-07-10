using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using XenForms.Core.Reflection;

namespace XenForms.Core.Messages
{
    /// <summary>
    /// A data structure used as a request from the toolbox or response from the designer.
    /// Subclasses should add properties related to it's purpose. 
    /// They will be serialized to JSON.
    /// </summary>
    public abstract class XenMessage
    {
        /// <summary>
        /// The time the <see cref="XenMessage"/> was created, not sent.
        /// </summary>
        public DateTime Time { get; set; }


        /// <summary>
        /// The message's typename.
        /// </summary>
        public string Action { get; set; }


        /// <summary>
        /// A unique identifier for a particular instance.
        /// </summary>
        public string MessageId { get; set; }


        /// <summary>
        /// The next <see cref="XenMessage"/> that should be executed.
        /// </summary>
        public string NextSuggestedMessage { get; set; }


        #region Utilities


        public void Suggest<T>() where T : XenMessage
        {
            NextSuggestedMessage = typeof(T).Name;
        }


        public bool Is<T>() where T : XenMessage
        {
            return typeof (T).Name == Action;
        }


        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };


        #endregion


        #region Converters & Factories


        /// <summary>
        /// Create a strongly-typed <see cref="XenMessage"/>.
        /// The <see cref="XenMessage"/> factory methods must be used to instantiate a <see cref="XenMessage"/> subclass.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        public static TMessage Create<TMessage>() where TMessage : XenMessage, new()
        {
            var message = new TMessage
            {
                Time = DateTime.Now,
                Action = typeof(TMessage).Name,
                MessageId = Guid.NewGuid().ToString(),
            };

            return message;
        }


        /// <summary>
        /// Create a <see cref="XenMessage"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static XenMessage Create(Type type)
        {
            var message = Activator.CreateInstance(type) as XenMessage;
            if (message == null) return null;

            message.Time = DateTime.Today;
            message.Action = type.Name;
            message.MessageId = Guid.NewGuid().ToString();
            
            return message;
        }


        /// <summary>
        /// Create an object of the type identified by <see cref="Action"/>.
        /// </summary>
        /// <param name="finder">Responsible for instantiating an object of the required type</param>
        /// <param name="message">JSON</param>
        /// <param name="output">Result</param>
        /// <returns>True if converted; otherwise, false.</returns>
        public static bool TryParseFromAction(IXenMessageFinder finder, string message, out XenMessage output)
        {
            try
            {
                object temp;

                var converted = TryParse(typeof(UnknownXenMessage), message, out temp);

                if (!converted)
                {
                    output = null;
                    return false;
                }

                var msgTemp = temp as XenMessage;
                var actionName = string.Empty;

                if (msgTemp != null)
                {
                    actionName = msgTemp.Action;
                }

                if (string.IsNullOrWhiteSpace(actionName))
                {
                    output = null;
                    return false;
                }

                var actionType = finder.Find(actionName);

                if (actionType == null)
                {
                    output = null;
                    return false;
                }

                object tempActionObject;
                var parsed = TryParse(actionType, message, out tempActionObject);

                var actionObject = tempActionObject as XenMessage;
                output = actionObject;

                return parsed && actionObject != null;
            }
            catch (Exception)
            {
                output = null;
                return false;
            }
        }


        /// <summary>
        /// Parse and create a strongly-typed <see cref="XenMessage"/>.
        /// </summary>
        /// <typeparam name="TXenMessage"></typeparam>
        /// <param name="message">JSON</param>
        /// <param name="output">Result</param>
        /// <returns>True if the message was parsed; otherwise, false.</returns>
        public static bool TryParse<TXenMessage>(string message, out TXenMessage output) where TXenMessage : XenMessage
        {
            object temp;
            var converted = TryParse(typeof(TXenMessage), message, out temp);

            output = temp as TXenMessage;
            return converted;
        }


        /// <summary>
        /// Parse and create a subclass of <see cref="XenMessage"/>.
        /// </summary>
        /// <param name="message">JSON</param>
        /// <param name="output">Result</param>
        /// <returns>True if the message was parsed; otherwise, false.</returns>
        public static bool TryParse(string message, out XenMessage output)
        {
            object temp;
            var converted = TryParse(typeof(XenMessage), message, out temp);

            output = temp as XenMessage;
            return converted;
        }


        /// <summary>
        /// Parse and create a <see cref="XenMessage"/>.
        /// </summary>
        /// <param name="type">The type of message to create</param>
        /// <param name="message">JSON</param>
        /// <param name="output">Result</param>
        /// <returns>True if the message was parsed; otherwise, false.</returns>
        public static bool TryParse(Type type, string message, out object output)
        {
            output = null;

            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            if (!message.Trim().StartsWith("{"))
            {
                return false;
            }

            try
            {
                output = JsonConvert.DeserializeObject(message, type);
            }
            catch (Exception)
            {
                if (!message.Contains("action"))
                {
                    return false;
                }
            }

            if (output == null)
            {
                try
                {
                    output = JsonConvert.DeserializeObject(message, typeof(UnknownXenMessage));
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// JSON serialization of the message.
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.None, JsonSettings);
            return json;
        }


        #endregion
    }
}