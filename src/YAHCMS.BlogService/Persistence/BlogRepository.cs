

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using YAHCMS.BlogService.Models;

namespace YAHCMS.BlogService.Persistence
{
    public class BlogRepository : IBlogRepository
    {

        private BlogDbContext context;

        public BlogRepository(BlogDbContext context)
        {
            this.context = context;
        }

        public Blog Add(Blog blog)
        {
            var b = context.Add(blog);
            context.SaveChanges();
            return b.Entity;
        }

        public Post AddPost(long blogID, Post post)
        {
            Blog b = context.blogs.FirstOrDefault(b => b.ID == blogID);
            if(b == null)
                return null;

            post.Blog = b;
            var p = context.Add(post);
            context.SaveChanges();
            return p.Entity;
        }

        public Blog Delete(long blogID)
        {
            Blog b = this.GetBlog(blogID);
            context.Remove(b);
            return b;
        }

        public Blog GetBlog(long blogID)
        {
            return context.blogs.FirstOrDefault(b => b.ID == blogID);
        }

        public Post GetBlogPost(long blogID, long postID)
        {
            return GetBlogPosts(blogID).FirstOrDefault(p => p.ID == postID);
        }

        public IEnumerable<Post> GetBlogPosts(long blogID)
        {
            return context.blogs.FirstOrDefault(b => b.ID == blogID).Posts;
        }

        public IEnumerable<Blog> GetUserBlogs(string userID)
        {
            return context.blogs.Where(b => b.UserID == userID).Select(b => b);
        }

        public Post RemovePost(long blogID, long postID)
        {
            Post p = this.GetBlogPost(blogID, postID);
            context.Remove(p);
            return p;
        }

        public Blog Update(Blog blog)
        {
            context.Entry(blog).State = EntityState.Modified;
            context.SaveChanges();
            return blog;
        }

        public Post UpdatePost(Post post, long blogID)
        {
            Blog b = GetBlog(blogID);
            if(b == null)
                return null;

            context.Entry(post).State = EntityState.Modified;
            context.SaveChanges();
            return post;
        }
    }
}