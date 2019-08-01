using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stuart_Hopwood_Photography_API.Entities;

namespace Stuart_Hopwood_Photography_API.Services
{
   public interface IOAuthService
   {
      /// <summary>
      /// Retreives valid a oAuth token, either from the datastore, or requests a new one from the oAuth API
      /// </summary>
      /// <param name="userKey"></param>
      /// <returns></returns>
      Task<OAuthToken> GetAuthTokenAsync(string userKey);


      /// <summary>
      /// Creates a url with the correct querystring paramters for requesting access to the desired scope.
      /// Used to redirect the user to the consent pages for authentication.
      /// </summary>
      /// <param name="returnUrl"></param>
      /// <returns></returns>
      RedirectResult SendAuthRequest(string returnUrl = null);

      /// <summary>
      /// Exchange AuthToken for a Access Token and Refresh Token
      /// </summary>
      /// <param name="code"></param>
      Task ExchangeAuthCodeForAuthToken(string code);
   }
}
