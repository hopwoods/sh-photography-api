using Stuart_Hopwood_Photography_API.Entities;
using System.Threading.Tasks;

namespace Stuart_Hopwood_Photography_API.Helpers
{
   public interface IPhotosApi
   {
      Task<GalleryPhotos> GetAlbumPhotos(string albumId, int viewportWidth, int viewportHeight, string tokenType, string accessToken);
   }
}
