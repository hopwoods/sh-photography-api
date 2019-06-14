using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Stuart_Hopwood_Photography_API.Entities
{

    // Todo - Make class non-static with static member so that it can use IConfiguration

    /// <summary>
    /// Client auth information, loaded from a Google user credential json string.
    /// </summary>
    public static class ClientInfo
    {
        private static IConfiguration Configuration { get; set; }
        public static string UserName { get; private set; }
        public static string ClientId { get; private set; }
        public static string ClientSecret { get; private set; }
        public static string RedirectUri { get; private set; }
        public static string ProjectId { get; private set; }

        private const string ClientApiSecret =
            "{\"web\":{\"client_id\":\"908335520577-t4p75mhif1vrfcmlqlr4u4det9pt0kog.apps.googleusercontent.com\",\"project_id\":\"sh-photography-1560067890037\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"rvAPB54ehghstbbCA5Jhm44f\",\"redirect_uris\":[\"https://stuarthopwoodphotographyapi20190612014652.azurewebsites.net\",\"https://localhost:44398\",\"https://localhost:44398/api/auth/callback\"],\"javascript_origins\":[\"https://stuarthopwoodphotographyapi20190612014652.azurewebsites.net\",\"https://localhost:44398\"]}}";
        static ClientInfo()
        {
            var secrets = JObject.Parse(ClientApiSecret)["web"];
            var projectId = secrets["project_id"].Value<string>();
            var clientId = secrets["client_id"].Value<string>();
            var clientSecret = secrets["client_secret"].Value<string>();

            UserName = "stoo.hopwood@gmail.com";
            RedirectUri = "https://localhost:44398/api/auth/callback";
            ProjectId = projectId;
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
    }
}