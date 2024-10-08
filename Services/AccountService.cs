using System;
using System.Linq;
using Main.Exceptions;
using Main.Models;
using Main.Utils;
using Microsoft.EntityFrameworkCore;

namespace Main.Services;

public class AccountService : ServiceBase {
    public AccountService(Context context): base(context)
    {
    }

    public void FinishTransaction(string transactionId) {
        var transactionIds = _context.TransactionDetail.Include(p => p.TransactionActivitySeller).Where(p => p.TransactionActivityBuyer != null && p.TransactionActivityBuyer.Id == transactionId).Select(p => p.TransactionActivitySeller.Id).ToList();
        transactionIds.Add(transactionId);
        var pointActivities = _context.PointActivity.Include(p => p.Account).Where(p => transactionIds.Contains(p.ActivityId));
        foreach(var pointActivity in pointActivities) {
            var account = pointActivity.Account;
            account.Point += pointActivity.Point;
        }
        _context.Account.UpdateRange(pointActivities.Select(p => p.Account));
        _context.SaveChanges();
    }

    public Account CheckAccount(string email) {
        var account = new Account(email);
        Validation.Validate(account);
        var existingAccount = _context.Account.FirstOrDefault(p => p.Email == email);
        if(existingAccount != null) return existingAccount;
        _context.Account.Add(account);
        _context.SaveChanges();
        return account;
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

    public Account UpdatePoint(string accountId, string pointActivityId) {
        var account = _context.Account.First(p => p.Id == accountId);
        var pointActivity = _context.PointActivity.First(p => p.Id == pointActivityId);
        account.Point += pointActivity.Point;
        _context.Account.Update(account);
        _context.SaveChanges();
        return account;
    }
}