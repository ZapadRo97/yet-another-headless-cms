using Xunit;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using YAHCMS.BlogService.Controllers;
using YAHCMS.BlogService.Persistence;
using YAHCMS.BlogService.Models;
using Microsoft.AspNetCore.Http;
using System;

namespace YAHCMS.BlogService.Tests
{
    public class PostsControllerTest
    {

        MemoryBlogRepository repository;
        PostsController controller;
        Blog blog;

        public PostsControllerTest()
        {
            repository = new MemoryBlogRepository();
            controller = new PostsController(repository);
            blog = new Blog(1, "name", "desc");
            repository.Add(blog);
        }
        //BlogsController blogsController = new BlogsController(new MemoryBlogRepository());

        [Fact]
        public void GetSingularPostReturns()
        {
            Post post = new Post("post", "content");
            post = (((controller.Create(blog.ID, post)) as ObjectResult).Value as Post);

            Assert.NotEqual(post.ID, 0);

            post = controller.GetPost(blog.ID, post.ID).Value;

            Assert.Equal(post.Title, "post");

        }

        [Theory]
        [InlineData(1, StatusCodes.Status200OK)]
        [InlineData(999, StatusCodes.Status404NotFound)]
        public void GetPostsFromBlog(long blogID, int expectedStatus)
        {
            
            repository.GetBlog(1).Posts.Clear();

            Post p1 = new Post("pos1", "content");
            Post p2 = new Post("post2", "content");

            controller.Create(1, p1);
            controller.Create(1, p2);

            var result = controller.GetPosts(blogID);

            switch(expectedStatus) {
                case StatusCodes.Status404NotFound:
                {
                    Assert.Equal((result.Result as StatusCodeResult).StatusCode, expectedStatus);
                    break;
                }
                case StatusCodes.Status200OK:
                {
                    Assert.Equal((result.Value as List<Post>).Count, 2);
                    break;
                }
            }

        }

        [Fact]
        public void AddPostToNonExistentBlog()
        {
            Post p = new Post("post1", "content");
            var result = controller.Create(777, p);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void AddPostToBlogUpdateList()
        {
            Post p = new Post("post1", "content");
            var result = controller.Create(blog.ID, p);
            var post = (result as ObjectResult).Value as Post;

            Assert.NotEqual(post.ID, 0);
            //why?
            //Assert.NotNull(post.Blog);

            blog = repository.GetBlog(blog.ID);
            Assert.NotEmpty(blog.Posts);

        }

        [Fact]
        public void DeletePostRemoveFromList()
        {
            Post samplePost = new Post("sample", "content");
            controller.Create(blog.ID, samplePost);

            var posts = controller.GetPosts(blog.ID).Value;
            samplePost = posts.FirstOrDefault(target => target.Title == samplePost.Title);
            Assert.NotNull(samplePost);

            controller.Delete(blog.ID, samplePost.ID);

            samplePost = controller.GetPosts(blog.ID).Value
                .FirstOrDefault(target => target.Title == samplePost.Title);
            Assert.Null(samplePost);

        }

        [Fact]
        public void DeleteNotExistentPost()
        {
            var result = controller.Delete(blog.ID, 999);
            Assert.True(result is NotFoundResult);


        }

        [Fact]
        public void UpdatePostIsModified()
        {
            Post post = new Post("post1", "content");
            var result = controller.Create(blog.ID, post);
            post = ((result as ObjectResult).Value as Post);

            post.Title = "post2";
            controller.Update(post, blog.ID, post.ID);

            var posts = controller.GetPosts(blog.ID).Value;
            var oldPost = posts.FirstOrDefault(t => t.Title == "post1");
            Assert.Null(oldPost);

            post = controller.GetPost(blog.ID, post.ID).Value;
            Assert.Equal(post.Title, "post2");


        }

        [Fact]
        public void UpdateNonExistentPost()
        {
            Post post = new Post("post1", "content");
            post.ID = 999;

            var result = controller.Update(post, blog.ID, post.ID);
            Assert.True(result is NotFoundResult);
            result = controller.Update(post, 999, post.ID);
            Assert.True(result is NotFoundResult);
        }

        
    }
}