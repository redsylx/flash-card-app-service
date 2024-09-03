using System.ComponentModel.DataAnnotations.Schema;
using Main.Models;

public class TransactionActivity : ModelBase
{
    public int TotalPoint { get; set; }
    public string Category { get; set; } = "";

    [ForeignKey("AccountId")]
    public virtual Account? Account { get; set; }
}