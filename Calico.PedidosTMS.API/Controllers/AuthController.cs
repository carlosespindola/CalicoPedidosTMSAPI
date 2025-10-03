
 
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using Asp.Versioning;
using Calico.PedidosTMS.DAL.Repositories;
using Calico.PedidosTMS.API.Auth;
using Calico.PedidosTMS.Auth.API.Models;
using Calico.PedidosTMS.API.Models;
using Calico.PedidosTMS.Model;
using Calico.PedidosTMS.API.Helpers;
using Calico.PedidosTMS.Auth.API.Helpers;

namespace Calico.PedidosTMS.API.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IClienteAccesoApiRepository _clienteAccesoApiRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtFactory _jwtFactory;
        private readonly Jwt _jwtOptions;
        private readonly IConfiguration _configuration;

        public AuthController(IClienteAccesoApiRepository clienteAccesoApiRepository, IRefreshTokenRepository refreshTokenRepository, IConfiguration configuration, IJwtFactory jwtFactory, IOptions<Jwt> jwtOptions)
        {
            _clienteAccesoApiRepository = clienteAccesoApiRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _configuration = configuration;
        }

        // POST api/auth/login

        /// <summary>
        /// Log in and get token.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /auth/login
        ///
        /// </remarks>
        /// <returns>A JWT token</returns>
        /// <response code="200">Returns a JWT token</response>
        /// <response code="400">Invalid username or password or the user account is not active<br/>OR<br/>Invalid company</response>
        [HttpPost("login")]
        public async Task<IActionResult> Post([FromBody]LoginModel credentials)
        {
             if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CLIENTESACCESOAPI user = await ValidateUserCredentials(credentials.Cliente, credentials.Usuario, credentials.Password);
            if (user == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid cardID or PIN or the employee is not active.", ModelState));
            }

            //remove expired refresh tokens for current user
            //_dbContext.Database.ExecuteSqlCommand("DELETE FROM RefreshToken WHERE ExpiresUtc < '" + DateTime.Now.ToString("yyyyMMdd HH:mm") + "'");

            // NOTA: en el VS y sin el AsNoTracking() muestra la coleccion de UserAccounts de la compañia cargados...no se porque?!?!?!
            //       Al agregarle AsNoTracking() la coleccion de userAccounts queda sin cargar
            //       En ninguno de los 2 casos pude ver como cargaba los UserAccounts, ni en el trace del SQL Server ni con 
            //       el programa Entity Framework Profiler
            //
            //       Debido a esto es que hice un DTO para el company que no tuviera ninguna de las propiedades de navegacion de Company
            //       y que al serializarlo genere la minima info posible

            var identity = await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(user.CODIGOCLIENTE, user.USUARIOAPI));
            if (identity == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password or the user account is not active.", ModelState));
            }

            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.Cliente, user.USUARIOAPI, CreateRefreshToken(user.CODIGOCLIENTE, user.USUARIOAPI), _jwtOptions);

            return new OkObjectResult(jwt);
        }

        private string CreateRefreshToken(string cliente, string usuario)
        {
            var refresh_token = System.Guid.NewGuid().ToString().Replace("-", "");

            var rToken = new RefreshToken();
            rToken.CODIGOCLIENTE = cliente;
            rToken.USUARIOAPI = usuario;
            rToken.Token = refresh_token;
            rToken.IssuedUtc = DateTime.Now;
            rToken.ExpiresUtc = DateTime.Now.Add(_jwtOptions.RefreshTokenValidFor);

            // Delete any previous token for current user using Dapper
            _refreshTokenRepository.DeleteByClienteAndUsuarioAsync(cliente, usuario).Wait();

            // Create a new refresh token using Dapper
            _refreshTokenRepository.InsertAsync(rToken).Wait();

            return refresh_token;
        }

        private async Task<CLIENTESACCESOAPI> ValidateUserCredentials(string cliente, string usuario, string password)
        {
            if (string.IsNullOrEmpty(cliente))
                return null;

            // Validate credentials directly against the database using Dapper
            var user = await _clienteAccesoApiRepository.ValidateCredentialsAsync(cliente.TrimStart('0'), usuario, password);

            return user;
        }
    }
}
