using MTCG.Server.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCGTests.ConversionTests
{
    [TestFixture]
    public class HttpParserTests
    {
        private HttpParser _parser;

        [SetUp]
        public void Setup()
        {
            _parser = new HttpParser();
        }

        [Test]
        public void Parse_ValidHttpRequest_Success()
        {
            
            string httpRequest =
                "POST /users HTTP/1.1\r\n" +
                "Content-Type: application/json\r\n" +
                "Authorization: Bearer kienboec-mtcgToken\r\n" +
                "Content-Length: 29\r\n" +
                "\r\n" +
                "{ \"Username\": \"kienboec\" }";

            using var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(httpRequest)));

            
            var result = _parser.Parse(reader);

            
            Assert.IsNotNull(result);
            Assert.AreEqual("POST", result.Method);
            Assert.AreEqual("/users", result.Path);
            Assert.AreEqual("HTTP/1.1", result.Version);
            Assert.AreEqual(3, result.Headers.Count);
            Assert.AreEqual("application/json", result.Headers["Content-Type"]);
            Assert.AreEqual("kienboec-mtcgToken", result.Headers["Authorization"]);
            Assert.AreEqual("{ \"Username\": \"kienboec\" }", result.Body);
        }

        [Test]
        public void Parse_MissingAuthorizationHeader_Fail()
        {
            
            string httpRequest =
                "POST /users HTTP/1.1\r\n" +
                "Content-Type: application/json\r\n" +
                "Content-Length: 29\r\n" +
                "\r\n" +
                "{ \"username\": \"kienboec\" }";

            using var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(httpRequest)));

            
            var result = _parser.Parse(reader);

            
            Assert.IsNotNull(result);
            Assert.AreEqual("POST", result.Method);
            Assert.AreEqual("/users", result.Path);
            Assert.AreEqual("HTTP/1.1", result.Version);
            Assert.AreEqual(2, result.Headers.Count);
            Assert.IsFalse(result.Headers.ContainsKey("Authorization"));
        }

        [Test]
        public void Parse_InvalidJsonBody_EmptyBody()
        {
            
            string httpRequest =
                "POST /users HTTP/1.1\r\n" +
                "Content-Type: application/json\r\n" +
                "Authorization: Bearer kienboec-mtcgToken\r\n" +
                "Content-Length: 0\r\n" +
                "\r\n";

            using var reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(httpRequest)));

            
            var result = _parser.Parse(reader);

            
            Assert.IsNotNull(result);
            Assert.AreEqual("POST", result.Method);
            Assert.AreEqual("/users", result.Path);
            Assert.AreEqual("HTTP/1.1", result.Version);
            Assert.AreEqual(3, result.Headers.Count);
            Assert.AreEqual("kienboec-mtcgToken", result.Headers["Authorization"]);
            Assert.IsEmpty(result.Body);
        }
    }
}
