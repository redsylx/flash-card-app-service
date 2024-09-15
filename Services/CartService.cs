using System.Linq;
using Main.Models;
using Main.Utils;

namespace Main.Services;

public class CartService : ServiceBase {
    public CartService(Context context) : base(context)
    {
    }

    public void CalculateNItems(string cartId) {
        var cart = Get(cartId);
        var nItems = _context.CartDetail.Count(p => p.Cart != null && p.Cart.Id == cartId);
        cart.NItems = nItems;
        _context.Cart.Update(cart);
        _context.SaveChanges();
    }

    public Cart Get(string id) {
        return _context.Cart.First(p => p.Id == id);
    }

    public Cart GetByAccountId(string id) {
        return _context.Cart.First(p => p.Account != null && p.Account.Id == id);
    }

    public Cart Create(string accountId) {
        var account = _context.Account.First(p => p.Id == accountId);
        var existingItem = _context.Cart.FirstOrDefault(p => p.Account != null && p.Account.Id == accountId);
        if(existingItem != null) return existingItem; 
        var newItem = new Cart();
        newItem.Account = account;
        _context.Cart.Add(newItem);
        _context.SaveChanges();
        return newItem;
    }

    public PaginationResult<SellCardCategory> ListByAccount(PaginationRequest req, string accountId)
    {
        var query = _context.SellCardCategory.Where(p => p.Account != null && p.Account.Id == accountId).AsQueryable();
        return GetPaginationResult(query, req);
    }
    
    public PaginationResult<SellCardCategory> ListExcludeAccount(PaginationRequest req, string accountId)
    {
        var query = _context.SellCardCategory.Where(p => p.Account != null && p.Account.Id != accountId).AsQueryable();
        return GetPaginationResult(query, req);
    }
}