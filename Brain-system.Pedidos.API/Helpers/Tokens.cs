

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using BrainSystem.Auth.API.Models;
using BrainSystem.Pedidos.API.Auth;
using BrainSystem.Pedidos.API.Models;

namespace BrainSystem.Auth.API.Helpers
{
    public class InvalidToken : Exception
    {
        public InvalidToken(string message) : base(message) { }
        public InvalidToken(string message, Exception ex) : base(message, ex) { }
    }

    public class Tokens
    {
      public static async Task<AuthenticationToken> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory,string cliente, string usuario, string _refresh_token, Jwt jwtOptions)
      {
        AuthenticationToken response = new AuthenticationToken
        {
          id = identity.Claims.Single(c => c.Type == "id").Value,
          auth_token = await jwtFactory.GenerateEncodedToken(cliente, identity),
          expires_in = (int)jwtOptions.ValidFor.TotalSeconds,
          refresh_token = _refresh_token,
          usuario = usuario,
        };

        return response;// JsonConvert.SerializeObject(response, serializerSettings);
      }
    }
}
