namespace Main.Services;

public class ServiceBase {
    protected readonly Context _context;
    public ServiceBase(Context context)
    {
        _context = context;
    }
}