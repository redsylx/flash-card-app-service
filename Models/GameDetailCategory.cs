using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Models;

public class GameDetailCategory : ModelBase {
    [ForeignKey("GameId")]
    public virtual Game? Game { get; set; }
    [ForeignKey("CardCategoryId")]
    public virtual CardCategory? CardCategory { get; set; }
}