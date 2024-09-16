using System.ComponentModel.DataAnnotations.Schema;
using Main.Models;

public class TransactionDetail : ModelBase
{
    [ForeignKey("SellCardCategoryId")]
    public virtual SellCardCategory? SellCardCategory { get; set; }

    [ForeignKey("TransactionActivityIdSeller")]
    public virtual TransactionActivity? TransactionActivitySeller { get; set; }
    
    [ForeignKey("TransactionActivityIdBuyer")]
    public virtual TransactionActivity? TransactionActivityBuyer { get; set; }
}
