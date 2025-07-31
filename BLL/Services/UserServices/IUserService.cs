using Models;
using Models.User_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.UserServices
{
    public interface IUserService
    {
        Task<UserRead?> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<UserRead>> GetAllUsersAsync();
        Task<UserRead> UpdateUserAsync(UserUpdate user);
        Task<bool> DeleteUserAsync(Guid userId);
    }
}
