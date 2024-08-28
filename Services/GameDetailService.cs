using System;
using System.Collections.Generic;
using System.Linq;
using Main.Consts;
using Main.Exceptions;
using Main.Models;
using Main.Utils;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.EntityFrameworkCore;

namespace Main.Services;

public class GameDetailService : ServiceBase {
    public GameDetailService(Context context) : base(context)
    {
    }

    public void Create(List<string> cardVersionIds, string gameId) {
        if(cardVersionIds is null || cardVersionIds.Count == 0)
            throw new BadRequestException("cardVersionIds is required");
        var game = _context.Game.FirstOrDefault(p => p.Id == gameId)
            ?? throw new BadRequestException($"Game with id {gameId} is not found");
        var mapCardVersion = _context.CardVersion.Where(p => cardVersionIds.Contains(p.Id)).ToList().ToDictionary(p => p.Id);
        var indexNumber = 0;
        var newGameDetails = cardVersionIds.Select(p => {
            if(!mapCardVersion.TryGetValue(p, out var cardVersion)) throw new BadRequestException($"cardVersion id {p} is not found");
            return new GameDetail {
                CardVersion = cardVersion,
                Game = game,
                IndexNumber = indexNumber++
            };
        }).ToList();
        _context.GameDetail.AddRange(newGameDetails);
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
        return [.. _context.GameDetail.Include(p => p.CardVersion).ThenInclude(p => p.Card).ThenInclude(p => p.CardCategory).Where(p => p.Game != null && p.Game.Id == gameId).OrderBy(p => p.IndexNumber)];
    }

    public PaginationResult<GameDetail> List(PaginationRequest req, string gameId)
    {
        var query = _context.GameDetail.Where(p => p.Game != null && p.Game.Id == gameId).AsQueryable();
        return GetPaginationResult(query, req);
    }

    public List<string> Delete(string gameId, string accountId)
    {
        var gameDetails = _context.GameDetail.Include(p => p.Game).Where(p => p.Game != null && p.Game.Account != null && p.Game.Account.Id == accountId && p.Game.Status == GameConst.FINISH).ToList();
        var deletedGameDetails = gameDetails.Where(p => p.Game != null && p.Game.Id == gameId);
        var otherGameDetails = gameDetails.Where(p => p.Game != null && p.Game.Id != gameId);

        var deletedCardVersionIds = deletedGameDetails.Select(p => p.CardVersion?.Id ?? "").ToHashSet();
        var otherCardVersionIds = otherGameDetails.Select(p => p.CardVersion?.Id ?? "").ToHashSet();

        _context.GameDetail.RemoveRange(deletedGameDetails);
        
        return deletedCardVersionIds.Except(otherCardVersionIds).ToList();
    }
}