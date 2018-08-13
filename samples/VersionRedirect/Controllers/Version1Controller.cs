using Microsoft.AspNetCore.Mvc;

namespace VersionRedirect.Controllers
{
    [Route("api/v1/version")]
    public class Version1Controller : Controller
    {        
        [HttpGet]
        public IActionResult GetVersion()
        {
            return Ok(new { version = 1.0 });
        }
    }
}
