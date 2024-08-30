using System;
using System.Collections.Generic;
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
    public virtual List<GameDetailCategory>? Categories { get; set; }
    public virtual List<GameDetail>? Details { get; set; }
    public void Finish(int correct) {
        Status = GameConst.FINISH;
        PctCorrect = Math.Round((decimal) correct / NCard, 2);
        LastUpdatedTime = DateTime.UtcNow;
    }
}