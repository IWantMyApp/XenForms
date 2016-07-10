using NUnit.Framework;
using XenForms.Core.Networking;

namespace XenForms.Designer.Tests.Networking
{
    [TestFixture]
    public class TestServiceEndpoint
    {
        [Test]
        public void Format_hostname_with_no_path()
        {
            const string hostname = "localhost";
            const int port = 8888;

            // Act
            var url = ServiceEndpoint.FormatAddress(hostname, port);

            Assert.AreEqual(url, $"ws://{hostname}:{port}/");
        }


        [Test]
        public void Format_hostname_with_path_added()
        {
            const string hostname = "SERVER123";
            const int port = 8282;
            const string path = "/test";

            // Act
            var url = ServiceEndpoint.FormatAddress(hostname, port, path);

            Assert.AreEqual(url, $"ws://{hostname}:{port}{path}");
        }


        [Test]
        public void Invalid_parameters()
        {
            var noHost = ServiceEndpoint.FormatAddress("   ", 9999);
            Assert.IsNull(noHost, "No hostname was provided.");

            // Act
            var badPort = ServiceEndpoint.FormatAddress("testing", 80);

            Assert.IsNull(badPort, "This port should be reserved.");
        }
    }
}
