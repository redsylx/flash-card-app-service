using System;

namespace Main.Exceptions;

public class BadRequestException : Exception {
    public BadRequestException(string message) : base(message)
    {
    }
}