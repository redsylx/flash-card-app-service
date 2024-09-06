using System.Linq;
using Main.Utils;

namespace Main.Services;

public class SellCardCategoryService : ServiceBase {
    public SellCardCategoryService(Context context) : base(context)
    {
    }

    public SellCardCategory Get(string id) {
        return _context.SellCardCategory.First(p => p.Id != null);
    }

    public SellCardCategory Create(string accountId, string cardCategoryId, SellCardCategory dto) {
        var cardCategory = _context.CardCategory.First(p => p.Id == cardCategoryId);
        var account = _context.Account.First(p => p.Id == accountId);
        var newSellCardCategory = new SellCardCategory();
        newSellCardCategory.Name = dto.Name;
        newSellCardCategory.Description = dto.Description;
        newSellCardCategory.Img = dto.Img;
        newSellCardCategory.Point = dto.Point;
        newSellCardCategory.Account = account;
        newSellCardCategory.NCard = cardCategory.NCard;
        _context.SellCardCategory.Add(newSellCardCategory);
        _context.SaveChanges();
        return newSellCardCategory;
    }

    public PaginationResult<SellCardCategory> List(PaginationRequest req, string accountId)
    {
        var query = _context.SellCardCategory.Where(p => p.Account != null && p.Account.Id == accountId).AsQueryable();
        return GetPaginationResult(query, req);
    }
}