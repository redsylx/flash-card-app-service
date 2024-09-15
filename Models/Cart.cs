using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Models;

public class Cart : ModelBase {
    public int NItems { get; set; } = 0;
    
    [ForeignKey("AccountId")]
    public virtual Account? Account { get; set; }
}