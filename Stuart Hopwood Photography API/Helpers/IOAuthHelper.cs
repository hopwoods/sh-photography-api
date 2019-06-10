using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;

namespace Stuart_Hopwood_Photography_API.Helpers
{
   public interface IOAuthHelper
   {
      UserCredential GetUserCredentials();

      Task<bool> RefreshToken(UserCredential credential);
   }
}
