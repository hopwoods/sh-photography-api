using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Stuart_Hopwood_Photography_API.Data;
using Stuart_Hopwood_Photography_API.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Stuart_Hopwood_Photography_API.Services
{
   public class GoogleIoAuthService : IOAuthService
   {
      private readonly ITokenDataStore _dataStore;
      private readonly ILogger<GoogleIoAuthService> _logger;

      private readonly string[] _scopes = { "https://www.googleapis.com/auth/photoslibrary.readonly" };  
      private readonly string RedirectUri = "https://localhost:44398/api/auth/callback";
      private readonly HttpClient _client = new HttpClient();

      public GoogleIoAuthService(ITokenDataStore dataStore, ILogger<GoogleIoAuthService> logger)
      {
         _dataStore = dataStore;
         _logger = logger;
      }

      /// <summary>
      /// Retreives valid a oAuth token, either from the datastore, or requests a new one from the oAuth API
      /// </summary>
      /// <param name="userKey"></param>
      /// <returns></returns>
      public OAuthToken GetAuthToken(string userKey)
      {
         _logger.LogDebug($"Get Auth Token from DB. UserKey = {userKey}");

         // Get stored token if it exists
         var storedAuthToken = _dataStore.GetToken(userKey);

         _logger.LogDebug($"Auth Token retrieved from DB. Token = {storedAuthToken}");

         if (storedAuthToken == null)
         {
            _logger.LogDebug($"Auth Token from DBis NULL, returning NULL");
            return null;
         }

         // If Expired, Refresh
         _logger.LogDebug($"Check if Token is expired");
         var isTokenExpired = IsAccessTokenExpired(storedAuthToken);

         if (!isTokenExpired)
         {
            _logger.LogDebug($"Token has NOT expired (isTokenExpired = false), returning token {storedAuthToken}");
            return storedAuthToken;
         }

         _logger.LogDebug($"Token has expired (isTokenExpired = true), refreshing token {storedAuthToken}");

         RefreshTokenAsync(storedAuthToken.refresh_token);

         // Get Updated version of the token
         _logger.LogDebug($"Token has been updated, Returning token {storedAuthToken}");

         return _dataStore.GetToken(userKey);
      }

      /// <summary>
      /// Check if a Access Token has expired
      /// </summary>
      /// <param name="authToken"></param>
      /// <returns></returns>
      private bool IsAccessTokenExpired(OAuthToken authToken)
      {
         _logger.LogWarning($"Param AuthToken is null, returning true.");
         if (authToken == null) return true;

         var currentDateTime = DateTime.UtcNow;
         var tokenIssued = DateTime.Parse(authToken.IssuedUtc).ToUniversalTime();
         var tokenExpires = tokenIssued.AddSeconds(authToken.expires_in);

         var isExpired = tokenExpires <= currentDateTime;

         _logger.LogDebug($"isExpired = {isExpired}), Token Expires: {tokenExpires}, returning {isExpired}");
         return isExpired;
      }

      /// <summary>
      /// Creates a url with the correct querystring paramters for requesting access to the desired scope.
      /// Used to redirect the user to the consent pages for authentication.
      /// </summary>
      /// <param name="returnUrl"></param>
      /// <returns></returns>
      public RedirectResult SendAuthRequest(string returnUrl = "")
      {
         var requestData = new Dictionary<string, string>
         {
            {"client_id", ClientInfo.ClientId},
            {"redirect_uri", ClientInfo.RedirectUri},
            {"scope", _scopes[0]},
            {"access_type", "offline"},
            {"include_granted_scopes", "true"},
            {"response_type", "code"},
            {"state", returnUrl }
         };

         var url = QueryHelpers.AddQueryString("https://accounts.google.com/o/oauth2/v2/auth", requestData);
         _logger.LogDebug($"Redirecting user to oAuth consent screen {url}");

         return new RedirectResult(url);
      }

      /// <summary>
      /// Exchange AuthToken for a Access Token and Refresh Token
      /// </summary>
      /// <param name="code"></param>
      public async Task ExchangeAuthCodeForAuthToken(string code)
      {
         var requestData = new Dictionary<string, string>
         {
            {"client_id", ClientInfo.ClientId},
            {"client_secret", ClientInfo.ClientSecret},
            {"code", code},
            {"grant_type", "authorization_code"},
            {"redirect_uri", RedirectUri}
         };

         _logger.LogDebug($"Request Data for Auth Code -> Access Code exchange: {requestData}");

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
            _logger.LogDebug($"Response from oAuth Server = {response.StatusCode} : {response.ReasonPhrase}");
            if (response.StatusCode != HttpStatusCode.OK) throw new ApplicationException(response.ReasonPhrase);

            var responseJson = await response.Content.ReadAsStringAsync();
            var accessToken = JsonConvert.DeserializeObject<OAuthToken>(responseJson);

            accessToken.UserKey = ClientInfo.UserName;
            accessToken.Issued = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
            accessToken.IssuedUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
            accessToken.scope = _scopes[0];

            _logger.LogDebug($"Access Token created {accessToken}");

            _logger.LogDebug($"Storing New Access Token.");
            _dataStore.StoreToken(accessToken);
         }
         
      }

      /// <summary>
      /// Send a RefreshToken to Google Auth API in exchange for a new Access Token
      /// </summary>
      /// <param name="refreshToken"></param>
      /// <returns></returns>
      private async void RefreshTokenAsync(string refreshToken)
      {
         AccessToken accessToken;
         var requestData = new Dictionary<string, string>
         {
            {"client_id", ClientInfo.ClientId},
            {"client_secret", ClientInfo.ClientSecret},
            {"refresh_token", refreshToken},
            {"grant_type", "refresh_token"},
         };

         _logger.LogDebug($"Request Data for Refresh Code -> Access Code exchange: {requestData}");

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
            _logger.LogDebug($"Response from oAuth Server = {response.StatusCode} : {response.ReasonPhrase}");

            if (response.StatusCode == HttpStatusCode.Unauthorized) return;

            var responseJson = await response.Content.ReadAsStringAsync();
            accessToken = JsonConvert.DeserializeObject<AccessToken>(responseJson);
         }

         _logger.LogDebug($"Access Token created {accessToken}");
         _logger.LogDebug($"Updating Access Token.");

         _dataStore.UpdateAccessToken(accessToken);
      }
   }
}
