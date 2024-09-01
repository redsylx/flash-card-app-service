using System;
using System.Collections.Generic;
using System.Linq;
using Main.Consts;
using Main.Exceptions;
using Main.Models;
using Main.Utils;
using Microsoft.EntityFrameworkCore;

namespace Main.Services;

public class GameDetailService : ServiceBase {
    public GameDetailService(Context context) : base(context)
    {
    }

    public void Create(List<string> cardIds, string gameId) {
        if(cardIds is null || cardIds.Count == 0)
            throw new BadRequestException("cardIds is required");
        var game = _context.Game.FirstOrDefault(p => p.Id == gameId)
            ?? throw new BadRequestException($"Game with id {gameId} is not found");
        var mapCard = _context.Card.Include(p => p.CardCategory).Where(p => cardIds.Contains(p.Id)).ToList().ToDictionary(p => p.Id);
        var indexNumber = 0;
        var newGameDetails = new List<GameDetail>();
        foreach(var cardId in cardIds) {
            var card = mapCard.GetValueOrDefault(cardId);
            if(card == null) return;
            var newGameDetail = new GameDetail();
            newGameDetail.Game = game;
            newGameDetail.IndexNumber = indexNumber++;
            newGameDetail.CardId = cardId;
            newGameDetail.ClueImg = card.ClueImg;
            newGameDetail.ClueTxt = card.ClueTxt;
            newGameDetail.CategoryName = card.CardCategory.Name;
            newGameDetail.DescriptionTxt = card.DescriptionTxt;
            newGameDetails.Add(newGameDetail);
        }
        _context.GameDetail.AddRange(newGameDetails);
        _context.SaveChanges();
    }

    public void DeleteAll(List<string> gameIds) {
        _context.GameDetail.Where(p => p.Game != null && gameIds.Contains(p.Game.Id)).ExecuteDelete();
        _context.SaveChanges();
    }

    public GameDetail Answer(GameDetail gameDetail) {
        if(gameDetail is null)
            throw new BadRequestException("GameDetail is missing");
        var gameId = gameDetail.Game?.Id ?? "";
        var existingGame = _context.Game.FirstOrDefault(p => p.Id == gameId && p.Status == GameConst.PLAYING)
            ?? throw new BadRequestException($"Game with id {gameId} is not found or already finish");
        var currentGameDetail = _context.GameDetail.FirstOrDefault(p => p.Id == gameDetail.Id && !p.IsAnswered)
            ?? throw new BadRequestException($"GameDetail with id {gameDetail.Id} is not found");
        currentGameDetail.Update(gameDetail.IsCorrect);
        _context.GameDetail.Update(currentGameDetail);
        _context.SaveChanges();
        return currentGameDetail;
    }

    public List<GameDetail> List(string gameId) {
        return [.. _context.GameDetail.Where(p => p.Game != null && p.Game.Id == gameId).OrderBy(p => p.IndexNumber)];
    }

    public PaginationResult<GameDetail> List(PaginationRequest req, string gameId)
    {
        var query = _context.GameDetail.Where(p => p.Game != null && p.Game.Id == gameId).AsQueryable();
        return GetPaginationResult(query, req);
    }

    // public List<string> Delete(string gameId, string accountId)
    // {
    //     var gameDetails = _context.GameDetail.Include(p => p.Game).Where(p => p.Game != null && p.Game.Account != null && p.Game.Account.Id == accountId && p.Game.Status == GameConst.FINISH).ToList();
    //     var deletedGameDetails = gameDetails.Where(p => p.Game != null && p.Game.Id == gameId);
    //     var otherGameDetails = gameDetails.Where(p => p.Game != null && p.Game.Id != gameId);

    //     var deletedCardVersionIds = deletedGameDetails.Select(p => p.CardVersion?.Id ?? "").ToHashSet();
    //     var otherCardVersionIds = otherGameDetails.Select(p => p.CardVersion?.Id ?? "").ToHashSet();

    //     _context.GameDetail.RemoveRange(deletedGameDetails);
        
    //     return deletedCardVersionIds.Except(otherCardVersionIds).ToList();
    // }
}