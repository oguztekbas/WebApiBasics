using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiBasics.IdentityModels;
using WebApiBasics.Models;

namespace WebApiBasics.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> usermanager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;

        public AuthenticationController(UserManager<ApplicationUser> usermanager , SignInManager<ApplicationUser> signInManager , IConfiguration configuration)
        {

            this.usermanager = usermanager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register(DtoRegisterUser user)
        {

            var newuser = new ApplicationUser()
            {
                Id = Guid.NewGuid().ToString(),
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName

            };

            var result = await usermanager.CreateAsync(newuser, user.Password);

            if(result.Succeeded)
            {
                return StatusCode(201);
            }
            return BadRequest(result.Errors);
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(DtoLoginUser user)
        {

            var loginuser = await usermanager.FindByNameAsync(user.UserName);

            if(loginuser == null)
            {
                return BadRequest(  new { message = "kullanıcı adı hatalı" } );
            }

            var result = await signInManager.CheckPasswordSignInAsync(loginuser, user.Password, true);

            if(result.Succeeded)

            {
                return Ok(new {

                    token = GenerateJwtToken(loginuser),
                    username = user.UserName 

                });

            }

            return Unauthorized(); //401 

        }


        private string GenerateJwtToken(ApplicationUser user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var stringkey = configuration.GetSection("AppSettings:Secret").Value;
            byte [] bytekey = Encoding.ASCII.GetBytes(stringkey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(new Claim[]
                {

                    new Claim("userid", user.Id.ToString()),
                    new Claim("username", user.UserName)


                }),

                //Issuer = "oguztekbas.xyz", Extra Validate
                //Audience = "oguztekbasfirma",

                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(bytekey), SecurityAlgorithms.HmacSha256Signature)



            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token); //Token'ı stringe çevirip tekrar gönderdik.

        }


    }
}
