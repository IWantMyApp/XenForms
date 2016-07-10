using System;
using NUnit.Framework;
using XenForms.Core.Designer.Reactions;
using XenForms.Core.Messages;
using XenForms.Core.Networking;
using XenForms.Designer.Tests.EmptyFakes;

namespace XenForms.Designer.Tests.Reactions
{
    [TestFixture]
    public class TestReactions
    {
        [SetUp]
        public void BeforeTest()
        {
            Reaction.Reset();
        }


        [Test]
        public void Reset_clears_internal_data()
        {
            Reaction.Register<FakeSimpleRequest, FakeTrackRequestReaction>();
            Assert.IsTrue(Reaction.Actions.Count == 1, "There should be one action.");

            Reaction.Reset();

            Assert.IsTrue(Reaction.Actions.Count == 0, "Action check failed.");
            Assert.IsTrue(Reaction.Creators.Count == 0, "Creators check failed.");
        }


        [Test]
        public void Register_method_with_no_arguments()
        {
            Reaction.Register<FakeSimpleRequest, FakeTrackRequestReaction>();

            Assert.IsTrue(Reaction.Actions.Count == 1, "One action should return.");
            Assert.IsTrue(Reaction.Creators.Count == 0, "No creators were added.");
        }


        [Test]
        public void Register_method_with_lambda()
        {
            Reaction.Register<FakeSimpleRequest, FakeTrackRequestReaction>(() => new FakeTrackRequestReaction("hello"));

            Assert.IsTrue(Reaction.Actions.Count == 1, "One action should return.");
            Assert.IsTrue(Reaction.Creators.Count == 1, "creators were added.");
        }


        [Test]
        public void Execution_of_Register_called_without_passing_lambda_when_needed()
        {
            var request = XenMessage.Create<FakeSimpleRequest>();
            var ctx = new XenMessageContext {Request = request};

            Reaction.Register<FakeSimpleRequest, FakeTrackRequestReaction>();

            /*
                The FakeTrackRequestDesignerAction class does not have a parameterless constructor.
                It should either:
                    1) Have a parameterless ctor created
                    2) Have invoked the DesignerAction.Register overload to create an object
            */

            // Acting & Assert
            Assert.Throws<InvalidOperationException>(() => Reaction.Execute(ctx));
        }


        [Test]
        public void Execution_of_Register_with_lambda_passed_and_success()
        {
            var request = XenMessage.Create<FakeSimpleRequest>();
            var ctx = new XenMessageContext { Request = request };

            Reaction.Register<FakeSimpleRequest, FakeTrackRequestReaction>(() => new FakeTrackRequestReaction("Hello!"));
            var handled = Reaction.Execute(ctx);

            Assert.IsTrue(handled, "The OnExecute method should have returned true.");
            Assert.AreEqual(ctx.Message, "Hello!", "Data did not flow through the context.");
        }

         
        [Test]
        [Description("When a request arrives that the designer cannot handle.")]
        public void Test_execute_called_and_no_action_found()
        {
            // Note how FakeCompletedRequest and FakeSimpleRequest are different types
            Reaction.Register<FakeSimpleRequest, FakeAttachReaction>();
            var request = XenMessage.Create<FakeCompletedRequest>();
            var ctx = new XenMessageContext { Request = request };
            
            var handled = Reaction.Execute(ctx);

            Assert.IsFalse(handled);
        }
    }
}
