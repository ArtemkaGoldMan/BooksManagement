using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BaseLibrary.DTOs;

namespace WPFClient.Services
{
    public interface IAuthenticationService
    {
        // Method to login the user and retrieve the JWT token
        Task<bool> LoginAsync(LoginUserDTO loginDto);

        // Method to register a new user
        Task<bool> RegisterAsync(RegisterUserDTO registerDto);

        Task<bool> DeleteUserAsync(int userId);

        ClaimsPrincipal GetAuthenticationState();

        Task MarkUserAsLoggedIn(string token);

        IEnumerable<Claim> ParseClaimsFromJwt(string jwt);

        Task<bool> IsTokenValidAsync();


        // Method to set the Authorization header for HTTP requests (with the JWT token)
        void SetAuthorizationHeader(ClaimsPrincipal user);


        // Method to log out the user (clear the token and authorization header)
        void Logout();

        string Base64UrlDecode(string input);


        // Method to check if the user is authenticated (i.e., if a token is available)
        bool IsAuthenticated();
    }
}
