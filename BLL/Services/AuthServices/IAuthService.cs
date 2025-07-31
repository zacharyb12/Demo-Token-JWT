using Models.Auth_Models;
using Models.User_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.AuthServices
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(RegisterForm form);
        Task<string?> LoginAsync(Loginform form);
    }
}
