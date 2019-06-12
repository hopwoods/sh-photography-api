using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stuart_Hopwood_Photography_API.Helpers;
using System;
using System.Threading.Tasks;

namespace Stuart_Hopwood_Photography_API.Controllers
{
   [Route("api/photos")]
   [ApiController]
   public class PhotosController : ControllerBase
   {
      private readonly IOAuthHelper _oAuthHelper;
      private readonly IPhotosApi _photosApi;

      public PhotosController(IOAuthHelper oAuthHelper, IPhotosApi photosApi)
      {
         _oAuthHelper = oAuthHelper;
         _photosApi = photosApi;
      }

      [AllowAnonymous]
      [HttpGet("GetAlbumPhotos")]
      public async Task<IActionResult> GetAlbumPhotos(string albumId)
      {
         if (albumId == null)
         {
            return BadRequest(new {message = "You must provide an album id."});
         }

         // Get Authorisation
         var credentials = _oAuthHelper.GetUserCredentials();
         if (credentials.Token.IsExpired(credentials.Flow.Clock))
         {
            var success = await _oAuthHelper.RefreshToken(credentials.Token.RefreshToken);
            if (success)
            {
               // Get Updated credentials from store
               credentials = _oAuthHelper.GetUserCredentials();
            }
         }

         // Get Photos
         try
         {
            var galleryPhotos =
               await _photosApi.GetAlbumPhotos(albumId, credentials.Token.TokenType, credentials.Token.AccessToken);
            return new JsonResult(galleryPhotos);
         }
         catch (Exception ex)
         {
            Console.WriteLine("Error occured: " + ex.Message);
            return new JsonResult(ex.Message);
         }
      }
   }
}