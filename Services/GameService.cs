using System.Linq;
using Main.Consts;
using Main.Exceptions;
using Main.Models;
using Main.Utils;

namespace Main.Services;

public class GameService : ServiceBase {
    public GameService(Context context) : base(context)
    {
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

    public Game Finish(string gameId) {
        if(string.IsNullOrEmpty(gameId)) throw new BadRequestException("gameId is missing");
        var existingGame = _context.Game.FirstOrDefault(p => p.Id == gameId)
            ?? throw new BadRequestException($"Card with id {gameId} is not found");
        if(existingGame.Status == GameConst.FINISH) return existingGame;
        existingGame.Finish();
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