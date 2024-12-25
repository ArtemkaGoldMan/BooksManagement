using System.Collections.Generic;

namespace BaseLibrary.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // Admin or Member
        public string PasswordHash { get; set; } // Encrypted password
        public ICollection<Borrow> Borrows { get; set; } // Navigation property
    }
}
