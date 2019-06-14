using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stuart_Hopwood_Photography_API.Helpers;
using Stuart_Hopwood_Photography_API.Services;
using System;
using System.Threading.Tasks;

namespace Stuart_Hopwood_Photography_API.Controllers
{
   [Route("api/photos")]
   [ApiController]
   public class PhotosController : ControllerBase
   {
      private readonly IOAuthService _oAuthService;
      private readonly IPhotosApi _photosApi;
      private IConfiguration Configuration { get; set; }

      public PhotosController(IOAuthService oAuthService, IPhotosApi photosApi, IConfiguration configuration)
      {
         _oAuthService = oAuthService;
         _photosApi = photosApi;
         Configuration = configuration;
      }

      [AllowAnonymous]
      [HttpGet("GetAlbumPhotos")]
      public async Task<IActionResult> GetAlbumPhotosAsync(string albumId)
      {
         if (albumId == null)
         {
            return BadRequest(new { message = "You must provide an album id." });
         }
         // Get Auth Token
         var returnUrl = HttpContext.Request.GetDisplayUrl();

         var authToken = _oAuthService.GetAuthToken(Configuration["GoogleApi:username"]);

         if (authToken == null)
            // Get new authorisation and token
            return RedirectToAction("GetAuthorization", "OAuth", new { returnUrl });

         // Get Photos
         try
         {
            var galleryPhotos = await _photosApi.GetAlbumPhotos(albumId, authToken.token_type, authToken.access_token);
            return new JsonResult(galleryPhotos);
         }
         catch (Exception ex)
         {
            //Console.WriteLine("Error occured: " + ex.Message);
            return new JsonResult(ex.Message);
         }
      }
   }
}