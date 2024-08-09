using System.ComponentModel.DataAnnotations;

namespace Main.Models;

public class Account : ModelBase {
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [StringLength(12, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 12 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Username can only contain alphanumeric characters and underscores.")]
    public string? Username { get; set; }
    
    public Account(string email) {
        Email = email;
    }

    public Account(string email, string username)
    {
        Email = email;
        Username = username;
    }

    public Account() {}
}