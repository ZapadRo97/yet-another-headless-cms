using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YAHCMS.BlogService.Models;
using YAHCMS.BlogService.Persistence;
using YAHCMS.BlogService.Extensions;

namespace YAHCMS.BlogService.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController : ControllerBase 
    {

        IBlogRepository repository;
        public BlogsController(IBlogRepository repo)
        {
            repository = repo;
        }

        [HttpGet("user/{userID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Blog>> GetUserBlogs(string userID)
        {
            //todo: return not found etc
            return repository.GetUserBlogs(userID).ToList();
        }

        [HttpGet("random/{limit}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<long>> GetRandomBlogIds(int limit)
        {
            var ids = repository.GetAllIds();
            ids.Shuffle();

            return ids.Take(limit).ToList();
        }

        [HttpGet("{blogID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Blog> GetBlog(long blogID)
        {
            Blog b = repository.GetBlog(blogID);
            if(b == null)
                return NotFound();

            return b;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create(Blog blog)
        {
            //await repository.AddBlog(blog);
            Blog b = repository.Add(blog);

            return Created($"{b.ID}", blog);
        }

        [HttpDelete("{blogID}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(long blogID)
        {
            Blog b = repository.Delete(blogID);
            if(b == null)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{blogID}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update([FromBody] Blog blog, long blogID)
        {
            blog.ID = blogID;
            if(repository.Update(blog) == null)
            {
                return NotFound();
            }

                return NoContent();
        }




    }
}