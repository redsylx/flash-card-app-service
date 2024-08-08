using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Main.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ControllerBase<T> {
    protected readonly Context _context;
    protected readonly ILogger<T> _logger;
    public ControllerBase(Context context, ILogger<T> logger)
    {
        _context = context;
        _logger = logger;
    }
}