using System.Collections.Generic;
using System.Linq;
using Main.Consts;
using Main.Exceptions;
using Main.Models;
using Main.Utils;

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

        var newGameDetails = cardVersionIds.Select(p => {
            if(!mapCardVersion.TryGetValue(p, out var cardVersion)) throw new BadRequestException($"cardVersion id {p} is not found");
            return new GameDetail {
                CardVersion = cardVersion,
                Game = game
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
        var currentGameDetail = _context.GameDetail.FirstOrDefault(p => p.Id == gameDetail.Id)
            ?? throw new BadRequestException($"GameDetail with id {gameDetail.Id} is not found");
        if(gameDetail.IsCorrect is null)
            throw new BadRequestException($"field IsCorrect can only be true or false");
        currentGameDetail.Update(gameDetail.IsCorrect);
        _context.GameDetail.Update(currentGameDetail);
        _context.SaveChanges();
        return currentGameDetail;
    }

    public PaginationResult<GameDetail> List(PaginationRequest req, string gameId)
    {
        var query = _context.GameDetail.Where(p => p.Game != null && p.Game.Id == gameId).AsQueryable();
        return GetPaginationResult(query, req);
    }
}