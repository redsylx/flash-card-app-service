using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Main.Exceptions;

public class InternalServerErrorException : ObjectResult {
    public InternalServerErrorException(string message) : base(message)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}