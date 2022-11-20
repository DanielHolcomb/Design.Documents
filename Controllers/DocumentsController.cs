using Microsoft.AspNetCore.Mvc;
using Google.Apis.Docs.v1;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Analytics.v3;
using Google.Apis.Drive.v2;

namespace Design.Documents.Controllers
{
    [Route("Design/[Controller]")]
    public class DocumentsController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(IConfiguration config, ILogger<DocumentsController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(_config.GetSection("GoogleApiEmail").Value)
                {
                    Scopes = new[] {
                    DocsService.Scope.Documents,
                    DriveService.Scope.Drive
                }
                }.FromPrivateKey(_config.GetSection("GoogleApiPrivateKey").Value)); ;

                var docsService = new DocsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "design-documents-goog"

                });

                var driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "design-documents-goog"

                });

                var filesRequest = driveService.Files.List();
                var files = filesRequest.Execute();
                var request = docsService.Documents.Get("1Emy2vaxH6Blnw0BL-cr5EcsU8AKIM4GvOWJTn7SrKDg");
                return Ok(files);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
