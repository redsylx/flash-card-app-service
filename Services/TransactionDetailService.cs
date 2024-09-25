using System.Collections.Generic;
using System.Linq;
using Main.Exceptions;
using Main.Models;
using Main.Utils;
using Microsoft.EntityFrameworkCore;

namespace Main.Services;

public class TransactionDetailService : ServiceBase
{
    public TransactionDetailService(Context context) : base(context)
    {
    }

    public PaginationResult<TransactionDetail> ListByBuyerTransactionId(PaginationRequest req, string transactionId)
    {
        var query = _context.TransactionDetail.Include(p => p.SellCardCategory).ThenInclude(p => p.Account).Where(p => p.TransactionActivityBuyer != null && p.TransactionActivityBuyer.Id == transactionId).AsQueryable();
        return GetPaginationResult(query, req);
    }
}