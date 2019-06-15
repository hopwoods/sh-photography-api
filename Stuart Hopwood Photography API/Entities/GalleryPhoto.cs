using System.Collections.Generic;

namespace Stuart_Hopwood_Photography_API.Entities
{
   public class GalleryPhotos
   {
      public List<Photo> Photos { get; set; }
   }

   public class Photo
   {
      public string Src { get; set; }
      public int Width { get; set; }
      public int Height { get; set; }
   }
}
