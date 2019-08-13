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
      private IDictionary<string,ImageDimensions> ScreenResolutions { get; set; }

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

            ScreenResolutions = new Dictionary<string, ImageDimensions>
            {
               {"1920x1080", new ImageDimensions {Width = 1920, Height = 1080}},
               {"1680x1050", new ImageDimensions {Width = 1680, Height = 1050}},
               {"1600x900", new ImageDimensions {Width = 1600, Height = 900}},
               {"1440x900", new ImageDimensions {Width = 1400, Height = 900}},
               {"1366x768", new ImageDimensions {Width = 1366, Height = 768}},
               {"1360x768", new ImageDimensions {Width = 1360, Height = 768}},
               {"1280x800", new ImageDimensions {Width = 1280, Height = 800}},
               {"1024x768", new ImageDimensions {Width = 1024, Height = 768}},
               {"768x1024", new ImageDimensions {Width = 768, Height = 1024}},
               {"720x1280", new ImageDimensions {Width = 720, Height = 1280}},
               {"480x800", new ImageDimensions {Width = 480, Height = 800}},
               {"360x640", new ImageDimensions {Width = 360, Height = 640}},
               {"320x568", new ImageDimensions {Width = 320, Height = 568}}
            };

            _logger.LogInformation($"Looping through photos and extracting required data.");
            foreach (var item in photosObject.MediaItems)
            {
               var srcSet = new List<string>();
               var imageDimensions = _photoUtilities.CalculateImageDimensions(item, viewportWidth, viewportHeight);

               foreach (var (key, value) in ScreenResolutions)
               {
                  var srcSetDimensions = _photoUtilities.CalculateImageDimensions(item, value.Width, value.Height);
                  srcSet.Add($"{item.BaseUrl}=w{srcSetDimensions.Width}-h{srcSetDimensions.Height} {value.Width}w");
               }
               


               galleryPhotos.Photos.Add(new Photo()
               {
                  Src = $"{item.BaseUrl}=w{imageDimensions.Width}-h{imageDimensions.Height}",
                  SrcSet = srcSet.ToArray(),
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
