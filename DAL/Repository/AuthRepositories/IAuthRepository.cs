using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.AuthRepositories
{
    public interface IAuthRepository
    {
        Task<User> RegisterUserAsync(User user);

        Task<User?> GetUserByCredentialsAsync(string usernameOrEmail);

        Task<bool> UpdateUserPasswordAsync(Guid userId, string newPasswordHash);
    }
}
