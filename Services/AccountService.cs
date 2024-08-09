using System.Linq;
using Main.Exceptions;
using Main.Models;
using Main.Utils;

namespace Main.Services;

public class AccountService : ServiceBase {
    public AccountService(Context context): base(context)
    {
    }

    public void CheckAccount(string email) {
        var account = new Account(email);
        Validation.Validate(account);
        if(_context.Account.Any(p => p.Email == email)) return;
        _context.Account.Add(account);
        _context.SaveChanges();
    }

    public Account UpdateAccountUsername(string email, string username) {
        var existingAccount = _context.Account.FirstOrDefault(p => p.Email == email)
            ?? throw new BadRequestException("Email is not registered yet");
        existingAccount.Username = username;
        Validation.Validate(existingAccount);
        if(_context.Account.Any(p => p.Username == username)) throw new BadRequestException("Username already exist");        
        _context.Account.Update(existingAccount);
        _context.SaveChanges();
        return existingAccount;
    }
}