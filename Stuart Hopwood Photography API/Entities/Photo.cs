namespace Stuart_Hopwood_Photography_API.Entities
{
    public class Photo
    {
        public string Src { get; set; }

        public string[] SrcSet { get; set; }

        public string[] Sizes { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Title { get; set; }
    }
}