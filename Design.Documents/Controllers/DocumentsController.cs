using Microsoft.AspNetCore.Mvc;
using Google.Apis.Docs.v1;

namespace Design.Documents.Controllers
{
    [Route("Design/[Controller]")]
    public class DocumentsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Documents");
        }
    }
}
