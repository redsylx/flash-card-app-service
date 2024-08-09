using Microsoft.Extensions.Logging;

namespace Main.Controllers;

public class ControllerBase<T> : ControllerCore {
    protected readonly Context _context;
    protected readonly ILogger<T> _logger;
    public ControllerBase(Context context, ILogger<T> logger)
    {
        _context = context;
        _logger = logger;
    }
}