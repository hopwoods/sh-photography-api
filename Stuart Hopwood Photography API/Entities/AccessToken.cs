namespace Stuart_Hopwood_Photography_API.Entities
{
   public class AccessToken
   {
      public string Access_Token { get; set; }

      public int Expires_In { get; set; }

      public string Scope { get; set; }

      public string Token_Type { get; set; }
   }
}
