using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Designer.Workflow;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Core.Platform.Reflection;
using XenForms.Designer.Tests.EmptyFakes;

namespace XenForms.Designer.Tests.Workflow
{
    [TestFixture]
    public class TestDefaultDesignWorkflow
    {
        private XenMessageFinder _finder;
        private Mock<DesignServer> _server;


        [SetUp]
        public void BeforeTest()
        {
            _finder = new XenMessageFinder();
            _server = new Mock<DesignServer>();

            Reaction.Reset();
        }


        [Test]
        public void Starting_server_creates_consumer_thread()
        {
            var workflow = new DefaultDesignWorkflow(null, null);

            Assert.AreEqual(0, workflow.ConsumerThreads, "Consumer threads should be 0.");

            workflow.Start();

            SpinWait.SpinUntil(() => workflow.ConsumerThreads > 1, TimeSpan.FromSeconds(3));

            Assert.AreEqual(1, workflow.ConsumerThreads, "Workflow isn't running.");
        }


        [Test]
        public void Queing_one_message_without_handler_registered()
        {
            var msg = XenMessage.Create<FakeQueueRequest>();

            var json = msg.ToJson();
            var workflow = new DefaultDesignWorkflow(_finder, null);

            workflow.Start();
            workflow.Queue(json);
            SpinWait.SpinUntil(() => workflow.QueuedMessages > 1, TimeSpan.FromSeconds(3));
            Assert.AreEqual(1, workflow.QueuedMessages);
        }


        [Test]
        public void Queing_message_with_handler_registered()
        {
            var msg = XenMessage.Create<FakeQueueRequest>();
            var json = msg.ToJson();
            var workflow = new DefaultDesignWorkflow(_finder, _server.Object);
            
            var reaction = new FakeQueueReaction();

            Reaction.Register<FakeQueueRequest, FakeQueueReaction>(() => reaction);
            workflow.Start();
            workflow.Queue(json);

            SpinWait.SpinUntil(() => reaction.Context != null, TimeSpan.FromSeconds(3));
            Assert.IsNotNull(reaction.Context);

            _server.Verify(r => r.Send(It.IsAny<string>()), Times.AtMostOnce);
        }


        [Test]
        public void OkResponse_set_when_reaction_didnt_set_a_response()
        {
            var msg = XenMessage.Create<FakeQueueRequest>();
            var json = msg.ToJson();
            var workflow = new DefaultDesignWorkflow(_finder, _server.Object);

            var reaction = new FakeNoResponseReaction();
            Reaction.Register<FakeQueueRequest, FakeNoResponseReaction>(() => reaction);

            workflow.Start();
            workflow.Queue(json);

            SpinWait.SpinUntil(() => reaction.Completed, TimeSpan.FromSeconds(3));
            Assert.IsTrue(reaction.Completed);
            Assert.IsInstanceOf(typeof(OkResponse), reaction.Context.Response);
        }
    }
}
