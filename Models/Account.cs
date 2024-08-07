using Main.Exceptions;
using Main.Utils;

namespace Main.Models;

public class Account : ModelBase {
    public string? Email { get; set; }
    public string? Username { get; set; }
    public Account(string email, string username)
    {
        Email = email;
        Username = username;
    }

    public void Validate() {
        if(string.IsNullOrEmpty(Username)) throw new BadRequestException("Username Can't be empty");
        if(string.IsNullOrEmpty(Email)) throw new BadRequestException("Email Can't be empty");
        if(Username.Length < 3 || Username.Length > 12) throw new BadRequestException("Username must be between 3 and 12 characters");
        var cleanUsername = Username.Replace("_", "");
        if(!Validation.IsAlphanumeric(cleanUsername)) throw new BadRequestException("Username can only be alphanumeric and underscore");
        Email = Email.ToLower();
        Username = Username.ToLower();
    }
}