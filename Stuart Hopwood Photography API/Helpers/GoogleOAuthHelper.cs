using System;
using Google.Apis.Auth.OAuth2;
using Stuart_Hopwood_Photography_API.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Stuart_Hopwood_Photography_API.Entities;

namespace Stuart_Hopwood_Photography_API.Helpers
{
   public class GoogleOAuthHelper : IOAuthHelper
   {
      private readonly ApplicationContext _context;
      private readonly string[] _scopes = { "https://www.googleapis.com/auth/photoslibrary.readonly" };
      private const string ClientApiSecret = "{\"installed\":{\"client_id\":\"908335520577-6avd0rlkkico8eoe4t2kasp7r8ede9bq.apps.googleusercontent.com\",\"project_id\":\"sh-photography-1560067890037\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"6Q2ODMCe0deI45yJ7UPMcBNg\",\"redirect_uris\":[\"urn:ietf:wg:oauth:2.0:oob\",\"http://localhost\"]}}";

      private IConfiguration Configuration { get; set; }
      private string UserName { get; set; }
      private string ClientId { get; set; }
      private string ClientSecret { get; set; }

      private readonly HttpClient _client = new HttpClient();

      public GoogleOAuthHelper(ApplicationContext context, IConfiguration configuration)
      {
         _context = context;
         Configuration = configuration;
         UserName = Configuration["GoogleAPI:username"];
         ClientId = Configuration["GoogleAPI:ClientId"];
         ClientSecret = Configuration["GoogleAPI:ClientSecret"];
      }

      public UserCredential GetUserCredentials()
      {
         var stream = CreateClientSecretStream();
         var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.Load(stream).Secrets,
            _scopes,
            UserName,
            CancellationToken.None,
            new DbDataStore(_context)
         ).Result;
         return credential;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns></returns>
      private static MemoryStream CreateClientSecretStream()
      {
         var byteArray = Encoding.ASCII.GetBytes(ClientApiSecret);
         var stream = new MemoryStream(byteArray);
         return stream;
      }

      /// <summary>
      /// Send a RefreshToken to Google Auth API in exchange for a new Access Token
      /// </summary>
      /// <param name="refreshToken"></param>
      /// <returns></returns>
      public async Task<bool> RefreshToken(string refreshToken)
      {
         GoogleAccessToken responseObject;
         var requestData = new Dictionary<string, string>
         {
            {"client_id", ClientId},
            {"client_secret", ClientSecret},
            {"refresh_token", refreshToken},
            {"grant_type", "refresh_token"},
         };

         var content = new FormUrlEncodedContent(requestData);
         var request = new HttpRequestMessage()
         {
            RequestUri = new Uri("https://www.googleapis.com/oauth2/v4/token"),
            Method = HttpMethod.Post
         };

         request.Headers.Add("ContentType", "application/x-www-form-urlencoded");
         request.Content = content;

         using (var response = await _client.PostAsync(request.RequestUri, request.Content))
         {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
               return false;
            var responseJson = await response.Content.ReadAsStringAsync();
            responseObject = JsonConvert.DeserializeObject<GoogleAccessToken>(responseJson);
         }

         // Update credentials
         var dbKey = _context.OAuthToken.FirstOrDefault(k => k.UserKey == UserName);
         if (dbKey == null) return false;
         using (var transaction = _context.Database.BeginTransaction())
         {
            try
            {
               dbKey.access_token = responseObject.Access_Token;
               dbKey.scope = responseObject.Scope;
               dbKey.expires_in = responseObject.Expires_In;
               dbKey.Issued = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
               dbKey.IssuedUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
               dbKey.token_type = responseObject.Token_Type;

               _context.SaveChanges();
               transaction.Commit();
               return true;
            }
            catch (Exception ex)
            {
               transaction.Rollback();
               throw new ApplicationException(ex.Message, ex.InnerException);
            }
         }
      }
   }
}
