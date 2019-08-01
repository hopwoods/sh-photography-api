using Stuart_Hopwood_Photography_API.Entities;

namespace Stuart_Hopwood_Photography_API.Helpers
{
    public interface IPhotoUtilities
    {
        ImageDimensions CalculateImageDimensions(GooglePhotosMediaItem image, int maxWidth, int maxHeight);
    }
}
