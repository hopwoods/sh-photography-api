using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Stuart_Hopwood_Photography_API.Data;
using Stuart_Hopwood_Photography_API.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Stuart_Hopwood_Photography_API.Controllers
{
   [Route("api/photos")]
   [ApiController]
   public class PhotosController : ControllerBase
   {
      private readonly ApplicationContext _context;

      public PhotosController(IConfiguration configuration, ApplicationContext context)
      {
         Configuration = configuration;
         _context = context;
      }

      private IConfiguration Configuration { get; set; }
      private static readonly HttpClient Client = new HttpClient();

      [AllowAnonymous]
      [HttpGet("hello")]
      public IActionResult Hello()
      {

         return Content("hello world");
      }

      [AllowAnonymous]
      [HttpGet("GetAlbumPhotos")]
      public async Task<IActionResult> GetAlbumPhotos(string albumId)
      {
         if (albumId == null)
         {
            return BadRequest(new {message = "You must provide an album id."});
         }
            

         UserCredential credential;
         string[] scopes =
         {
            "https://www.googleapis.com/auth/photoslibrary.readonly"
         };

         const string clientApiSecret =
            "{\"installed\":{\"client_id\":\"908335520577-6avd0rlkkico8eoe4t2kasp7r8ede9bq.apps.googleusercontent.com\",\"project_id\":\"sh-photography-1560067890037\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"6Q2ODMCe0deI45yJ7UPMcBNg\",\"redirect_uris\":[\"urn:ietf:wg:oauth:2.0:oob\",\"http://localhost\"]}}";
         // convert string to stream
         var byteArray = Encoding.ASCII.GetBytes(clientApiSecret);
         var stream = new MemoryStream(byteArray); 
         
         
         var userName = Configuration["GoogleAPI:username"];
         var clientId = Configuration["GoogleAPI:client_id"];
         var clientSecret = Configuration["GoogleAPI:client_secret"];
         var galleryPhotos = new GalleryPhotos
         {
            Photos = new List<Photo>()
         };

         credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.Load(stream).Secrets,
               scopes,
               userName,
               CancellationToken.None,
               new DbDataStore(_context)
         ).Result;
         

         try
         {
            var requestData = new Dictionary<string, string>
            {
               {"albumId", albumId}
            };

            var content = new FormUrlEncodedContent(requestData);
            var request = new HttpRequestMessage()
            {
               RequestUri = new Uri("https://photoslibrary.googleapis.com/v1/mediaItems:search"),
               Method = HttpMethod.Post
            };

            request.Headers.Add("ContentType", "application/json");
            request.Headers.Add("client_id", clientId);
            request.Headers.Add("client_secret", clientSecret);
            request.Headers.Add("Authorization", $"{credential.Token.TokenType} {credential.Token.AccessToken}");
            request.Content = content;

            using (var response = await Client.SendAsync(request))
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