using Stuart_Hopwood_Photography_API.Entities;
using System;

namespace Stuart_Hopwood_Photography_API.Helpers
{
   public class PhotoUtilities : IPhotoUtilities
   {
      public ImageDimensions CalculateImageDimensions(GooglePhotosMediaItem image, int maxWidth, int maxHeight)
      {
         var imageWidth = Convert.ToInt32(image.MediaMetadata.Width);
         var imageHeight = Convert.ToInt32(image.MediaMetadata.Height);

         var ratioX = (double)maxWidth / imageWidth;
         var ratioY = (double)maxHeight / imageHeight;
         var ratio = Math.Min(ratioX, ratioY);

         var imageDimensions = new ImageDimensions
         {
            Height = (int)(imageHeight * ratio),
            Width = (int)(imageWidth * ratio)
         };
         return imageDimensions;
      }
   }
}
