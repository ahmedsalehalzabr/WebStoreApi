﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebStoreApi.Model;
using WebStoreApi.Services;

namespace WebStoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly AppDbContext context;

        public AccountController(IConfiguration configuration,AppDbContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }

     

        [HttpPost("Register")]
        public IActionResult Register(UserDto userDto)
        {
            var emailCount =  context.Users.Count(u => u.Email == userDto.Email);
            if (emailCount > 0)
            {
                ModelState.AddModelError("Email", "the email address is already used");
                return BadRequest(ModelState);
            }
            //encrypt the password
            var passhasher = new PasswordHasher<User>();
            var encrPass =passhasher.HashPassword(new Model.User(), userDto.Password);

            //create new account
            User user = new User()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                Phone = userDto.Phone ?? "",
                Address = userDto.Address,
                Password = encrPass,
                Role = "client",
                CreateAt = DateTime.Now,

            };

            context.Users.Add(user);
            context.SaveChanges();

            var jwt = CreateJwToken(user);

            UserProfileDto profile = new UserProfileDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreateAt = DateTime.Now,
            };

            var response = new
            {
                Token = jwt,
                User = profile
            };
            return Ok(response);
        }

        [HttpPost("Login")]
        public IActionResult Login(string email, string password)
        {
            var user = context.Users.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("Error", "email or password not valid");
                return BadRequest(ModelState);
            }

            // verify the password
            var passHash = new PasswordHasher<User>();
            var result = passHash.VerifyHashedPassword(new Model.User(), user.Password, password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("Password", "wrong password");
                return BadRequest(ModelState);
            }

            var jwt = CreateJwToken(user);

            UserProfileDto profile = new UserProfileDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreateAt = DateTime.Now,
            };

            var response = new
            {
                Token = jwt,
                User = profile
            };
            return Ok(response);
        }

        [HttpPost("ForgotPassword")]
        public IActionResult ForgotPassword(string email)
        {
            var user = context.Users.FirstOrDefault(u => u .Email == email);
            if (user == null)
            {
                return NotFound();
            }
            //delete any old password reset request
            var oldPassReset = context.PasswordResets.FirstOrDefault(r => r.Email == email);
            if (oldPassReset != null)
            {
                //delete old password reset request 
                context.Remove(oldPassReset);
            }

            // create password reset token
            string token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();

            var pssReset = new PasswordReset()
            {
                Email = email,
                Token = token,
                CreatedAt = DateTime.Now,
            };
            context.PasswordResets.Add(pssReset);
            context.SaveChanges();

            // send the password peset token by email to the user
            string emailSubject = "Pssword Reset";
            string username = user.FirstName + " " + user.LastName;
            string emailMessage = "Dear " + username + "\n" +
                "We received your password reset request. \n" +
                "Please copy the following token and paste it in the password reset form: \n" +
                token + "\n\n" +
                "Best Regards\n";

            return Ok();
        }

        [Authorize]
        [HttpGet("GetTokenClaims")]
        public IActionResult GetTokenClaims()
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                Dictionary<string, string> claims = new Dictionary<string, string>();

                foreach (Claim claim in identity.Claims)
                {
                    claims.Add(claim.Type, claim.Value);
                }
                return Ok(claims);
            }
            return Ok();
        }


        private string CreateJwToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", "" + user.Id),
                new Claim("role", user.Role)
            };

            string strKey = configuration["JwtSettings:Key"]!;
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(strKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials:creds
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
