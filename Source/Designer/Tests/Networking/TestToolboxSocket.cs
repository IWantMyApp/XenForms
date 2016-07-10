using System;
using NUnit.Framework;
using XenForms.Core.Messages;
using XenForms.Designer.Tests.EmptyFakes;

namespace XenForms.Designer.Tests.Networking
{
    [TestFixture]
    public class TestToolboxSocket
    {
        [Test]
        public void Set_Endpoint_on_connect()
        {
            var socket = new FakeToolboxSocket();

            // Act
            socket.Connect("localhost", 9090);

            Assert.AreEqual("ws://localhost:9090/", socket.Endpoint, "Builtin formatting failed.");

            const string manual = "ws://manual:9191/";

            // Act
            socket.Connect(manual);

            Assert.AreEqual(manual, socket.Endpoint, "Manual entry failed.");
        }


        [Test]
        public void Invalid_data_to_connect()
        {
            var socket = new FakeToolboxSocket();

            // Acting & Asserting
            Assert.Throws<InvalidOperationException>(() => socket.Connect(null), "Null argument to connect.");
            Assert.Throws<InvalidOperationException>(() => socket.Connect("  ", 80), "Invalid host and port to connect.");
        }


        [Test]
        public void Send_method()
        {
            var socket = new FakeToolboxSocket();

            // Act
            socket.Send("Testing...");
            
            Assert.AreEqual("Testing...", socket.Incoming, "The string overload failed.");

            var msg = XenMessage.Create<FakeSimpleMessage>();

            // Act
            socket.Send(msg);

            StringAssert.Contains(nameof(FakeSimpleMessage), socket.Incoming, "XenMessage overload failed.");
        }


        [Test]
        public void IsConnected_property()
        {
            var socket = new FakeToolboxSocket();

            // Act
            // InvokeConnectEvent was created for unit testing purposes.

            socket.Connect("host", 9090);
            socket.InvokeConnectEvent();

            Assert.IsTrue(socket.IsConnected, "The connected event was invoked.");

            // Act
            // InvokeDisconnectEvent was created for unit testing purposes.
            socket.InvokeDisconnectEvent();
            Assert.IsFalse(socket.IsConnected, "The disconnected event was invoked.");
        }
    }
}
