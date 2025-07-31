using DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using System.Text;

namespace DAL.Repository.AuthRepositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(DataContext context, ILogger<AuthRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Register User
        public async Task<User> RegisterUserAsync(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("User est null : DAL - Register");
                throw new ArgumentNullException(nameof(user));
            }

            if (user.Id.Equals(Guid.Empty))
            {
                _logger.LogWarning("User Id est un GuidEmpty : DAL - Register");
                throw new Exception("User Id doit etre dans un format correct");
            }

            try
            {
                // Vérifier si l'utilisateur existe déjà (contraintes DB uniqueness)
                var existingUserByUsername = await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Username == user.Username);

                if (existingUserByUsername)
                {
                    _logger.LogWarning($"Username déjà existant: {user.Username} : DAL - Register");
                    throw new Exception("Ce nom d'utilisateur est déjà utilisé");
                }

                var existingUserByEmail = await _context.Users
                    .AsNoTracking()
                    .AnyAsync(u => u.Email == user.Email);

                if (existingUserByEmail)
                {
                    _logger.LogWarning($"Email déjà existant: {user.Email} : DAL - Register");
                    throw new Exception("Cet email est déjà utilisé");
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Utilisateur créé avec succès: {user.Id} - {user.Username} : DAL - Register");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de l'inscription de l'utilisateur: {user.Username} : DAL - Register");
                throw new Exception($"Erreur lors de l'inscription de l'utilisateur: {user.Username} : DAL - Register", ex);
            }
        }
        #endregion

        #region Get User By Credentials
        public async Task<User?> GetUserByCredentialsAsync(string usernameOrEmail)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail))
            {
                _logger.LogWarning("Username/Email est vide : DAL - GetUserByCredentials");
                return null;
            }

            try
            {
                // Chercher par username ou email - retourne l'utilisateur avec son hash
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail.ToLowerInvariant());

                if (user == null)
                {
                    _logger.LogInformation($"Utilisateur introuvable: {usernameOrEmail} : DAL - GetUserByCredentials");
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la recherche de l'utilisateur: {usernameOrEmail} : DAL - GetUserByCredentials");
                throw new Exception($"Erreur lors de la recherche de l'utilisateur: {usernameOrEmail} : DAL - GetUserByCredentials", ex);
            }
        }
        #endregion

        #region Update User Password
        public async Task<bool> UpdateUserPasswordAsync(Guid userId, string newPasswordHash)
        {
            if (userId.Equals(Guid.Empty))
            {
                _logger.LogWarning($"UserId invalide: {userId} : DAL - UpdateUserPassword");
                return false;
            }

            if (string.IsNullOrWhiteSpace(newPasswordHash))
            {
                _logger.LogWarning("New password hash est vide : DAL - UpdateUserPassword");
                return false;
            }

            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning($"Utilisateur introuvable pour changement de mot de passe: {userId} : DAL - UpdateUserPassword");
                    return false;
                }

                user.PasswordHash = newPasswordHash;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Mot de passe mis à jour avec succès pour: {userId} : DAL - UpdateUserPassword");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la mise à jour du mot de passe pour: {userId} : DAL - UpdateUserPassword");
                throw new Exception($"Erreur lors de la mise à jour du mot de passe pour: {userId} : DAL - UpdateUserPassword", ex);
            }
        }
        #endregion
    }
}
