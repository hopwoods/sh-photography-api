using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Stuart_Hopwood_Photography_API.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Stuart_Hopwood_Photography_API.Helpers
{
   public class GooglePhotosApi : IPhotosApi
   {
      private readonly HttpClient _client = new HttpClient();
      private IConfiguration Configuration { get; }

      public GooglePhotosApi(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public async Task<GalleryPhotos> GetAlbumPhotos(string albumId, string tokenType, string accessToken)
      {
         var galleryPhotos = new GalleryPhotos { Photos = new List<Photo>() };
         var requestData = new Dictionary<string, string>
         {
            {"albumId", albumId},
            {"pageSize", "100"} // Maximum page size
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
         request.Headers.Add("Authorization", $"{tokenType} {accessToken}");
         request.Content = content;

         using (var response = await _client.SendAsync(request))
         {
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<GooglePhotosResponse>(responseJson);

            if (responseObject == null) return galleryPhotos;

            if (responseObject.MediaItems == null || responseObject.MediaItems.Count <= 0)
               return galleryPhotos;

            foreach (var item in responseObject.MediaItems)
            {
               var width = Convert.ToInt32(item.MediaMetadata.Width);
               var height = Convert.ToInt32(item.MediaMetadata.Height);
               var scale = 3;

               galleryPhotos.Photos.Add(new Photo()
               {
                  Src = $"{item.BaseUrl}=w{height/scale}-h{height/scale}",
                  Width = width,
                  Height = height
               }
               );
            }
         }

         return galleryPhotos;
      }
   }
}
