using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stuart_Hopwood_Photography_API.Entities;

namespace Stuart_Hopwood_Photography_API.Services
{
   public interface IOAuthService
   {
      Task<OAuthToken> GetAuthTokenAsync(string userKey);

      RedirectResult SendAuthRequest(string returnUrl = null);

      Task ExchangeAuthCodeForAuthToken(string code);
   }
}
