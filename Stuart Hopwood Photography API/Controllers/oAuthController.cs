using Microsoft.AspNetCore.Mvc;
using Stuart_Hopwood_Photography_API.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Stuart_Hopwood_Photography_API.Controllers
{
   [Route("api/auth")]
   [ApiController]
   public class OAuthController : ControllerBase
   {
      private readonly IOAuthService _oAuthService;
      private readonly ILogger<OAuthController> _logger;

      public OAuthController(IOAuthService oAuthService, ILogger<OAuthController> logger)
      {
         _oAuthService = oAuthService;
         _logger = logger;
      }


      [HttpGet("GetAuthorization")]
      public IActionResult GetAuthorization(string returnUrl)
      {
         _logger.LogInformation($"Get oAuth authorisation and tokens.");
         return _oAuthService.SendAuthRequest(returnUrl);
      }

      [HttpGet("callback")]
      public async Task<IActionResult> CallbackAsync(string state, string code)
      {
         _logger.LogInformation($"Process oAuth authorisation response and generate tokens.");
         await _oAuthService.ExchangeAuthCodeForAuthToken(code);
         return new RedirectResult(state);
      }
   }
}