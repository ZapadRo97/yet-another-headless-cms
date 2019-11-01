using Xunit;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using YAHCMS.BlogService.Models;
using Newtonsoft.Json;
using System.Text;
using System.Linq;

namespace YAHCMS.BlogService.Tests.Integration
{
    public class SimpleIntegrationTest
    {
        private readonly TestServer testServer;
        private readonly HttpClient testClient;

        private readonly Blog someBlog;

        public SimpleIntegrationTest()
        {
            testServer = new TestServer(new  WebHostBuilder().UseStartup<Startup>());
            testClient = testServer.CreateClient();

            someBlog = new Blog(1, "name", "desc");
        }

        [Fact]
        public async void BlogPostAndGet()
        {
            StringContent stringContent = new StringContent(
                JsonConvert.SerializeObject(someBlog), 
                UnicodeEncoding.UTF8, "application/json");

            var postResponse = 
                await testClient.PostAsync("/api/blogs", stringContent);
            postResponse.EnsureSuccessStatusCode();

            var getResponse = await testClient.GetAsync("/api/blogs/user/1");
                getResponse.EnsureSuccessStatusCode();

            string raw = await getResponse.Content.ReadAsStringAsync();

            List<Blog> blogs =
                JsonConvert.DeserializeObject<List<Blog>>(raw);

            Assert.Equal(1, blogs.Count());
            Assert.NotEqual(0, blogs[0].ID);
            Assert.Equal("name", blogs[0].Name);
        
        }

        
    }
}