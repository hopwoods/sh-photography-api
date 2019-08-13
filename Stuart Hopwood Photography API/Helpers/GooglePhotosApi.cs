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
      private readonly IPhotoUtilities _photoUtilities;
      private readonly ILogger<GooglePhotosApi> _logger;
      private readonly ClientInfo _clientInfo;

      public GooglePhotosApi(ILogger<GooglePhotosApi> logger, ClientInfo clientInfo, IPhotoUtilities photoUtilities)
      {
         _logger = logger;
         _clientInfo = clientInfo;
         _photoUtilities = photoUtilities;
      }

      public async Task<GalleryPhotos> GetAlbumPhotos(string albumId, int viewportWidth, int viewportHeight, string tokenType, string accessToken)
      {
         if (albumId == null) throw new ArgumentNullException(nameof(albumId));
         if (viewportWidth <= 0) throw new ArgumentOutOfRangeException(nameof(viewportWidth));
         if (viewportHeight <= 0) throw new ArgumentOutOfRangeException(nameof(viewportHeight));

         _logger.LogInformation($"Get Photos JSON from Goolge album id {albumId}.");
            var galleryPhotos = new GalleryPhotos
            {
                Photos = new List<Photo>()
            };

            // Todo - Use Flurl instead of Http Cient
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
               var srcSet = new List<string>();
               var imageDimensions = _photoUtilities.CalculateImageDimensions(item, viewportWidth, viewportHeight);

               foreach (var (key, value) in ScreenResolutions.Resolutions)
               {
                  var srcSetDimensions = _photoUtilities.CalculateImageDimensions(item, value.Width, value.Height);
                  srcSet.Add($"{item.BaseUrl}=w{srcSetDimensions.Width}-h{srcSetDimensions.Height} {value.Width}w");
               }

               galleryPhotos.Photos.Add(new Photo()
               {
                  Src = $"{item.BaseUrl}=w{imageDimensions.Width}-h{imageDimensions.Height}",
                  SrcSet = srcSet.ToArray(),
                  Sizes = new[]{"(min-width: 480px) 50vw,(min-width: 1024px) 33.3vw,100vw"},
                  Width = imageDimensions.Width,
                  Height = imageDimensions.Height,
                  Title = item.Filename
               }
               );
            }

            _logger.LogInformation($"Photos retrieved {galleryPhotos}.");
         }
         return galleryPhotos;
      }
   }
}
