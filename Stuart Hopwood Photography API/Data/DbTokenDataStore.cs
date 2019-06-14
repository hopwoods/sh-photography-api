using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stuart_Hopwood_Photography_API.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stuart_Hopwood_Photography_API.Data
{
    public class DbTokenDataStore : ITokenDataStore
    {
        private readonly ApplicationContext _context;

        public DbTokenDataStore(ApplicationContext context)
        {
            _context = context;
        }

        #region Implementation of IDataStore

        /// <summary>
        /// Asynchronously stores the given value for the given key (replacing any existing value).
        /// </summary>
        /// <param name="token">The token credentials to store</param>
        public void StoreToken(OAuthToken token)
        {
            if (token == null)
            {
                throw new ArgumentException("oAuth Token missing.");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var dbKey = _context.OAuthToken.FirstOrDefault(k => k.UserKey == token.UserKey);
                    if (dbKey == null)
                    {
                        _context.OAuthToken.Add(new OAuthToken
                        {
                            UserKey = token.UserKey,
                            access_token = token.access_token,
                            expires_in = token.expires_in,
                            Issued = token.Issued,
                            IssuedUtc = token.IssuedUtc,
                            refresh_token = token.refresh_token,
                            scope = token.scope,
                            token_type = token.token_type
                        });
                    }
                    else
                    {
                        dbKey.UserKey = token.UserKey;
                        dbKey.access_token = token.access_token;
                        dbKey.expires_in = token.expires_in;
                        dbKey.Issued = token.Issued;
                        dbKey.IssuedUtc = token.IssuedUtc;
                        dbKey.refresh_token = token.refresh_token;
                        dbKey.scope = token.scope;
                        dbKey.token_type = token.token_type;
                    }
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message, ex.InnerException);
                }
            }
        }

        public void UpdateAccessToken(AccessToken accessToken)
        {
            var dbKey = _context.OAuthToken.FirstOrDefault(k => k.UserKey == ClientInfo.UserName);
            if (dbKey == null) return;
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    dbKey.access_token = accessToken.Access_Token;
                    dbKey.scope = accessToken.Scope;
                    dbKey.expires_in = accessToken.Expires_In;
                    dbKey.Issued = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
                    dbKey.IssuedUtc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK");
                    dbKey.token_type = accessToken.Token_Type;

                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message, ex.InnerException);
                }
            }
        }

        /// <summary>
        /// Asynchronously deletes the given key. The type is provided here as well because the "real" saved key should
        /// contain type information as well, so the data store will be able to store the same key for different types.
        /// </summary>
        /// <param name="key">The key to delete.</param>
        public void DeleteToken(string key)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var dbKey = _context.OAuthToken.FirstOrDefault(k => k.UserKey == key);
                    if (dbKey == null) return;

                    _context.Remove((object)dbKey);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message, ex.InnerException);
                }
            }
        }

        /// <summary>
        /// Asynchronously returns the stored value for the given key or <c>null</c> if not found.
        /// </summary>
        /// <typeparam name="T">The type to retrieve from the data store.</typeparam>
        /// <param name="key">The key to retrieve its value.</param>
        /// <returns>
        /// The stored object.
        /// </returns>
        public OAuthToken GetToken(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var token = _context.OAuthToken.FirstOrDefault(k => k.UserKey == key);
            return token;
        }

        /// <summary>
        /// Asynchronously clears all values in the data store.
        /// </summary>
        public void ClearTokens()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [OAuthToken]");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message, ex.InnerException);
                }
            }
        }

        #endregion
    }
}
