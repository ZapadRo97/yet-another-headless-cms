using Xunit;
using System;
using System.Net;
using System.Collections.Generic;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using YAHCMS.BlogService.Models;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task AddBlog()
        {
            StringContent stringContent = new StringContent(
                JsonConvert.SerializeObject(someBlog), 
                UnicodeEncoding.UTF8, "application/json");

            var postResponse = 
                await testClient.PostAsync("/api/blogs", stringContent);
            postResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void BlogPostAndGet()
        {
            await AddBlog();

            var getResponse = await testClient.GetAsync("/api/blogs/user/1");
                getResponse.EnsureSuccessStatusCode();

            string raw = await getResponse.Content.ReadAsStringAsync();

            List<Blog> blogs =
                JsonConvert.DeserializeObject<List<Blog>>(raw);

            Assert.NotEqual(0, blogs.Count());
            Assert.NotEqual(0, blogs[0].ID);
            Assert.Equal("name", blogs[0].Name);
        
        }

        [Fact]
        public async void AddPostToBlog()
        {
            
            var getResponse = await testClient.GetAsync("/api/blogs/user/1");
                getResponse.EnsureSuccessStatusCode();

            string raw = await getResponse.Content.ReadAsStringAsync();

            List<Blog> blogs =
                JsonConvert.DeserializeObject<List<Blog>>(raw);

            if(blogs.Count == 0)
                await AddBlog();

            getResponse = await testClient.GetAsync("api/blogs/1");
            raw = await getResponse.Content.ReadAsStringAsync();

            Blog blog = JsonConvert.DeserializeObject<Blog>(raw);

            Post post = new Post("title", "content");
            //post.Blog = blog;

            StringContent stringContent = new StringContent(
                JsonConvert.SerializeObject(post), 
                UnicodeEncoding.UTF8, "application/json");

            var postResponse = 
                await testClient.PostAsync($"/api/blogs/{blog.ID}/posts", stringContent);
            postResponse.EnsureSuccessStatusCode();

            getResponse = await testClient.GetAsync($"api/blogs/{blog.ID}/posts/1");
            raw = await getResponse.Content.ReadAsStringAsync();
            post = JsonConvert.DeserializeObject<Post>(raw);
            Assert.Equal(post.Title, "title");
            Assert.Equal(post.Content, "content");
        }

        [Theory]
        [InlineData("/api/blogs/9999")]
        [InlineData("/api/blogs/users/9999")]
        [InlineData("api/blogs/1/posts/9999")]
        [InlineData("api/blogs/9999/posts")]
        [InlineData("api/blogs/9999/posts/9999")]
        public async void GetNotExistentBlog(string route)
        {
            var getResponse = await testClient.GetAsync(route);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async void GetPostsFromNotExistentBlog()
        {
        
        }
        
    }
}