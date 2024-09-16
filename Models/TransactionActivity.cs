using System.ComponentModel.DataAnnotations.Schema;
using Main.Models;

public class TransactionActivity : ModelBase
{
    public int TotalPoint { get; set; }
    public int TotalItem { get; set; }
    public string Category { get; set; } = "";

    [ForeignKey("AccountId")]
    public virtual Account? Account { get; set; }
    public TransactionActivity()
    {
    }
    public TransactionActivity(Account account)
    {
        Account = account;
    }

    public TransactionActivity(Account account, string category)
    {
        Account = account;
        Category = category;
    }

    public TransactionActivity(Account account, string category, int totalPoint)
    {
        Account = account;
        Category = category;
        TotalPoint = totalPoint;
    }
}