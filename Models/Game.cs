using System;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Consts;

namespace Main.Models;

public class Game : ModelBase {
    public string Status { get; set; } = GameConst.PLAYING;
    public int NCard { get; set; }
    public decimal? PctCorrect { get; set; }
    public int HideDurationInSecond { get; set; } = 5;
    [ForeignKey("AccountId")]
    public virtual Account? Account { get; set; }
    public void Finish() {
        Status = GameConst.FINISH;
        LastUpdatedTime = DateTime.UtcNow;
    }
}