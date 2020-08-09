using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProcessPension;

namespace MenuItemListing.Controllers
{
    /// <summary>
    /// Authentication Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private IConfiguration _config;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(TokenController));

        /// <summary>
        /// Dependency Injection
        /// </summary>
        /// <param name="config"></param>
        public TokenController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginUser login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJSONWebToken(LoginUser userInfo)
        {
            _log4net.Info("Token Is Generated!");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(30),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private LoginUser AuthenticateUser(LoginUser login)
        {
            LoginUser user = null;
            _log4net.Info("Validating the Admin!");

            //Validate the User Credentials    
            //Demo Purpose, I have Passed HardCoded User Information    
            if (login.Username == "Deepanshu" && login.Password == "Deepanshu12")
            {
                user = new LoginUser { Username = "Deepanshu", Password = "Deepanshu12" };
            }
            return user;
        }
    }
}
