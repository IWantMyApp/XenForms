using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using XenForms.Core.Widgets;

namespace XenForms.Designer.Tests.Widgets
{
    [TestFixture]
    public class TestXenWidgetExtensions
    {
        [Test]
        public void Should_return_one_node()
        {
            var w1 = new XenWidget();
            var nodes = w1.GetNodeAndDescendants();
            Assert.IsTrue(nodes.Count() == 1);
        }


        [Test]
        public void Should_return_all_nodes()
        {
            var w1 = new XenWidget {Name = "1"};
            var w2 = new XenWidget {Name = "2"};
            var w3 = new XenWidget {Name = "3"};
            var w4 = new XenWidget {Name = "4"};
            var w5 = new XenWidget {Name = "5"};

            w1.Children = new List<XenWidget>
            {
                w2,
                w3
            };

            w3.Children = new List<XenWidget>
            {
                w4
            };

            w4.Children = new List<XenWidget> {w5};
            var nodes = w1.GetNodeAndDescendants();

            CollectionAssert.AreEquivalent(nodes, new[] {w1,w2,w3,w4,w5});
        }
    }
}
