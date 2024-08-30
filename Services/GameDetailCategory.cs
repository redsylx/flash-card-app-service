using System.Collections.Generic;
using System.Linq;
using Main.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Services;

public class GameDetailCategoryService : ServiceBase {
    public GameDetailCategoryService(Context context) : base(context)
    {
    }

    public void Create(List<string> cardCategoryIds, string gameId) {
        var categories = _context.CardCategory.Where(p => cardCategoryIds.Contains(p.Id)).ToList();
        var game = _context.Game.FirstOrDefault(p => p.Id == gameId);
        var detailCategories = new List<GameDetailCategory>();
        foreach(var category in categories) {
            var detailCategory = new GameDetailCategory();
            detailCategory.Game = game;
            detailCategory.CardCategory = category;
            detailCategories.Add(detailCategory);
        }
        _context.GameDetailCategory.AddRange(detailCategories);
        _context.SaveChanges();
    }

    public void DeleteAll(List<string> gameIds) {
        _context.GameDetailCategory.Where(p => p.Game != null && gameIds.Contains(p.Game.Id)).ExecuteDelete();
        _context.SaveChanges();
    }
}