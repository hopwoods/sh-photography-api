using System;

namespace Stuart_Hopwood_Photography_API.Entities
{
    public class GooglePhotosMediaMetaData
    {
        public DateTime CreationTime { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public GooglePhotosPhoto Photo { get; set; }
    }
}
