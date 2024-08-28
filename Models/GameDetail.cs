using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Models;

public class GameDetail : ModelBase {
    public bool IsCorrect { get; set; } = false;
    public bool IsAnswered { get; set; } = false;
    public int IndexNumber { get; set; }
    [ForeignKey("GameId")]
    public virtual Game? Game { get; set; }
    [ForeignKey("CardVersionId")]
    public virtual CardVersion? CardVersion { get; set; }
    public void Update(bool isCorrect)
    {
        IsCorrect = isCorrect;
        IsAnswered = true;
        LastUpdatedTime = DateTime.UtcNow;
    }
}