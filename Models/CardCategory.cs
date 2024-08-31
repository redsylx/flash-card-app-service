using System.ComponentModel.DataAnnotations.Schema;
using Main.Exceptions;
using Main.Utils;

namespace Main.Models;

public class CardCategory : ModelBase {
    public string Name { get; set; } = "";
    public bool IsDeleted { get; set; } = false;
    public int NCard { get; set; } = 0;
    public decimal? PctCorrect { get; set; }
    
    [ForeignKey("AccountId")]
    public virtual Account? Account { get; set; }

    public void Validate() {
        if(string.IsNullOrEmpty(Name)) throw new BadRequestException("Name Can't be empty");
        if(Name.Length < 3 || Name.Length > 12) throw new BadRequestException("Name must be between 3 and 12 characters");
        Name = Name.ToLower().Replace("_", "");
        if(!Validation.IsAlphanumeric(Name)) throw new BadRequestException("Name can only be alphanumeric and underscore");
    }
}