using DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Post_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.PostRepositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<PostRepository> _logger;

        public PostRepository(DataContext context, ILogger<PostRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Get Post By Id
        public async Task<Post?> GetPostByIdAsync(Guid postId)
        {
            if (postId.Equals(Guid.Empty))
            {
                _logger.LogWarning($"postId est null : {postId} : DAL - GetById");
                return null;
            }

            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

                if (post == null)
                {
                    _logger.LogInformation($"Post introuvable avec l'id : {postId} : DAL - GetById");
                }

                return post;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la recuperation du post ID: {postId} : DAL - GetById");
                throw new Exception($"Erreur lors de la recuperation du post {postId} : DAL - GetById", ex);
            }
        }
        #endregion


        #region GetAll Posts
        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            try
            {
                return await _context.Posts.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation des posts : DAL - GetAll");
                throw new Exception("Erreur lors de la recuperation des posts", ex);
            }
        }
        #endregion

        #region Get Posts By User Id
        public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(Guid userId)
        {
            if (userId.Equals(Guid.Empty))
            {
                _logger.LogWarning($"userId est null : {userId} : DAL - GetByUserId");
                return Enumerable.Empty<Post>();
            }

            try
            {
                return await _context.Posts.Where(p => p.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la recuperation des posts pour l'utilisateur ID: {userId} : DAL - GetByUserId");
                throw new Exception($"Erreur lors de la recuperation des posts pour l'utilisateur {userId} : DAL - GetByUserId", ex);
            }
        }
        #endregion

        #region Create Post
        public async Task<Post> CreatePostAsync(Post post)
        {
            if (post == null)
            {
                _logger.LogWarning("Post est null : DAL - Create");
                throw new ArgumentNullException(nameof(post));
            }

            if (post.UserId.Equals(Guid.Empty))
            {
                _logger.LogWarning("Post UserId est un GuidEmpty : DAL - Create");
                throw new Exception("Post UserId doit etre dans un format correct");
            }

            try
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == post.UserId);

                if (!userExists)
                {
                    _logger.LogWarning($"Utilisateur introuvable pour créer le post: {post.UserId} : DAL - Create");
                    throw new Exception($"Utilisateur avec l'ID {post.UserId} introuvable");
                }

                post.Id = Guid.NewGuid();
                post.CreatedAt = default;

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Post créé avec succès: {post.Id} : DAL - Create");
                return post;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la creation du post pour l'utilisateur: {post.UserId} : DAL - Create");
                throw new Exception($"Erreur lors de la creation du post pour l'utilisateur: {post.UserId} : DAL - Create", ex);
            }
        }
        #endregion

        #region Update Post
        public async Task<Post> UpdatePostAsync(Post post)
        {
            if (post == null)
            {
                _logger.LogWarning("Post est null : DAL - Update");
                throw new ArgumentNullException(nameof(post));
            }

            if (post.Id.Equals(Guid.Empty))
            {
                _logger.LogWarning("Post Id est un GuidEmpty : DAL - Update");
                throw new Exception("Post Id doit etre dans un format correct");
            }

            try
            {
                var existingPost = await _context.Posts.FindAsync(post.Id);

                if (existingPost == null)
                {
                    _logger.LogWarning($"Aucun post avec un id: {post.Id} : DAL - Update");
                    throw new Exception($"Aucun post avec un id {post.Id}");
                }

                existingPost.Title = post.Title;
                existingPost.Content = post.Content;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Post mis a jour: {post.Id} : DAL - Update");

                return existingPost;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la mise a jour du post: {post.Id} : DAL - Update");
                throw new Exception($"Erreur lors de la mise a jour du post: {post.Id} : DAL - Update", ex);
            }
        }
        #endregion

        #region Delete Post
        public async Task<bool> DeletePostAsync(Guid postId)
        {
            if (postId.Equals(Guid.Empty))
            {
                _logger.LogWarning($"PostId invalide: {postId} : DAL - Delete");
                return false;
            }

            try
            {
                var rowsAffected = await _context.Posts.Where(p => p.Id.Equals(postId)).ExecuteDeleteAsync();

                if (rowsAffected == 0)
                {
                    _logger.LogInformation($"Post introuvable pour suppression: {postId} : DAL - Delete");
                    return false;
                }

                _logger.LogInformation($"Post supprimer : {postId} : DAL - Delete");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la suppression du post : {postId} : DAL - Delete");
                throw new Exception($"Erreur lors de la suppression du post : {postId} : DAL - Delete", ex);
            }
        }
        #endregion

    }
}
