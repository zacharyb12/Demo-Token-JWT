using DAL.Repository.PostRepositories;
using Models.Post_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.PostServices
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _repository;

        public PostService(IPostRepository repository)
        {
            _repository = repository;
        }

        public async Task<PostRead> CreatePostAsync(PostCreate post)
        {
            Post postAdd = new();

            postAdd.Title = post.Title;
            postAdd.Content = post.Content;
            postAdd.UserId = post.UserId;
            
            var created = await _repository.CreatePostAsync(postAdd);
            return ToPostRead(created);
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            return await _repository.DeletePostAsync(postId);
        }

        public async Task<IEnumerable<PostRead>> GetAllPostsAsync()
        {
            var posts = await _repository.GetAllPostsAsync();
            return posts.Select(ToPostRead);
        }

        public async Task<PostRead?> GetPostByIdAsync(Guid postId)
        {
            var post = await _repository.GetPostByIdAsync(postId);
            return post == null ? null : ToPostRead(post);
        }

        public async Task<IEnumerable<PostRead>> GetPostsByUserIdAsync(Guid userId)
        {
            var posts = await _repository.GetPostsByUserIdAsync(userId);
            return posts.Select(ToPostRead);
        }

        public async Task<PostRead> UpdatePostAsync(PostUpdate post)
        {
            var updatedPost = await _repository.GetPostByIdAsync(post.Id);

            updatedPost.Title = post.Title;
            updatedPost.Content = post.Content;

            var updated = await _repository.UpdatePostAsync(updatedPost);
            return ToPostRead(updated);
        }

        private PostRead ToPostRead(Post post)
        {
            return new PostRead
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UserId = post.UserId
            };
        }
    }
}
