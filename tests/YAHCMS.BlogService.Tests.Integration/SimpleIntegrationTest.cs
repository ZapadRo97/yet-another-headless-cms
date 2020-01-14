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
        private readonly Post somePost;

        public SimpleIntegrationTest()
        {
            testServer = new TestServer(new  WebHostBuilder().UseStartup<Startup>());
            testClient = testServer.CreateClient();

            someBlog = new Blog("tt", "name", "desc");
            somePost = new Post("title", "content");
        }

        public async Task<Blog> AddBlog()
        {
            StringContent stringContent = new StringContent(
                JsonConvert.SerializeObject(someBlog), 
                UnicodeEncoding.UTF8, "application/json");

            var postResponse = 
                await testClient.PostAsync("/api/blogs", stringContent);
            postResponse.EnsureSuccessStatusCode();
            string raw = await postResponse.Content.ReadAsStringAsync();

            Blog blog = JsonConvert.DeserializeObject<Blog>(raw);
            return blog;
        }

        public async Task<Post> AddPost(Blog blog) {

            StringContent stringContent = new StringContent(
                JsonConvert.SerializeObject(somePost), 
                UnicodeEncoding.UTF8, "application/json");

            var postResponse = 
                await testClient.PostAsync($"/api/blogs/{blog.ID}/posts", stringContent);
            postResponse.EnsureSuccessStatusCode();
            string raw = await postResponse.Content.ReadAsStringAsync();

            Post p = JsonConvert.DeserializeObject<Post>(raw);
            return p;

        }

        [Fact]
        public async void BlogPostAndGet()
        {
            await AddBlog();

            var getResponse = await testClient.GetAsync("/api/blogs/user/tt");
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
            
            var getResponse = await testClient.GetAsync("/api/blogs/user/tt");
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
            raw = await postResponse.Content.ReadAsStringAsync();
            Post pp = JsonConvert.DeserializeObject<Post>(raw);

            getResponse = await testClient.GetAsync($"api/blogs/{blog.ID}/posts/{pp.ID}");
            raw = await getResponse.Content.ReadAsStringAsync();
            post = JsonConvert.DeserializeObject<Post>(raw);
            Assert.Equal(post.Title, "title");
            Assert.Equal(post.Content, "content");
        }

        [Theory]
        [InlineData("/api/blogs/9999")]
        [InlineData("/api/blogs/users/aaaa")]
        [InlineData("api/blogs/1/posts/9999")]
        [InlineData("api/blogs/9999/posts")]
        [InlineData("api/blogs/9999/posts/9999")]
        public async void GetNotExistentBlog(string route)
        {
            var getResponse = await testClient.GetAsync(route);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }


        [Fact]
        public async void UpdateBlogCheckPosts()
        {
            Blog b = await AddBlog();
            Post p = await AddPost(b);

            b.Name = "NEW";
            StringContent stringContent = new StringContent(
                JsonConvert.SerializeObject(b), 
                UnicodeEncoding.UTF8, "application/json");

            var putResponse = await testClient.PutAsync($"api/blogs/{b.ID}", stringContent);
            putResponse.EnsureSuccessStatusCode();

            var getResponse = await testClient.GetAsync($"api/blogs/{b.ID}");
            var raw = await getResponse.Content.ReadAsStringAsync();

            Blog blog = JsonConvert.DeserializeObject<Blog>(raw);
            Assert.Equal(blog.Name, "NEW");

        }

        [Fact]
        public async void UpdatePostCheckBlog()
        {
            Blog b = await AddBlog();
            Post p = await AddPost(b);

            p.Title = "NEW";
            StringContent stringContent = new StringContent(
                JsonConvert.SerializeObject(p), 
                UnicodeEncoding.UTF8, "application/json");

            var putResponse = await testClient.PutAsync($"api/blogs/{b.ID}/posts/{p.ID}", stringContent);
            putResponse.EnsureSuccessStatusCode();

            var getResponse = await testClient.GetAsync($"api/blogs/{b.ID}");
            var raw = await getResponse.Content.ReadAsStringAsync();

            Blog blog = JsonConvert.DeserializeObject<Blog>(raw);
            Assert.NotEmpty(blog.Posts);

            Post pp = blog.Posts.FirstOrDefault(pp => pp.ID == p.ID);
            Assert.Equal(pp.Title, "NEW");

        }
        
        [Fact]
        public async void DeletePostCheck()
        {
            Blog b = await AddBlog();
            Post p1 = await AddPost(b); 
            Post p2 = await AddPost(b);

            var getResponse = await testClient.GetAsync($"api/blogs/{b.ID}/posts");
            getResponse.EnsureSuccessStatusCode();

            string raw = await getResponse.Content.ReadAsStringAsync();
            List<Post> postsBefore = JsonConvert.DeserializeObject<List<Post>>(raw);

            var deleteResponse = await testClient.DeleteAsync($"api/blogs/{b.ID}/posts/{p1.ID}");
            deleteResponse.EnsureSuccessStatusCode();

            getResponse = await testClient.GetAsync($"api/blogs/{b.ID}/posts");
            getResponse.EnsureSuccessStatusCode();

            raw = await getResponse.Content.ReadAsStringAsync();
            List<Post> postsAfter = JsonConvert.DeserializeObject<List<Post>>(raw);

            Assert.NotEqual(postsBefore.Count, postsAfter.Count);

        }

        [Fact]
        public async void DeleteBlogCheck()
        {
            Blog b = await AddBlog();

            var getResponse = await testClient.GetAsync("/api/blogs/user/tt");
                getResponse.EnsureSuccessStatusCode();

            string raw = await getResponse.Content.ReadAsStringAsync();

            List<Blog> blogsBefore =
                JsonConvert.DeserializeObject<List<Blog>>(raw);

            var deleteResponse = await testClient.DeleteAsync($"api/blogs/{b.ID}");
            deleteResponse.EnsureSuccessStatusCode();

            getResponse = await testClient.GetAsync("/api/blogs/user/tt");
                getResponse.EnsureSuccessStatusCode();

            raw = await getResponse.Content.ReadAsStringAsync();

            List<Blog> blogsAfter =
                JsonConvert.DeserializeObject<List<Blog>>(raw);

            Assert.Equal(blogsBefore.Count, blogsAfter.Count + 1);
        }
        
    }
}