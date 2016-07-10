using System;
using NUnit.Framework;
using XenForms.Core.Messages;
using XenForms.Core.Platform.Reflection;

namespace XenForms.Designer.Tests.Messages
{
    [TestFixture]
    public class TestXenMessage
    {

        public class SomeXenMessage : XenMessage
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
        }


        [Test]
        public void Builder_should_populate_properties()
        {
            // Act
            var msg = XenMessage.Create<SomeXenMessage>();

            Assert.IsNotNull(msg);
            Assert.IsTrue(msg.MessageId.Length > 0);
            Assert.AreEqual(msg.Action, nameof(SomeXenMessage));
            Assert.IsTrue(msg.Time.Year == DateTime.Now.Year);
        }


        [Test]
        public void TryParse_from_valid_json_literal()
        {
            var json = @"{  
                            'stringProperty': 'Michael Davis',
                            'intProperty': 31,
                            'time': '2015-05-03T00:00:00',
                            'action': 'SomeXenMessage',
                            'messageId': '123'  
                        }";

            // Act
            XenMessage baseMsg;
            var baseParsed = XenMessage.TryParse(json, out baseMsg);

            Assert.IsTrue(baseParsed, "The json literal wasn't parsed.");
            Assert.IsNotNull(baseMsg, "The baseMsg was null.");

            Assert.IsTrue
            (
                baseMsg.Action == nameof(SomeXenMessage)
                && baseMsg.MessageId == "123"
                && baseMsg.Time == new DateTime(2015, 5, 3),
                "The XenMessage comparison failed."
            );

            // Act
            SomeXenMessage subclassMsg;
            var subclassParsed = XenMessage.TryParse(json, out subclassMsg);

            Assert.IsTrue(subclassParsed, "The subclass did not parse.");
            Assert.IsNotNull(subclassMsg, "The subclass message is null.");

            // Act
            SomeXenMessage genericMsg;
            var genericParsed = XenMessage.TryParse(json, out genericMsg);

            Assert.IsTrue(genericParsed, "The generic did not parse.");
            Assert.IsNotNull(genericMsg, "The generic is null.");

            // Act
            object typedMessage;
            var typedPassed = XenMessage.TryParse(typeof (SomeXenMessage), json, out typedMessage);

            Assert.IsTrue(typedPassed, "The typed message did not parse.");
            Assert.IsNotNull(typedMessage, "The typed message is null.");
            Assert.IsInstanceOf<SomeXenMessage>(typedMessage, "The object was not the correct type.");
        }


        [Test]
        public void TryParse_from_invalid_json_literals()
        {
            XenMessage msg;

            // Acting & Asserting
            Assert.IsFalse(XenMessage.TryParse(null, out msg), "The nullString check failed.");
            Assert.IsFalse(XenMessage.TryParse("", out msg), "The empty check failed.");
            Assert.IsFalse(XenMessage.TryParse("   ", out msg), "The only whitespace check failed.");
            Assert.IsFalse(XenMessage.TryParse("random", out msg), "The random string check failed.");

            Assert.IsFalse(XenMessage.TryParse("{}", out msg), "The obj schema check failed.");
            Assert.IsFalse(XenMessage.TryParse("{'property':'value'}", out msg), "Valid json obj; invalid schema check failed.");
        }


        [Test]
        public void Read_action_property_and_create_the_corresponding_object_from_json()
        {
            // Json.Net requires you know the .NET object type when attempting to deserialize the json
            // This method inspects the XenMessage's action property, and creates an object of that type.

            var json = @"{  
                            'stringProperty': 'Michael Davis',
                            'intProperty': 31,
                            'time': '2015-05-03T00:00:00',
                            'action': 'SomeXenMessage',
                            'messageId': '123'  
                        }";

            var finder = new XenMessageFinder();

            // Act
            XenMessage msg;
            var created = XenMessage.TryParseFromAction(finder, json, out msg);

            Assert.IsTrue(created, "The json did not parse.");
            Assert.IsInstanceOf<SomeXenMessage>(msg, "The resultant XenMessage was not the right type.");

            var castedMsg = msg as SomeXenMessage;

            if (castedMsg == null)
            {
                Assert.Fail("Stopping test execution for failed conversion.");
            }

            Assert.IsTrue
            (
                castedMsg.IntProperty == 31 && castedMsg.StringProperty == "Michael Davis",
                "Property inspection failed."
            );
        }
    }
}
