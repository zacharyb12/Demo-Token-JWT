using DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.UserRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(DataContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        # region Get User By Id
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            if (userId .Equals(Guid.Empty))
            {
                _logger.LogWarning($"userId est null : {userId} : DAL - GetById");
                return null;
            }

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    _logger.LogInformation($"Utilisateur introuvable avec l'id : { userId}  : DAL - GetById");
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la recuperation de l'utilisateur  ID: {userId} :  : DAL - GetById");
                throw new Exception($"Erreur lors de la recuperation de l'utilisateur {userId} :  : DAL - GetById", ex);
            }
        }
        #endregion


        #region GetAll Users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users
                    .AsNoTracking()
                    .OrderBy(u => u.Username) // Ordre cohérent
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recuperation des utilisateurs : DAL - GetAll");
                throw new Exception("Erreur lors de la recuperation des utilisateurs ", ex);
            }
        }
        #endregion


        #region Update User
        public async Task<User> UpdateUserAsync(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("Utilisateur est null :  DAL - Update");
                throw new ArgumentNullException(nameof(user));
            }

            if (user.Id.Equals(Guid.Empty))
            {
                _logger.LogWarning("Utilisateur Id est un GuidEmpty :  DAL - Update");
                throw new Exception("Utilisateur Id doit etre dans un format correct");
            }

            try
            {
                var existingUser = await _context.Users.FindAsync(user.Id);

                if (existingUser == null)
                {
                    _logger.LogWarning($"Aucun utilisateur avec un id: {user.Id} : DAL - Update");
                    throw new Exception($"Aucun utilisateur avec un id {user.Id}");
                }

                _context.Entry(existingUser).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Utilisateur mis a jour: {user.Id} : :  DAL - Update");
                return existingUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la mise a jour de l'utilisateur: {user.Id}");
                throw new Exception($"Erreur lors de la mise a jour de l'utilisateur: {user.Id}", ex);
            }
        }
        #endregion


        #region Delete User
        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            if (userId.Equals(Guid.Empty))
            {
                _logger.LogWarning($"UserId invalide: {userId} : :  DAL - Delete");
                return false;
            }

            try
            {
                var rowsAffected = await _context.Users
                    .Where(u => u.Id.Equals(userId))
                    .ExecuteDeleteAsync();

                if (rowsAffected == 0)
                {
                    _logger.LogInformation($"Utilisateur introuvable pour suppression: {userId} : :  DAL - Delete");
                    return false;
                }

                _logger.LogInformation($"Utilisateur supprimer : {userId} : :  DAL - Delete");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la suppression de l'utilisateur : {userId} : :  DAL - Delete");
                throw new Exception($"Erreur lors de la suppression de l'utilisateur :  {userId} ", ex);
            }
        }
        #endregion

    }
}
