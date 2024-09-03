using System;
using System.Linq;
using Main.Consts;
using Main.Exceptions;

namespace Main.Services;

public class PointActivityService : ServiceBase {
    public PointActivityService(Context context): base(context)
    {
    }

    public PointActivity FinishGame(string gameId, string accountId) {
        var game = _context.Game.FirstOrDefault(p => p.Id == gameId) ?? throw new BadRequestException($"Game with id {gameId} not found");
        if (game.Status != GameConst.FINISH) throw new BadRequestException("Invalid game");
        var existingPointWithActivity = _context.PointActivity.FirstOrDefault(p => p.Account != null && p.Account.Id == accountId && p.ActivityId == gameId);
        if (existingPointWithActivity != null) throw new BadRequestException($"Activity is already registered as point");
        var account = _context.Account.FirstOrDefault(p => p.Id == accountId) ?? throw new BadRequestException($"Account with id {accountId} not found");
        var newPointActivity = new PointActivity();
        newPointActivity.ActivityId = game.Id;
        newPointActivity.ActivityName = nameof(game);
        newPointActivity.Point = (int) Math.Round(game.NCard * (game.PctCorrect ?? 0)) * 10;
        newPointActivity.Account = account;
        _context.PointActivity.Add(newPointActivity);
        _context.SaveChanges();
        return newPointActivity;
    }
}