using System;
using Microsoft.AspNetCore.Mvc;
using Stuart_Hopwood_Photography_API.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
         try
         {
            _logger.LogInformation($"Exchanging token...code = {code} state = {state}");
            await _oAuthService.ExchangeAuthCodeForAuthToken(code);
         }
         catch (Exception ex)
         {
            _logger.LogError($"{ex.Message}, {ex.StackTrace}");
            var result = JsonConvert.SerializeObject(ex);
            return Content(result);
         }
         
         return new RedirectResult(state);
      }
   }
}