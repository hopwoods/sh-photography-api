using NUnit.Framework;
using Stuart_Hopwood_Photography_API.Entities;
using Stuart_Hopwood_Photography_API.Helpers;

namespace Api.Tests
{
   public class Tests
   {
      private PhotoUtilities PhotoUtilities { get; set; }


      [SetUp]
      public void Setup()
      {
         PhotoUtilities = new PhotoUtilities();
      }

      [TestCase(2000, 2000, 1000, 1000, 1000, 1000)]
      [TestCase(250, 250, 300, 300, 300, 300)]
      [TestCase(2000, 2000, 0, 0, 2000, 2000)]
      public void CalculateImageDimensions_Returns_CorrectDimensions(int photoWidth, int photoHeight, int maxWidth, int maxHeight, int expectedWidth, int expectedHeight)
      {
         // Arrange
         var photo = new GooglePhotosMediaItem
         {
            MediaMetadata = new GooglePhotosMediaMetaData
            {
               Width = photoWidth.ToString(), Height = photoHeight.ToString()
            }
         };

         // Act

         var result = PhotoUtilities.CalculateImageDimensions(photo, maxWidth, maxHeight);

         // Assert
         Assert.That(result.Height, Is.EqualTo(expectedHeight), $"Calculated Height of {photoHeight} does not equal exptected height of {expectedHeight}");
         Assert.That(result.Width, Is.EqualTo(expectedWidth), $"Calculated Height of {photoWidth} does not equal exptected height of {expectedWidth}");
      }
   }
}