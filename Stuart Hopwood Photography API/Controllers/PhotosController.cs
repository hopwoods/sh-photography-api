using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Stuart_Hopwood_Photography_API.Entities;
using Stuart_Hopwood_Photography_API.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stuart_Hopwood_Photography_API.Controllers
{
   [Route("api/photos")]
   [ApiController]
   public class PhotosController : ControllerBase
   {
      private readonly HttpClient _client = new HttpClient();
      private readonly IOAuthHelper _oAuthHelper;

      public PhotosController(IConfiguration configuration, IOAuthHelper oAuthHelper)
      {
         Configuration = configuration;
         _oAuthHelper = oAuthHelper;
      }

      private IConfiguration Configuration { get; }

      [AllowAnonymous]
      [HttpGet("GetAlbumPhotos")]
      public async Task<IActionResult> GetAlbumPhotos(string albumId)
      {
         if (albumId == null)
         {
            return BadRequest(new {message = "You must provide an album id."});
         }

         var galleryPhotos = new GalleryPhotos
         {
            Photos = new List<Photo>()
         };

         // Get Authorisation Token from Google
         // Get previously stored one if present, or get and store new one if not.
         var credentials = _oAuthHelper.GetUserCredentials();

         // Check if the token has expired and request a new one using the refresh token
         if (credentials.Token.IsExpired(credentials.Flow.Clock))
         {
            var success = await _oAuthHelper.RefreshToken(credentials);
            if (success)
            {
               // Update credentials from store
               credentials = _oAuthHelper.GetUserCredentials();
            }
         }

         // Get The Photos
         try
         {
            var requestData = new Dictionary<string, string>
            {
               {"albumId", albumId},
               {"pageSize", "100"}
            };

            var content = new FormUrlEncodedContent(requestData);
            var request = new HttpRequestMessage()
            {
               RequestUri = new Uri("https://photoslibrary.googleapis.com/v1/mediaItems:search"),
               Method = HttpMethod.Post
            };

            request.Headers.Add("ContentType", "application/json");
            request.Headers.Add("client_id", Configuration["GoogleAPI:client_id"]);
            request.Headers.Add("client_secret", Configuration["GoogleAPI:client_secret"]);
            request.Headers.Add("Authorization", $"{credentials.Token.TokenType} {credentials.Token.AccessToken}");
            request.Content = content;

            using (var response = await _client.SendAsync(request))
            {
               var responseJson = await response.Content.ReadAsStringAsync();
               var responseObject = JsonConvert.DeserializeObject<GooglePhotosResponse>(responseJson);

               if (responseObject == null) return new JsonResult(null);

               if (responseObject.MediaItems == null || responseObject.MediaItems.Count <= 0)
                  return new JsonResult(null);

               foreach (var item in responseObject.MediaItems)
               {
                  galleryPhotos.Photos.Add(new Photo()
                     {
                        Src = item.ProductUrl,
                        Width = item.MediaMetadata.Width,
                        Height = item.MediaMetadata.Height
                     }
                  );
               }
            }
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