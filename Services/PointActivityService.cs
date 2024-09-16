using System;
using System.Collections.Generic;
using System.Linq;
using Main.Consts;
using Main.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Main.Services;

public class PointActivityService : ServiceBase {
    public PointActivityService(Context context): base(context)
    {
    }

    public void FinishTransaction(string transactionId) {
        var transactionBuyer = _context.TransactionActivity.Include(p => p.Account).First(p => p.Id == transactionId);
        var transactions = _context.TransactionDetail.Include(p => p.TransactionActivitySeller).ThenInclude(p => p.Account).Where(p => p.TransactionActivityBuyer != null && p.TransactionActivityBuyer.Id == transactionId).Select(p => p.TransactionActivitySeller).ToList();
        transactions.Add(transactionBuyer);
        var pointActivities = new List<PointActivity>();
        foreach(var transaction in transactions) {
            var pointActivity = new PointActivity();
            pointActivity.Account = transaction.Account;
            pointActivity.ActivityId = transaction.Id;
            pointActivity.ActivityName = nameof(transaction);
            pointActivity.Point = transaction.TotalPoint;
            pointActivities.Add(pointActivity);
        }
        _context.PointActivity.AddRange(pointActivities);
        _context.SaveChanges();
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