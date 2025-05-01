
 
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using BrainSystem.Pedidos.Model;
using BrainSystem.Pedidos.DAL;
using BrainSystem.Auth.API.Identity;
using BrainSystem.Auth.API.Models;
using BrainSystem.Auth.API.Helpers;
using Asp.Versioning;
using BrainSystem.Pedidos.API.Auth;
using BrainSystem.Pedidos.API.Models;
using BrainSystem.Pedidos.API.Helpers;

namespace BrainSystem.Pedidos.API.Controllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<CLIENTESACCESOAPI> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly Jwt _jwtOptions;
        private readonly IConfiguration _configuration;
        private readonly BrainSystemDBContext _dbContext;
        private readonly SaadisDBContext _saadisDbContext;

        public AuthController(UserManager<CLIENTESACCESOAPI> userManager, IConfiguration configuration, BrainSystemDBContext dbContext, SaadisDBContext saadisDbContext, IJwtFactory jwtFactory, IOptions<Jwt> jwtOptions)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _configuration = configuration;
            _dbContext = dbContext;
            _saadisDbContext = saadisDbContext;
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

            //deletes any previous token for current userAccountId
            FormattableString query = $"DELETE FROM RefreshToken WHERE CODIGO_CLIENTE = {cliente} AND USUARIO_API = {usuario}";
            _saadisDbContext.Database.ExecuteSqlInterpolated(query);

            //and now creates a new one
            _saadisDbContext.RefreshTokens.Add(rToken);
            _saadisDbContext.SaveChanges();
            
            return refresh_token;
        }

        private async Task<CLIENTESACCESOAPI> ValidateUserCredentials(string cliente, string usuario, string password)
        {
            if (string.IsNullOrEmpty(cliente))
                return await Task.FromResult<CLIENTESACCESOAPI>(null);

            // get the user to verifty
            var userToVerify = await _saadisDbContext.CLIENTESACCESOAPIs
                                     .Where(p => p.CODIGOCLIENTE == cliente
                                                 && (p.USUARIOAPI != null && p.USUARIOAPI == usuario)
                                                     || (p.USUARIOAPI == null && string.IsNullOrEmpty(p.USUARIOAPI)))
                                     .SingleOrDefaultAsync();

            //_userManager.FindByNameAsync(cliente, usuario);

            if (userToVerify == null) return await Task.FromResult<CLIENTESACCESOAPI>(null);

            // check the credentials
            //if (await _userManager.CheckPasswordAsync(userToVerify, password))
            if (userToVerify.PASSWORDAPI == password)
            {
                return await Task.FromResult(userToVerify);
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<CLIENTESACCESOAPI>(null);
        }
    }
}
