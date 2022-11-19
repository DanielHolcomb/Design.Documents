using Microsoft.AspNetCore.Mvc;

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
