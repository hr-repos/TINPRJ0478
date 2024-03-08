using Microsoft.AspNetCore.Mvc;
using testOpcUA.Controllers.mainController;

namespace testOpcUA.Controllers;

[ApiController]
[Route("api")]
public class Controller : ControllerBase
{
    private readonly ILogger<Controller> _logger;

    public Controller(ILogger<Controller> logger)
    {
        _logger = logger;
    }

    [HttpPost("testPost")]
    public ActionResult<ResponseBody> Get(RequestBody request)
    {
        string? foo = request.Foo;

        if (foo == null)
            return NotFound();

        return Ok(new ResponseBody()
        {
            TestNumber = 200
        });
    }
}
