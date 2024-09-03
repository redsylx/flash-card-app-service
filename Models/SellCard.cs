using System.ComponentModel.DataAnnotations.Schema;
using Main.Models;

public class SellCard : ModelBase
{
    [ForeignKey("SellCardCategoryId")]
    public virtual SellCardCategory? SellCardCategory { get; set; }
    public string ClueTxt { get; set; } = "";
    public string ClueImg { get; set; } = "";
    public string DescriptionTxt { get; set; } = "";
}
