using DAL.Repository.UserRepositories;
using Models;
using Models.User_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        #region Get by id
        public async Task<UserRead?> GetUserByIdAsync(Guid userId)
        {
            var userRead =  await _repository.GetUserByIdAsync(userId);

            if(userRead is not null)
            {

                return new UserRead
                {
                    Id = userRead?.Id ?? Guid.Empty,
                    Username = userRead?.Username ?? string.Empty,
                    Email = userRead?.Email ?? string.Empty,
                    Address = userRead?.Address ?? string.Empty,
                    Role = userRead?.Role ?? string.Empty,
                    Posts = userRead?.Posts
                };
            }
            return null;
        }
        #endregion


        #region GetAll Users
        public async Task<IEnumerable<UserRead>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllUsersAsync();

            return users.Select(user => new UserRead
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Address = user.Address,
                Role = user.Role,
                Posts = user.Posts
            });
        }
        #endregion


        #region Update User
        public async Task<UserRead?> UpdateUserAsync(UserUpdate userUpdate)
        {
            var existingUser = await _repository.GetUserByIdAsync(userUpdate.Id);
            if (existingUser == null)
            {
                return null;
            }

            existingUser.Username = userUpdate.Username;
            existingUser.Email = userUpdate.Email;
            existingUser.Address = userUpdate.Address;


            var updatedUser = await _repository.UpdateUserAsync(existingUser);

            return new UserRead
            {
                Id = updatedUser.Id,
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                Address = updatedUser.Address,
                Role = updatedUser.Role,
                Posts = updatedUser.Posts
            };
        }
        #endregion


        #region Delete User
        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            return await _repository.DeleteUserAsync(userId);
        }
        #endregion

    }
}
