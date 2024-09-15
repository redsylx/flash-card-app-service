using System.Collections.Generic;
using System.Linq;
using Main.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Services;

public class CartDetailService : ServiceBase {
    public CartDetailService(Context context) : base(context)
    {
    }

    public CartDetail AddCartDetail(string cartId, string sellCardCategoryId) {
        var existingItem = _context.CartDetail.FirstOrDefault(p => p.Cart != null && p.Cart.Id == cartId && p.SellCardCategory != null && p.SellCardCategory.Id == sellCardCategoryId);
        if(existingItem != null) return existingItem;
        var cart = _context.Cart.First(p => p.Id == cartId);
        var category = _context.SellCardCategory.First(p => p.Id == sellCardCategoryId);
        var newItem = new CartDetail();
        newItem.Cart = cart;
        newItem.SellCardCategory = category;
        _context.CartDetail.Add(newItem);
        _context.SaveChanges();
        return newItem;
    }

    public void Remove(string cartId, string sellCardCategoryId) {
        var existingItem = _context.CartDetail.FirstOrDefault(p => p.Cart != null && p.Cart.Id == cartId && p.SellCardCategory != null && p.SellCardCategory.Id == sellCardCategoryId);
        if(existingItem == null) return;
        _context.CartDetail.Remove(existingItem);
        _context.SaveChanges();
    }

    public List<CartDetail> GetCartDetailsByCartId(string id) {
        return _context.CartDetail.Include(p => p.SellCardCategory).Where(p => p.Cart != null && p.Cart.Id == id).ToList();
    }
}