using Microsoft.EntityFrameworkCore;
using Stuart_Hopwood_Photography_API.Entities;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Stuart_Hopwood_Photography_API.Data
{
    public class DbTokenDataStore : ITokenDataStore
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<DbTokenDataStore> _logger;

        public DbTokenDataStore(ApplicationContext context, ILogger<DbTokenDataStore> logger)
        {
            _context = context;
            _logger = logger;
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

            _logger.LogDebug($"Store Token in DB. Token {token}");
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _logger.LogDebug($"Get AuthToken from DB if exists");
                    var dbKey = _context.OAuthToken.FirstOrDefault(k => k.UserKey == token.UserKey);
                    
                    if (dbKey == null)
                    {
                        _logger.LogDebug($"AuthToken does not exist, adding to db.");
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
                        _logger.LogDebug($"AuthToken does exist, updating db.");
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
                    _logger.LogDebug($"db changes saved.");
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError($"{ex.Message}");
                    throw new ApplicationException(ex.Message, ex.InnerException);
                }
            }
        }

        public void UpdateAccessToken(AccessToken accessToken)
        {
            _logger.LogDebug($"Updating AuthToken with new AccessToken {accessToken}.");
            var dbKey = _context.OAuthToken.FirstOrDefault(k => k.UserKey == ClientInfo.UserName);
            if (dbKey == null)
            {
                _logger.LogError($"AuthToken does not exist in DB");
                return;
            }
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
                    _logger.LogDebug($"Updating AuthToken in db {dbKey}.");
                    _context.SaveChanges();
                    _logger.LogDebug($"db changes saved");
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError($"{ex.Message}");
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

                    _logger.LogDebug($"Deleting AuthToken in db {dbKey}.");
                    _context.Remove((object)dbKey);
                    _context.SaveChanges();
                    _logger.LogDebug($"db changes saved.");
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError($"{ex.Message}");
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
            _logger.LogDebug($"Retrieved AuthToken from db {token}.");
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
                    _logger.LogDebug($"Deleting all AuthToken's in db.");
                    _context.Database.ExecuteSqlCommand("TRUNCATE TABLE [OAuthToken]");
                    _logger.LogDebug($"Deleted all AuthToken's in db.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ex.Message}");
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message, ex.InnerException);
                }
            }
        }
        #endregion
    }
}
