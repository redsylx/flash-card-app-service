using System.Collections.Generic;
using System.Linq;
using Main.Consts;
using Main.Exceptions;
using Main.Models;
using Main.Utils;
using Microsoft.EntityFrameworkCore;

namespace Main.Services;

public class GameService : ServiceBase {
    public GameService(Context context) : base(context)
    {
    }

    public Game Get(string accountId, string gameId) {
        var game = _context.Game.FirstOrDefault(p => p.Account != null && p.Account.Id == accountId && p.Id == gameId)
            ?? throw new BadRequestException($"Game with id {gameId} is missing");
        return game;
    }

    public Game? GetResume(string accountId) {
        var game = _context.Game.FirstOrDefault(p => p.Account != null && p.Account.Id == accountId && p.Status == GameConst.PLAYING);
        return game;
    }

    public Game Create(string accountId, int nCard, int hideDurationInSecond) {
        if(string.IsNullOrEmpty(accountId)) throw new BadRequestException("accountId is missing");
        var account = _context.Account.FirstOrDefault(p => p.Id == accountId)
            ?? throw new BadRequestException($"Account with id {accountId} is not found");
        var newGame = new Game() {
            Account = account,
            NCard = nCard,
            HideDurationInSecond = hideDurationInSecond,
        };

        _context.Game.Add(newGame);
        _context.SaveChanges();
        return newGame;
    }

    public List<Game> GetPlayingGames(string accountId) {
        var otherGamePlaying = _context.Game.Where(p => p.Account != null && p.Account.Id == accountId && p.Status == GameConst.PLAYING).ToList();
        return otherGamePlaying;
    }

    public void DeletePlayingGames(string accountId) {
        _context.Game.Where(p => p.Account != null && p.Account.Id == accountId && p.Status == GameConst.PLAYING).ExecuteDelete();
        _context.SaveChanges();
    }

    public Game Finish(string gameId) {
        if(string.IsNullOrEmpty(gameId)) throw new BadRequestException("gameId is missing");
        var existingGame = _context.Game.FirstOrDefault(p => p.Id == gameId)
            ?? throw new BadRequestException($"Card with id {gameId} is not found");
        if(existingGame.Status == GameConst.FINISH) return existingGame;
        var isNotAnsweredAll = _context.GameDetail.Any(p => p.Game != null && p.Game.Id == gameId && !p.IsAnswered);
        var correct = _context.GameDetail.Count(p => p.Game != null && p.Game.Id == gameId && p.IsCorrect);
        if(isNotAnsweredAll) throw new BadRequestException($"All card must be answered");
        existingGame.Finish(correct);
        _context.Game.Update(existingGame);
        _context.SaveChanges();
        return existingGame;
    }

    public PaginationResult<Game> List(PaginationRequest req, string accountId)
    {
        var query = _context.Game.Where(p => p.Account != null && p.Account.Id == accountId).AsQueryable();
        return GetPaginationResult(query, req);
    }
}