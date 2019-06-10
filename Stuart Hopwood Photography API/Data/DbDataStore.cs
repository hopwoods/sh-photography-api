using Google.Apis.Util.Store;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stuart_Hopwood_Photography_API.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stuart_Hopwood_Photography_API.Data
{
    public class DbDataStore : IDataStore
    {
        private readonly ApplicationContext _context;

        public DbDataStore(ApplicationContext context)
        {
            _context = context;
        }


        #region Implementation of IDataStore

        /// <summary>
        /// Asynchronously stores the given value for the given key (replacing any existing value).
        /// </summary>
        /// <typeparam name="T">The type to store in the data store.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to store.</param>
        public Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var contents = JsonConvert.SerializeObject(value);
            var credentials = JsonConvert.DeserializeObject<OAuthToken>(contents);

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var dbKey = _context.OAuthToken.FirstOrDefault(k => k.UserKey == key);

                    if (dbKey == null)
                    {
                        _context.OAuthToken.Add(new OAuthToken
                        {
                            UserKey = key,
                            //Token = contents,
                            access_token = credentials.access_token,
                            expires_in = credentials.expires_in,
                            id_token = credentials.id_token,
                            Issued = credentials.Issued,
                            IssuedUtc = credentials.IssuedUtc,
                            refresh_token = credentials.refresh_token,
                            scope = credentials.scope,
                            token_type = credentials.token_type
                        });
                    }
                    else
                    {
                        dbKey.UserKey = key;
                        //dbKey.Token = contents;
                        dbKey.access_token = credentials.access_token;
                        dbKey.expires_in = credentials.expires_in;
                        dbKey.id_token = credentials.id_token;
                        dbKey.Issued = credentials.Issued;
                        dbKey.IssuedUtc = credentials.IssuedUtc;
                        dbKey.refresh_token = credentials.refresh_token;
                        dbKey.scope = credentials.scope;
                        dbKey.token_type = credentials.token_type;
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

            return Task.Delay(0);
        }

        /// <summary>
        /// Asynchronously deletes the given key. The type is provided here as well because the "real" saved key should
        /// contain type information as well, so the data store will be able to store the same key for different types.
        /// </summary>
        /// <typeparam name="T">
        /// The type to delete from the data store.
        /// </typeparam>
        /// <param name="key">The key to delete.</param>
        public Task DeleteAsync<T>(string key)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var dbKey = _context.OAuthToken.FirstOrDefault(k => k.UserKey == key);
                    if (dbKey != null)
                    {
                        _context.Remove((object)dbKey);
                        _context.SaveChanges();
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new ApplicationException(ex.Message, ex.InnerException);
                }
            }
            return Task.Delay(0);
        }

        /// <summary>
        /// Asynchronously returns the stored value for the given key or <c>null</c> if not found.
        /// </summary>
        /// <typeparam name="T">The type to retrieve from the data store.</typeparam>
        /// <param name="key">The key to retrieve its value.</param>
        /// <returns>
        /// The stored object.
        /// </returns>
        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }
            
            var completionSource = new TaskCompletionSource<T>();
            var dbKey = _context.OAuthToken.FirstOrDefault(k => k.UserKey == key);

            var token = JsonConvert.SerializeObject(dbKey);

            completionSource.SetResult(dbKey == null ? default(T) : JsonConvert.DeserializeObject<T>(token));

            return completionSource.Task;
        }

        /// <summary>
        /// Asynchronously clears all values in the data store.
        /// </summary>
        public Task ClearAsync()
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
            return Task.Delay(0);
        }

        #endregion
    }
}
