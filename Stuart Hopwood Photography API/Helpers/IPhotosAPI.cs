using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stuart_Hopwood_Photography_API.Entities;

namespace Stuart_Hopwood_Photography_API.Helpers
{
   public interface IPhotosApi
   {
      Task<GalleryPhotos> GetAlbumPhotos(string albumId, string tokenType, string accessToken);
   }
}
