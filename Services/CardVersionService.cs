using System.Linq;
using Main.Exceptions;
using Main.Models;
using Main.Utils;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace Main.Services;

public class CardVersionService : ServiceBase {
    public CardVersionService(Context context) : base(context)
    {
    }

    public CardVersion CreateOrUpdate(string cardVersionId, string cardId, string clueTxt, string descriptionTxt, string? clueImg, string? descriptionImg) {
        if(string.IsNullOrEmpty(cardId)) throw new BadRequestException("cardId is missing");
        var card = _context.Card.FirstOrDefault(p => p.Id == cardId)
            ?? throw new BadRequestException($"Card with id {cardId} is not found");
        var existingCardVersion  = _context.CardVersion.FirstOrDefault(p => p.Id == cardVersionId);
        if (existingCardVersion != null && !_context.GameDetail.Any(p => p.CardVersion != null && p.CardVersion.Id == cardVersionId)) {
            existingCardVersion.ClueTxt = clueTxt;
            existingCardVersion.DescriptionTxt = descriptionTxt;
            existingCardVersion.ClueImg = clueImg;
            existingCardVersion.DescriptionImg = descriptionImg;
            _context.CardVersion.Update(existingCardVersion);
            _context.SaveChanges();
            return existingCardVersion;
        } else {
            if(existingCardVersion != null) cardVersionId = Guid.NewGuid().ToString();
            var newCardVersion = new CardVersion() {
                ClueTxt = clueTxt,
                ClueImg = clueImg,
                DescriptionTxt = descriptionTxt,
                DescriptionImg = descriptionImg,
                Card = card,
                Id = cardVersionId,
            };
            _context.CardVersion.Add(newCardVersion);
            _context.SaveChanges();
            return newCardVersion;
        }
    }

    public PaginationResult<CardVersion> List(PaginationRequest req, string cardId)
    {
        var query = _context.CardVersion.Where(p => p.Card != null && p.Card.Id == cardId);
        return GetPaginationResult(query, req);
    }

    public void Delete(List<string> deletedCardVersionIds) {
        var cardVersions = _context.CardVersion.Include(p => p.Card).Where(p => deletedCardVersionIds.Contains(p.Id)).ToList();
        var deletedCardVersions = cardVersions.Where(p => p.Card != null && p.Card.CurrentVersionId != p.Id).ToList();
        _context.CardVersion.RemoveRange(deletedCardVersions);
    }
}