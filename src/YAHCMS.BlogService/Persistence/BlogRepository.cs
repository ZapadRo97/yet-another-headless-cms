

using System.Collections.Generic;
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
            throw new System.NotImplementedException();
        }

        public Post AddPost(long blogID, Post post)
        {
            throw new System.NotImplementedException();
        }

        public Blog Delete(long blogID)
        {
            throw new System.NotImplementedException();
        }

        public Blog GetBlog(long blogID)
        {
            throw new System.NotImplementedException();
        }

        public Post GetBlogPost(long blogID, long postID)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Post> GetBlogPosts(long blogID)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Blog> GetUserBlogs(long userID)
        {
            throw new System.NotImplementedException();
        }

        public Post RemovePost(long blogID, long postID)
        {
            throw new System.NotImplementedException();
        }

        public Blog Update(Blog blog)
        {
            throw new System.NotImplementedException();
        }

        public Post UpdatePost(Post post, long blogID)
        {
            throw new System.NotImplementedException();
        }
    }
}