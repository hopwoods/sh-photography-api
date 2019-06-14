using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Stuart_Hopwood_Photography_API.Entities
{

    // Todo - Make class non-static with static member so that it can use IConfiguration

    /// <summary>
    /// Client auth information, loaded from a Google user credential json string.
    /// </summary>
    public class ClientInfo
    {
        public string UserName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }

        private IConfiguration Configuration { get; set; }


        public ClientInfo(IConfiguration configuration)
        {
            Configuration = configuration;
            UserName = Configuration["GoogleAPI:username"];
            ClientId = Configuration["GoogleAPI:client_id"];
            ClientSecret = Configuration["GoogleAPI:client_secret"];
            RedirectUri = Configuration["GoogleAPI:redirect_uri"];
        }
    }
}