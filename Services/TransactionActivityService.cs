using System.Collections.Generic;
using System.Linq;
using Main.Consts;
using Main.Exceptions;
using Main.Models;
using Main.Utils;
using Microsoft.EntityFrameworkCore;

namespace Main.Services;

public class TransactionActivityService : ServiceBase
{
    public TransactionActivityService(Context context) : base(context)
    {
    }

    public TransactionActivity Checkout(string accountId, string cartId)
    {
        var account = _context.Account.First(p => p.Id == accountId);
        var sellCardCategories = _context.CartDetail.Include(p => p.SellCardCategory).ThenInclude(p => p.Account).Where(p => p.Cart != null && p.Cart.Id == cartId).Select(p => p.SellCardCategory).ToList();

        var requiredPoint = sellCardCategories.Sum(p => p.Point);
        if(requiredPoint > account.Point) throw new BadRequestException("Point is not enough");

        var mapTransactionByAccountId = new Dictionary<string, TransactionActivity>();
        var transactionDetails = new List<TransactionDetail>();

        var newTransactionBuyer = new TransactionActivity(account, TransactionActivityConst.Category.BUYER, -requiredPoint);
        newTransactionBuyer.TotalItem = sellCardCategories.Count;

        mapTransactionByAccountId.Add(accountId, newTransactionBuyer);

        foreach (var sellCardCategory in sellCardCategories)
        {
            var transactionSeller = mapTransactionByAccountId.GetValueOrDefault(sellCardCategory?.Account?.Id) ?? new TransactionActivity(sellCardCategory.Account, TransactionActivityConst.Category.SELLER);
            var transactionBuyer = mapTransactionByAccountId.GetValueOrDefault(accountId);
            if(transactionBuyer == null || transactionSeller == null) continue;
            var transactionDetail = new TransactionDetail();
            transactionDetail.TransactionActivitySeller = transactionSeller;
            transactionDetail.TransactionActivityBuyer = transactionBuyer;
            transactionDetail.SellCardCategory = sellCardCategory;
            transactionDetails.Add(transactionDetail);
            transactionSeller.TotalPoint += sellCardCategory.Point;
            transactionSeller.TotalItem += 1;
        }

        _context.TransactionActivity.AddRange(mapTransactionByAccountId.Values);
        _context.TransactionDetail.AddRange(transactionDetails);
        _context.SaveChanges();

        return newTransactionBuyer;
    }

    public PaginationResult<TransactionActivity> ListByBuyerAccountId(PaginationRequest req, string accountId)
    {
        var query = _context.TransactionActivity.Where(p => p.Account != null && p.Account.Id == accountId && p.Category == TransactionActivityConst.Category.BUYER).AsQueryable();
        return GetPaginationResult(query, req);
    }
}