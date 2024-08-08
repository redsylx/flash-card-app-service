using System.Linq;
using Main.Exceptions;
using Main.Models;

namespace Main.Services;

public class AccountService : ServiceBase {
    public AccountService(Context context): base(context)
    {
    }

    public Account CreateAccount(string email, string username) {
        var newAccount = new Account(email, username);
        newAccount.Validate();

        if(_context.Account.Any(p => p.Username == username)) throw new BadRequestException("Username already exist");
        if(_context.Account.Any(p => p.Email == email)) throw new BadRequestException("Email already used");
        
        _context.Account.Add(newAccount);
        _context.SaveChanges();
        return newAccount;
    }
}