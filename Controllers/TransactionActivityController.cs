using Main.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Main.Utils;

namespace Main.Controllers;

public class TransactionActivityController : ControllerBase<TransactionActivityController> {
    public TransactionActivityController(ILogger<TransactionActivityController> logger, Context context) : base(context, logger)
    {
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Post([FromQuery] string accountId, [FromQuery] string cartId) {
        var transactionService = new TransactionActivityService(_context);
        var sellCardCategoryService = new SellCardCategoryService(_context);
        var cartDetailService = new CartDetailService(_context);
        var cartService = new CartService(_context);
        var accountService = new AccountService(_context);
        var pointActivityService = new PointActivityService(_context);

        var transaction = transactionService.Checkout(accountId, cartId);
        sellCardCategoryService.CalculateSold(transaction.Id);
        cartDetailService.RemoveAll(cartId);
        cartService.CalculateNItems(cartId);
        pointActivityService.FinishTransaction(transaction.Id);
        accountService.FinishTransaction(transaction.Id);
        return new OkResult();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Get([FromQuery] string accountId, [FromQuery] PaginationRequest req) {
        var transactionService = new TransactionActivityService(_context);
        var transaction = transactionService.ListByBuyerAccountId(req, accountId);
        return new OkObjectResult(transaction);
    }
}