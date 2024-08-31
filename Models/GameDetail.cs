using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Models;

public class GameDetail : ModelBase {
    public bool IsCorrect { get; set; } = false;
    public bool IsAnswered { get; set; } = false;
    public int IndexNumber { get; set; }
    public string CategoryName { get; set; } = "";
    public string ClueTxt { get; set; } = "";
    public string ClueImg { get; set; } = "";
    public string DescriptionTxt { get; set; } = "";
    public string CardId { get; set; } = "";
    [ForeignKey("GameId")]
    public virtual Game? Game { get; set; }
    public void Update(bool isCorrect)
    {
        IsCorrect = isCorrect;
        IsAnswered = true;
        LastUpdatedTime = DateTime.UtcNow;
    }
}