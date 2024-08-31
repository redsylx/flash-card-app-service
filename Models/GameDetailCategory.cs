using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Models;

public class GameDetailCategory : ModelBase {
    [ForeignKey("GameId")]
    public virtual Game? Game { get; set; }
    public string Name { get; set; } = "";
}