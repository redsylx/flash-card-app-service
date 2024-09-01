using System.Linq;
using Main.Exceptions;
using Main.Models;
using Main.Utils;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;

namespace Main.Services;

public class CardService : ServiceBase {
    public CardService(Context context) : base(context)
    {
    }

    public Card CreateCard(string cardCategoryId, string clueTxt, string descriptionTxt, string? clueImg, string? descriptionImg) {
        if(string.IsNullOrEmpty(cardCategoryId)) throw new BadRequestException("cardCategoryId is missing");
        var cardCategory = _context.CardCategory.FirstOrDefault(p => p.Id == cardCategoryId)
            ?? throw new BadRequestException($"Category with id {cardCategoryId} is not found");
        var newCard = new Card() {
            ClueTxt = clueTxt,
            ClueImg = clueImg,
            DescriptionTxt = descriptionTxt,
            DescriptionImg = descriptionImg,
            CardCategory = cardCategory
        };
        _context.Card.Add(newCard);
        _context.SaveChanges();
        return newCard;
    }

    public Card Update(string cardId, string clueTxt, string descriptionTxt, string? clueImg, string? descriptionImg) {
        if(string.IsNullOrEmpty(cardId)) throw new BadRequestException("cardId is missing");
        var existingCard = _context.Card.FirstOrDefault(p => p.Id == cardId)
            ?? throw new BadRequestException($"Card with id {cardId} is not found");
        existingCard.Update(clueTxt, descriptionTxt, clueImg, descriptionImg);
        _context.Card.Update(existingCard);
        _context.SaveChanges();
        return existingCard;
    }

    public List<Card> Generate(int nCard, List<string>? CategoryIds) {
        if(nCard <= 0) throw new BadRequestException("ncard must not be less than 1");
        if(CategoryIds is null || CategoryIds.Count == 0)
            throw new BadRequestException("CategoryIds is required");
        var cards = _context.Card.Where(p => p.CardCategory != null && CategoryIds.Contains(p.CardCategory.Id)).ToList();
        var selectedCards = new List<Card>();
        for(var i = 0; i < nCard; i++) {
            selectedCards.Add(cards[Random.Shared.Next(cards.Count)]);
        }
        return selectedCards;
    }

    public void Finish(string gameId) {
        var gameDetails = _context.GameDetail.Where(p => p.Game != null && p.Game.Id == gameId).ToList();
        if(gameDetails.Count == 0) throw new BadRequestException($"GameDetails with gameid {gameId} are not found");
        var setCardIds = gameDetails.Select(p => p.CardId).ToHashSet();
        var cards = _context.Card.Where(p => setCardIds.Contains(p.Id)).ToList();
        var mapGameDetails = gameDetails.GroupBy(p => p.CardId).ToDictionary(p => p.Key, p => p.ToList());
        foreach (var card in cards) {
            if(mapGameDetails.TryGetValue(card.Id, out var listGameDetail))
            listGameDetail.ForEach(p => card.Update(p.IsCorrect));
            card.CalculatePctCorrect();
        }
        _context.Card.UpdateRange(cards);
        _context.SaveChanges();
    }

    public PaginationResult<Card> List(PaginationRequest req, string cardCategoryId)
    {
        var query = _context.Card.Where(p => p.CardCategory != null && p.CardCategory.Id == cardCategoryId).AsQueryable();
        return GetPaginationResult(query, req);
    }

    public void DeleteByCategory(string categoryId) {
        _context.Card.Where(p => p.CardCategory != null && p.CardCategory.Id == categoryId).ExecuteDelete();
    }

    public Card Delete(string cardId) {
        var card = _context.Card.Include(p => p.CardCategory).FirstOrDefault(p => p.Id == cardId) 
            ?? throw new BadRequestException($"Card with id {cardId} is not found");
        _context.Card.Remove(card);
        _context.SaveChanges();
        return card;
    }
}