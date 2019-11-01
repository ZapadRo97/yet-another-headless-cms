using Xunit;
using System;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;

namespace YAHCMS.BlogService.Tests.Integration
{
    public class SimpleIntegrationTest
    {
        private readonly TestServer testServer;
        private readonly HttpClient testClient;

        public SimpleIntegrationTest()
        {
            testServer = new TestServer(new  WebHostBuilder().UseStartup<Startup>());
            testClient = testServer.CreateClient();
        }

        [Fact]
        public void SomeTest()
        {

        }

        
    }
}