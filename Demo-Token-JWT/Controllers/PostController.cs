using BLL.Services.PostServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Post_Models;

namespace Demo_Token_JWT.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]  // <-- ici, obligatoire d'avoir un token JWT valide
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<PostRead>>> GetAllPosts()
        {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<PostRead>> GetPostById(Guid id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
                return NotFound();
            return Ok(post);
        }


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PostRead>>> GetPostsByUserId(Guid userId)
        {
            var posts = await _postService.GetPostsByUserIdAsync(userId);
            return Ok(posts);
        }


        [HttpPost]
        public async Task<ActionResult<PostRead>> CreatePost([FromBody] PostCreate post)
        {
            var createdPost = await _postService.CreatePostAsync(post);
            return CreatedAtAction(nameof(GetPostById), new { id = createdPost.Id }, createdPost);
        }


        [HttpPut]
        public async Task<ActionResult<PostRead>> UpdatePost([FromBody] PostUpdate post)
        {
            var updatedPost = await _postService.UpdatePostAsync(post);
            return Ok(updatedPost);
        }


        [HttpDelete("{id}")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var result = await _postService.DeletePostAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
