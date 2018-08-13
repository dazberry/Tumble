using Microsoft.AspNetCore.Mvc;

namespace VersionRedirect.Controllers
{
    [Route("api/v2/version")]
    public class Version2Controller : Controller
    {
        [HttpGet]
        public IActionResult GetVersion()
        {
            return Ok(new { version = 2.0 });
        }
    }
}