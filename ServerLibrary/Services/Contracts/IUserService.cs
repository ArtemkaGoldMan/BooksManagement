using BaseLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Services.Contracts
{
    public interface IUserService
    {
        Task<UserDTO?> AuthenticateAsync(LoginUserDTO login);
        Task<UserDTO> RegisterAsync(RegisterUserDTO user);
        Task<string?> GenerateJwtTokenAsync(string email); // Generates JWT token
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<bool> DeleteUserAsync(int id);
    }
}
