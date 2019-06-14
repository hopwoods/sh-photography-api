using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stuart_Hopwood_Photography_API.Helpers;
using Stuart_Hopwood_Photography_API.Services;

namespace Stuart_Hopwood_Photography_API.Controllers
{
   [Route("api/auth")]
   [ApiController]
   public class OAuthController : ControllerBase
   {
      private readonly IOAuthService _oAuthService;
      private readonly IPhotosApi _photosApi;
      private IConfiguration Configuration { get; set; }

      public OAuthController(IOAuthService oAuthService, IPhotosApi photosApi, IConfiguration configuration)
      {
         _oAuthService = oAuthService;
         _photosApi = photosApi;
         Configuration = configuration;
      }


      [HttpGet("GetAuthorization")]
      public IActionResult GetAuthorization(string returnUrl)
      {
         return _oAuthService.SendAuthRequest(returnUrl);
      }

      [HttpGet("callback")]
      public async Task<IActionResult> CallbackAsync(string state, string code)
      {
         await _oAuthService.ExchangeAuthCodeForAuthToken(code);
         return new RedirectResult(state);
      }
   }
}