using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Models;

public class CartDetail : ModelBase {
    [ForeignKey("CartId")]
    public virtual Cart? Cart { get; set; }

    [ForeignKey("SellCardCategoryId")]
    public virtual SellCardCategory? SellCardCategory { get; set; }
}