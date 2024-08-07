namespace Main.Controllers;

public class ControllerBase {
    protected readonly Context _context;
    public ControllerBase(Context context)
    {
        _context = context;
    }
}