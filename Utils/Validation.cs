using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Main.Exceptions;

namespace Main.Utils;

public class Validation {
    public static bool IsAlphanumeric(string input) {
        if(string.IsNullOrEmpty(input)) return false;
        return input.All(char.IsLetterOrDigit);
    }
    
    public static void Validate<T>(T obj) {
        if(obj is null) throw new BadRequestException("Validation: Object can't be null");
        var validationContext = new ValidationContext(obj, serviceProvider: null, items: null);
        var validationResult = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
        if(!isValid)
            throw new BadRequestException(string.Join(Environment.NewLine, validationResult.Select(p => p.ErrorMessage)));
    }
}