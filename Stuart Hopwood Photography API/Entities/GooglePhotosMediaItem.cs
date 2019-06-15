namespace Stuart_Hopwood_Photography_API.Entities
{
    public class GooglePhotosMediaItem
    {
        public string Id { get; set; }
        public string ProductUrl { get; set; }
        public string BaseUrl { get; set; }
        public string MimeType { get; set; }
        public GooglePhotosMediaMetaData MediaMetadata { get; set; }
        public string Filename { get; set; }
    }
}
