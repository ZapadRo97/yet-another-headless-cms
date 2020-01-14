using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using YAHCMS.BlogService.Controllers;
using YAHCMS.BlogService.Models;
using YAHCMS.BlogService.Persistence;

namespace YAHCMS.BlogService.Tests
{
    public class BlogsControllerTest
    {
        BlogsController controller;

        public BlogsControllerTest()
        {
            controller = new BlogsController(new MemoryBlogRepository());
        }

        [Fact]
        public void CompleteBlogDetails() {
            //todo
            
        }

        [Fact]
        public void CreateBlog() {
            //first parameter is userID
            Blog b = new Blog("tt", "name", "desc");
            var result = controller.Create(b);
            var blogs = controller.GetUserBlogs("tt").Value;
        

            Assert.Equal((result as ObjectResult).StatusCode, 201);
            Assert.Equal(((result as ObjectResult).Value as Blog).ID, blogs.Count());
        }

        [Fact]
        public void CreateBlogsAddToList() {
            var userID = "aa";

            Blog b1 = new Blog(userID, "the name", "the description");
            Blog b2 = new Blog(userID, "name", "desc", "en", "France");
            
            controller.Create(b1);
            controller.Create(b2);

            var result = controller.GetUserBlogs(userID);

            Assert.Equal((result.Value as List<Blog>).Count, 2);
        }

        [Fact]
        public void DeleteBlogRemovesFromList()
        {
            var userID = "dd";
            Blog sampleBlog = new Blog(userID, "blog1", "description");
            controller.Create(sampleBlog);

            var blogs = controller.GetUserBlogs(userID).Value;
            sampleBlog = blogs.FirstOrDefault(target => target.Name == sampleBlog.Name);

            Assert.NotNull(sampleBlog);

            controller.Delete(sampleBlog.ID);

            sampleBlog = controller.GetUserBlogs(userID).Value
                .FirstOrDefault(target => target.Name == sampleBlog.Name);
            Assert.Null(sampleBlog);
        }

        [Fact]
        public void DeleteNonExistentReturnsNotFound()
        {
            var result = controller.Delete(999);

            Assert.True(result is NotFoundResult);

        }

        [Fact]
        public void UpdateBlogModifiesBlogsList()
        {
            var userID = "dd";
            Blog sampleBlog = new Blog(userID, "blog1", "description");
            var result = controller.Create(sampleBlog);

            sampleBlog = ((result as ObjectResult).Value as Blog);
            sampleBlog.Name = "blog2";

            controller.Update(sampleBlog, sampleBlog.ID);

            var blogs = controller.GetUserBlogs(userID).Value;
            var oldBlog = blogs.FirstOrDefault(b => b.Name == "blog1");
            Assert.Null(oldBlog);

            sampleBlog = controller.GetBlog(sampleBlog.ID).Value;
            Assert.Equal(sampleBlog.Name, "blog2");
        }

        [Fact]
        public void UpdateNotExistentBlogReturnsNotFound()
        {
            Blog blog = new Blog("tt", "name", "desc");
            var result = controller.Update(blog, 999);
            Assert.True(result is NotFoundResult);
        }
    }
}