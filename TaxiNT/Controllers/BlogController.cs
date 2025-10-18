using Microsoft.AspNetCore.Mvc;
using TaxiNT.Libraries.Models;
using TaxiNT.Services.Interfaces;

namespace TaxiNT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogServer _blogService;

        public BlogController(IBlogServer blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var blogs = await _blogService.GetAllAsync();
            return Ok(blogs);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var blog = await _blogService.GetByIdAsync(id);
            if (blog == null) return NotFound();
            return Ok(blog);
        }

        [HttpGet("get/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var blog = await _blogService.GetBySlugAsync(slug);
            if (blog == null) return NotFound();
            await _blogService.IncrementViewCountAsync(blog.Id);
            return Ok(blog);
        }

        [HttpGet("related/{category}/{excludeId}")]
        public async Task<IActionResult> GetRelated(string category, int excludeId)
        {
            var blogs = await _blogService.GetRelatedAsync(category, excludeId);
            return Ok(blogs);
        }

        [HttpGet("popular")]
        public async Task<IActionResult> GetMostPopular()
        {
            var blogs = await _blogService.GetMostPopularAsync();
            return Ok(blogs);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Blog blog)
        {
            var newBlog = await _blogService.CreateAsync(blog);
            return CreatedAtAction(nameof(GetById), new { id = newBlog.Id }, newBlog);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Blog blog)
        {
            if (id != blog.Id) return BadRequest();
            var success = await _blogService.UpdateAsync(blog);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _blogService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
