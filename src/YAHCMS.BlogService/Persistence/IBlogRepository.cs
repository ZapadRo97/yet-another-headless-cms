using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YAHCMS.BlogService.Models;

namespace YAHCMS.BlogService.Persistence
{
    public interface IBlogRepository
    {
        IEnumerable<Blog> GetUserBlogs(string userID);
        Blog GetBlog(long blogID);
        List<long> GetAllIds();
        Blog Add(Blog blog);
        Blog Delete(long blogID);
        Blog Update(Blog blog);

        Post AddPost(long blogID, Post post);
        IEnumerable<Post> GetBlogPosts(long blogID);
        Post GetBlogPost(long blogID, long postID);
        Post RemovePost(long blogID, long postID);
        Post UpdatePost(Post post, long blogID);
        

    }
}
