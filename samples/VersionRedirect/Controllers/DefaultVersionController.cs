using Microsoft.AspNetCore.Mvc;

namespace VersionRedirect.Controllers
{
    [Route("api/version")]
    public class DefaultVersionController : Controller
    {
        [HttpGet]
        public IActionResult GetVersion()
        {
            return Ok(new { version = "default version" });
        }
    }
}
