using System.Collections.Generic;
using Stuart_Hopwood_Photography_API.Entities;

namespace Stuart_Hopwood_Photography_API.Services
{
   public interface IUserService
   {
      User Authenticate(string username, string password);
      IEnumerable<User> GetAll();
      User GetById(int id);
      User Create(User user, string password);
      void Update(User user, string password = null);
      void Delete(int id);
   }
}