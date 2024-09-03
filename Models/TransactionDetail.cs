using System.ComponentModel.DataAnnotations.Schema;
using Main.Models;

public class TransactionDetail : ModelBase
{
    [ForeignKey("SellCardCategoryId")]
    public virtual SellCardCategory? SellCardCategory { get; set; }

    [ForeignKey("TransactionActivityId")]
    public virtual TransactionActivity? TransactionActivity { get; set; }
}
