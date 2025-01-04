using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class LoginUserDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }

}