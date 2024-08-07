using System.Linq;
using Main.Exceptions;
using Main.Models;

namespace Main.Services;

public class AccountService : ServiceBase {
    private readonly Context _context;
    public AccountService(Context context)
    {
        _context = context;
    }

    public Account CreateAccount(string email, string username) {
        var newAccount = new Account(email, username);
        newAccount.GenerateId();
        newAccount.Validate();

        if(_context.Account.Any(p => p.Username == username)) throw new BadRequestException("Username already exist");
        if(_context.Account.Any(p => p.Email == email)) throw new BadRequestException("Email already used");
        
        _context.Account.Add(newAccount);
        _context.SaveChanges();
        return newAccount;
    }
}