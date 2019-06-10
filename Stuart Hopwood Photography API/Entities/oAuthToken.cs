using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Stuart_Hopwood_Photography_API.Entities
{
    public class OAuthToken
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string UserKey { get; set; }

        [StringLength(2000)]
        public string Token { get; set; }
    }
}
