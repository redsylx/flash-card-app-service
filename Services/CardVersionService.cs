using System.Linq;
using Main.Exceptions;
using Main.Models;
using Main.Utils;
using System.Linq.Dynamic.Core;

namespace Main.Services;

public class CardVersionService : ServiceBase {
    public CardVersionService(Context context) : base(context)
    {
    }

    public CardVersion Create(string cardVersionId, string cardId, string clueTxt, string descriptionTxt, string? clueImg, string? DescriptionImg) {
        if(string.IsNullOrEmpty(cardId)) throw new BadRequestException("cardId is missing");
        var card = _context.Card.FirstOrDefault(p => p.Id == cardId)
            ?? throw new BadRequestException($"Card with id {cardId} is not found");
        var newCardVersion = new CardVersion() {
            ClueTxt = clueTxt,
            ClueImg = clueImg,
            DescriptionTxt = descriptionTxt,
            DescriptionImg = DescriptionImg,
            Card = card,
            Id = cardVersionId,
        };
        _context.CardVersion.Add(newCardVersion);
        _context.SaveChanges();
        return newCardVersion;
    }

    public PaginationResult<CardVersion> List(PaginationRequest req, string cardId)
    {
        var query = _context.CardVersion.Where(p => p.Card != null && p.Card.Id == cardId);
        return GetPaginationResult(query, req);
    }
}