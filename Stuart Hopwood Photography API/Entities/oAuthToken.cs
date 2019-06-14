using System.ComponentModel.DataAnnotations;

namespace Stuart_Hopwood_Photography_API.Entities
{
    public class OAuthToken
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string UserKey { get; set; }

        [StringLength(2000)]
        public string access_token { get; set; }

        [StringLength(255)]
        public string token_type { get; set; }

        public int expires_in { get; set; }

        [StringLength(1000)]
        public string refresh_token { get; set; }

        [StringLength(1000)]
        public string scope { get; set; }

        public string Issued { get; set; }

        public string IssuedUtc { get; set; }
    }
}
