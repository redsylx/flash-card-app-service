using System.Linq;

namespace Main.Utils;

public class Validation {
    public static bool IsAlphanumeric(string input) {
        if(string.IsNullOrEmpty(input)) return false;
        return input.All(char.IsLetterOrDigit);
    }
}