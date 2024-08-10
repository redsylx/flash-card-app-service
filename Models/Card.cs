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

    public void Update(string clueTxt, string descriptionTxt, string? clueImg, string? descriptionImg)
    {
        ClueTxt = clueTxt;
        DescriptionTxt = descriptionTxt;
        ClueImg = clueImg;
        DescriptionImg = descriptionImg;
    }
}