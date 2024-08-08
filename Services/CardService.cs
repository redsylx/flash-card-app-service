using System.Linq;
using Main.Exceptions;
using Main.Models;
using Main.Utils;
using System.Linq.Dynamic.Core;

namespace Main.Services;

public class CardService : ServiceBase {
    public CardService(Context context) : base(context)
    {
    }

    public Card CreateCard(string cardCategoryId, string clueTxt, string descriptionTxt, string clueImg = "", string DescriptionImg = "") {
        if(string.IsNullOrEmpty(cardCategoryId)) throw new BadRequestException("cardCategoryId is missing");
        var cardCategory = _context.CardCategory.FirstOrDefault(p => p.Id == cardCategoryId)
            ?? throw new BadRequestException($"Category with id {cardCategoryId} is not found");
        var newCard = new Card() {
            ClueTxt = clueTxt,
            ClueImg = clueImg,
            DescriptionTxt = descriptionTxt,
            DescriptionImg = DescriptionImg,
            CardCategory = cardCategory
        };
        cardCategory.NCard += 1;
        _context.Card.Add(newCard);
        _context.SaveChanges();
        return newCard;
    }

    public PaginationResult<Card> List(PaginationRequest req)
    {
        var query = _context.Card.AsQueryable();
        return GetPaginationResult(query, req);
    }
}