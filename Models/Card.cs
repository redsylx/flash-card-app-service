using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Models;

public class Card : ModelBase
{
    public string ClueTxt { get; set; } = "";
    public string? ClueImg { get; set; }
    public string DescriptionTxt { get; set; } = "";
    public string? DescriptionImg { get; set; }
    public int NFrequency { get; set; }
    public int? NCorrect { get; set; }
    public decimal? PctCorrect { get; set; }
    [ForeignKey("CardCategoryId")]
    public virtual CardCategory? CardCategory { get; set; }
}