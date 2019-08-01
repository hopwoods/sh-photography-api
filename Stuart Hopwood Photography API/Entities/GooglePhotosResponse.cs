using System.Collections.Generic;

namespace Stuart_Hopwood_Photography_API.Entities
{
    public class GooglePhotosResponse
    {
        public List<GooglePhotosMediaItem> MediaItems { get; set; }

        public string NextPageToken { get; set; }
    }
}
