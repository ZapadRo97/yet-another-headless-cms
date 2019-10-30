using Microsoft.AspNetCore.Mvc;
using YAHCMS.BlogService.Persistence;
using YAHCMS.BlogService.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace YAHCMS.BlogService.Controllers
{
    [ApiController]
    [Route("api/blogs/{blogID}/[controller]")]
    public class PostsController : ControllerBase
    {
        IBlogRepository repository;
        public PostsController(IBlogRepository repo)
        {
            repository = repo;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create(long blogID, [FromBody] Post post)
        {
            Post p = repository.AddPost(blogID, post);
            if(p == null)
                return BadRequest();
            
            return Created($"{p.ID}", post);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Post>> GetPosts(long blogID)
        {
            var posts = repository.GetBlogPosts(blogID);
            if(posts == null)
                return NotFound();

            return posts.ToList();
        }

        [HttpGet("{postID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Post> GetPost(long blogID, long postID)
        {
            Post p = repository.GetBlogPost(blogID, postID);
            if(p == null)
                return NotFound();

            return p;
        }

        [HttpDelete("{postID}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(long blogID, long postID)
        {
            Post p = repository.RemovePost(blogID, postID);
            if(p == null)
                return NotFound();

            return NoContent();

        }

        [HttpPut("{postID}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update([FromBody] Post post, long blogID, long postID)
        {
            post.ID = postID;
            if(repository.UpdatePost(post, blogID) == null)
                return NotFound();

            return NoContent();
        }




    }
}