using Newtonsoft.Json;
using Stuart_Hopwood_Photography_API.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Stuart_Hopwood_Photography_API.Helpers
{
   public class GooglePhotosApi : IPhotosApi
   {
      private readonly HttpClient _client = new HttpClient();
      private readonly ILogger<GooglePhotosApi> _logger;
      private readonly ClientInfo _clientInfo;

      public GooglePhotosApi(ILogger<GooglePhotosApi> logger, ClientInfo clientInfo)
      {
         _logger = logger;
         _clientInfo = clientInfo;
      }

      public async Task<GalleryPhotos> GetAlbumPhotos(string albumId, string tokenType, string accessToken)
      {
         _logger.LogInformation($"Get Photos JSON from Goolge album id {albumId}.");
         var galleryPhotos = new GalleryPhotos { Photos = new List<Photo>() };
         var requestData = new Dictionary<string, string>
         {
            {"albumId", albumId},
            {"pageSize", "100"} // Maximum page size
         };

         _logger.LogInformation($"Request Data for API Request {requestData}.");

         var content = new FormUrlEncodedContent(requestData);
         var request = new HttpRequestMessage()
         {
            RequestUri = new Uri("https://photoslibrary.googleapis.com/v1/mediaItems:search"),
            Method = HttpMethod.Post
         };

         request.Headers.Add("ContentType", "application/json");
         request.Headers.Add("client_id", _clientInfo.ClientId);
         request.Headers.Add("client_secret", _clientInfo.ClientSecret);
         request.Headers.Add("Authorization", $"{tokenType} {accessToken}");
         request.Content = content;

         using (var response = await _client.SendAsync(request))
         {
            _logger.LogInformation($"Response from API {response.StatusCode} : {response.ReasonPhrase}.");
            var responseJson = await response.Content.ReadAsStringAsync();
            var photosObject = JsonConvert.DeserializeObject<GooglePhotosResponse>(responseJson);

            _logger.LogInformation($"JSON object from API response from db {photosObject}.");

            if (photosObject == null) return galleryPhotos;

            if (photosObject.MediaItems == null || photosObject.MediaItems.Count <= 0)
            {
               _logger.LogInformation($"No Photos returned by API.");
               return galleryPhotos;
            }

            _logger.LogInformation($"Looping through photos and extracting required data.");
            foreach (var item in photosObject.MediaItems)
            {
               var width = Convert.ToInt32(item.MediaMetadata.Width);
               var height = Convert.ToInt32(item.MediaMetadata.Height);
               const int scale = 3;

               galleryPhotos.Photos.Add(new Photo()
               {
                  Src = $"{item.BaseUrl}=w{height / scale}-h{height / scale}",
                  Width = width,
                  Height = height
               }
               );
            }

            _logger.LogInformation($"Photos retrieved {galleryPhotos}.");
         }
         return galleryPhotos;
      }
   }
}
