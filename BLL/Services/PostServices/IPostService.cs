using Models.Post_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.PostServices
{
    public interface IPostService
    {
        Task<PostRead?> GetPostByIdAsync(Guid postId);
        Task<IEnumerable<PostRead>> GetAllPostsAsync();
        Task<IEnumerable<PostRead>> GetPostsByUserIdAsync(Guid userId);
        Task<PostRead> CreatePostAsync(PostCreate post);
        Task<PostRead> UpdatePostAsync(PostUpdate post);
        Task<bool> DeletePostAsync(Guid postId);
    }
}
