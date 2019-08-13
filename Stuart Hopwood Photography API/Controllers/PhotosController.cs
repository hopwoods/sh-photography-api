using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stuart_Hopwood_Photography_API.Helpers;
using Stuart_Hopwood_Photography_API.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stuart_Hopwood_Photography_API.Entities;

namespace Stuart_Hopwood_Photography_API.Controllers
{
   [Route("api/photos")]
   [ApiController]
   public class PhotosController : ControllerBase
   {
      private readonly IOAuthService _oAuthService;
      private readonly IPhotosApi _photosApi;
      private readonly ILogger<PhotosController> _logger;
      private readonly ClientInfo _clientInfo;
      private IConfiguration Configuration { get; set; }

      public PhotosController(IOAuthService oAuthService, IPhotosApi photosApi, IConfiguration configuration, ILogger<PhotosController> logger, ClientInfo clientInfo)
      {
         _oAuthService = oAuthService;
         _photosApi = photosApi;
         Configuration = configuration;
         _logger = logger;
         _clientInfo = clientInfo;
      }

      [AllowAnonymous]
      [HttpGet("GetAlbumPhotos")]
      public async Task<IActionResult> GetAlbumPhotosAsync(string albumId, int viewportWidth, int viewportHeight)
      {
         _logger.LogInformation("Getting Album Photos");

         _logger.LogInformation($"Username = {_clientInfo.UserName}");
         _logger.LogInformation($"Client ID = {_clientInfo.ClientId}");
         _logger.LogInformation($"Client Secret = {_clientInfo.ClientSecret}");
         _logger.LogInformation($"Redirect URL = {_clientInfo.RedirectUri}");

         if (albumId == null)
         {
            _logger.LogError("Parameter AlbumID cannot be null");
            return BadRequest(new { message = "You must provide an album id." });
         }

         if (viewportWidth <= 0)
         {
            _logger.LogError("Parameter viewportWidth cannot be <= 0");
            return BadRequest(new {message = "You must provide an viewportWidth greater than 0."});
         }

         if (viewportHeight <= 0)
         {
            _logger.LogError("Parameter viewportHeight cannot be <= 0");
            return BadRequest(new {message = "You must provide an viewportHeight greater than 0."});
         }

         // Get Auth Token
         var returnUrl = HttpContext.Request.GetDisplayUrl();

         _logger.LogInformation("Get AuthToken for us in controller");

         var authToken = await _oAuthService.GetAuthTokenAsync(Configuration["GoogleApi:username"]);

         if (authToken == null)
         {
            _logger.LogInformation("AuthToken is null");
            // Get new authorisation and token
            return RedirectToAction("GetAuthorization", "OAuth", new {returnUrl});
         }

         _logger.LogInformation("AuthToken retreived");
            
         // Get Photos
         try
         {
            _logger.LogInformation("Retrieve Photos JSON from Google API");
            var galleryPhotos = await _photosApi.GetAlbumPhotos(albumId, viewportWidth, viewportHeight, authToken.token_type, authToken.access_token);
            _logger.LogInformation("Photos JSON Retrieved");
            return new JsonResult(galleryPhotos);
         }
         catch (Exception ex)
         {
           _logger.LogError(ex.Message);
            return new JsonResult(ex.Message);
         }
      }

      [HttpGet("TestRedirect")]
      public IActionResult TestRedirect(string albumId)
      {
         _logger.LogInformation("Redirect to BBC");
         return new RedirectResult("https://www.bbc.co.uk");
      }
   }
}