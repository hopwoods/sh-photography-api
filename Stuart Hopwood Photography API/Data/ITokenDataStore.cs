using Stuart_Hopwood_Photography_API.Entities;

namespace Stuart_Hopwood_Photography_API.Data
{
   public interface ITokenDataStore
   {
      /// <summary>
      /// Stores the given value for the given key (replacing any existing value).
      /// </summary>
      /// <param name="token"></param>
      void StoreToken(OAuthToken token);

      /// <summary>
      /// Update the stored auth token with a new Access Token
      /// </summary>
      /// <param name="accessToken"></param>
      void UpdateAccessToken(AccessToken accessToken);

      /// <summary>
      /// Deletes the given token.
      /// </summary>
      /// <param name="key">The key to delete.</param>
      void DeleteToken(string key);

      /// <summary>
      /// Returns the stored value for the given key or <c>null</c> if not found.
      /// </summary>
      /// <param name="key">The key to retrieve its value.</param>
      /// <returns>
      /// The stored token.
      /// </returns>
      OAuthToken GetToken(string key);

      /// <summary>
      /// Clears all values in the data store.
      /// </summary>
      void ClearTokens();
   }
}