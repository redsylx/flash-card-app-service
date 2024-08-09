using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Main.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ControllerCore {
}