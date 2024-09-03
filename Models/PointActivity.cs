using System.ComponentModel.DataAnnotations.Schema;
using Main.Models;

public class PointActivity : ModelBase
{
    public int Point { get; set; }
    public string ActivityName { get; set; } = "";
    public string ActivityId { get; set; } = "";

    [ForeignKey("AccountId")]
    public virtual Account? Account { get; set; }
}