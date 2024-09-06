using System.ComponentModel.DataAnnotations.Schema;
using Main.Models;

public class SellCardCategory : ModelBase
{
    public string Name { get; set; } = "";
    public bool IsDeleted { get; set; }
    public int NCard { get; set; }
    public string Img { get; set; } = "";
    public int Point { get; set; }
    public int Sold { get; set; }

    public string Description { get; set; } = "";

    [ForeignKey("AccountId")]
    public virtual Account? Account { get; set; }
}
