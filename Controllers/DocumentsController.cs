using Microsoft.AspNetCore.Mvc;
using Google.Apis.Docs.v1;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Analytics.v3;
using Google.Apis.Drive.v2;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

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
        [Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(_config.GetSection("GoogleApi:Email").Value)
                {
                    Scopes = new[] {
                    DocsService.Scope.Documents,
                    DriveService.Scope.Drive
                }
                }.FromPrivateKey(_config.GetSection("GoogleApi:PrivateKey").Value));

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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("Token")]
        public async Task<IActionResult> Token()
        {
            return Ok(GenerateJwtToken());
        }

        private string GenerateJwtToken()
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("Jwt:Key").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", "design") }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config.GetSection("Jwt:Issuer").Value,
                Audience = _config.GetSection("Jwt:Audience").Value,
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
