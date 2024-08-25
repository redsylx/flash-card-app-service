using System.Collections.Generic;
using System.Linq;
using Main.Consts;
using Main.Exceptions;
using Main.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Main.Services;

public class CardCategoryService : ServiceBase {
    public CardCategoryService(Context context) : base(context)
    {
    }

    public List<CardCategory> GetCardCategories(string accountId) {
        return [.. _context.CardCategory.Include(p => p.Account).Where(p => p.Account != null && p.Account.Id == accountId)];
    }

    public CardCategory CountNCard(string cardCategoryId) {
        var cardCategory = _context.CardCategory.FirstOrDefault(p => p.Id == cardCategoryId)
            ?? throw new BadRequestException($"Category with id {cardCategoryId} is not found");
        var totalCard = _context.Card.Count(p => p.CardCategory != null && p.CardCategory.Id == cardCategoryId);
        cardCategory.NCard = totalCard;
        _context.CardCategory.Update(cardCategory);
        _context.SaveChanges();
        return cardCategory;
    }

    public void Update(string accountId, string cardCategoryId, string categoryName) {
        var cardCategory = _context.CardCategory.FirstOrDefault(p => p.Account != null && p.Account.Id == accountId && p.Id == cardCategoryId) 
            ?? throw new BadRequestException($"Category with id {cardCategoryId} is not found");
        cardCategory.Name = categoryName;
        cardCategory.Validate();
        if(_context.CardCategory.Any(p => p.Account != null && p.Account.Id == accountId && p.Name == cardCategory.Name))
            throw new BadRequestException($"Category with name '{cardCategory.Name}' is already exist");
        _context.CardCategory.Update(cardCategory);
        _context.SaveChanges();
    }

    public void Delete(string accountId, string categoryId) {
        var cardCategory = _context.CardCategory.FirstOrDefault(p => p.Account != null && p.Account.Id == accountId && p.Id == categoryId)
            ?? throw new BadRequestException($"Category with id {categoryId} is not found");
        if (cardCategory.Name == DefaultNameConst.CARD_CATEGORY) throw new BadRequestException($"Category '{DefaultNameConst.CARD_CATEGORY}' cant be deleted");
        if (cardCategory.NCard == 0) _context.CardCategory.Remove(cardCategory);
        else {
            cardCategory.IsDelete = true;
            _context.CardCategory.Update(cardCategory);
        }
        _context.SaveChanges();
    }

    public CardCategory Create(string accountId, string name) {
        var account = _context.Account.FirstOrDefault(p => p.Id == accountId) 
            ?? throw new BadRequestException($"Account with id {accountId} is not found");
        var newCardCategory = new CardCategory() {
            Account = account,
            Name = name
        };
        newCardCategory.Validate();
        if(_context.CardCategory.Any(p => p.Account != null && p.Account.Id == accountId && p.Name == newCardCategory.Name))
            throw new BadRequestException($"Category with name '{newCardCategory.Name}' is already exist");
        _context.CardCategory.Add(newCardCategory);
        _context.SaveChanges();
        return newCardCategory;
    }
}