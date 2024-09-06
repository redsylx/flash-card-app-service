using System.Collections.Generic;
using System.Linq;
using Main.Utils;

namespace Main.Services;

public class SellCardService : ServiceBase {
    public SellCardService(Context context) : base(context)
    {
    }

    public SellCard Get(string id) {
        return _context.SellCard.First(p => p.Id != null);
    }

    public List<SellCard> Create(string sellCardCategoryId, string cardCategoryId) {
        var sellCardCategory = _context.SellCardCategory.First(p => p.Id == sellCardCategoryId);
        var cards = _context.Card.Where(p => p.CardCategory != null && p.CardCategory.Id == cardCategoryId).ToList();
        var newSellCards = new List<SellCard>();

        foreach(var card in cards) {
            var newSellCard = new SellCard();
            newSellCard.ClueImg = card.ClueImg ?? "";
            newSellCard.ClueTxt = card.ClueTxt;
            newSellCard.DescriptionTxt = card.DescriptionTxt;
            newSellCard.SellCardCategory = sellCardCategory;
            newSellCards.Add(newSellCard);
        }

        _context.SellCard.AddRange(newSellCards);
        _context.SaveChanges();
        return newSellCards;
    }

    public PaginationResult<SellCard> List(PaginationRequest req, string sellCardCategoryId)
    {
        var query = _context.SellCard.Where(p => p.SellCardCategory != null && p.SellCardCategory.Id == sellCardCategoryId).AsQueryable();
        return GetPaginationResult(query, req);
    }
}