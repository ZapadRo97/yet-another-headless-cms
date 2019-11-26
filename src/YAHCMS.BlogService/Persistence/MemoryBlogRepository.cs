
using System.Collections.Generic;
using YAHCMS.BlogService.Models;
using System.Linq;

namespace YAHCMS.BlogService.Persistence
{
    public class MemoryBlogRepository : IBlogRepository
    {
        protected static ICollection<Blog> blogs;

        public MemoryBlogRepository()
        {
            if(blogs == null)
                blogs = new List<Blog>();
        }

        public MemoryBlogRepository(ICollection<Blog> bl)
        {
            blogs = bl;
        }

        public Blog Add(Blog blog)
        {
            if(blog.ID == 0)
                blog.ID = blogs.Count + 1;
            blogs.Add(blog);

            return blog;
        }

        public Post AddPost(long blogID, Post post)
        {
            var blog = GetBlog(blogID);
            if(blog == null)
                return null;

            post.ID = blog.Posts.Count() + 1;
            post.Blog = blog;
            blog.Posts.Add(post);

            return post;
        }

        public IEnumerable<Post> GetBlogPosts(long blogID)
        {
            var blog = GetBlog(blogID);
            if(blog == null)
                return null;

            return blog.Posts;
        }

        public Post GetBlogPost(long blogID, long postID)
        {
            var blog = GetBlog(blogID);
            if(blog == null)
                return null;

            return blog.Posts.FirstOrDefault(t => t.ID == postID);
        }

        public Blog Delete(long blogID)
        {
            var q = blogs.Where(t => t.ID == blogID);
            Blog blog = null;

            if(q.Count() > 0) {
                blog = q.First();
                blogs.Remove(blog);
            }

            return blog;
        }

        public Post RemovePost(long blogID, long postID)
        {
            var q = blogs.Where(t => t.ID == blogID);
            Blog blog = null;

            if(q.Count() == 0) {
                return null;
            }
            blog = q.First();

            Post post = null;
            var q2 = blog.Posts.Where(t => t.ID == postID);

            if(q2.Count() > 0) {
                post = q2.First();
                blog.Posts.Remove(post);
            }

            return post;

        }

        public Post UpdatePost(Post post, long blogID)
        {
            Post p = RemovePost(blogID, post.ID);
            if(p != null)
                GetBlog(blogID).Posts.Add(post);

            return p;
        }

        public IEnumerable<Blog> GetUserBlogs(long userID)
        {
            
            return (from b in blogs where b.UserID == userID select b).ToArray();
        }

        public Blog GetBlog(long blogID)
        {
            return blogs.ToList().FirstOrDefault(b => b.ID == blogID);
        }

        public Blog Update(Blog b)
        {
            Blog blog = Delete(b.ID);
            if(blog != null)
                blog = Add(b);

            return blog;

        }
    }
}