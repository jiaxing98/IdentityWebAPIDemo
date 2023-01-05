using AutoMapper;
using Azure;
using IdentityLearningAPI.Dtos;
using IdentityLearningAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityLearningAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(
            IConfiguration config,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _config = config;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserSignUpDto userSignUpDto)
        {
            var user = _mapper.Map<ApplicationUser>(userSignUpDto);
            var result = await _userManager.CreateAsync(user, userSignUpDto.Password);

            if(!result.Succeeded) 
            {
                var msg = result.Errors.Select(x => x.Description);
                return BadRequest(new ResponseDto
                {
                    Status = Status.Failure,
                    ErrorMessages = msg.ToList(),
                });
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Auth", new { token, email = user.Email }, Request.Scheme);

            return Ok(new ResponseDto
            {
                Result = link,
            });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest(
                new ResponseDto
                {
                    Status = Status.Failure,
                    ErrorMessages = new List<string> { $"Unable to find {email}!" },
                });

            await _userManager.ConfirmEmailAsync(user, token);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var user = await _userManager.FindByNameAsync(userLoginDto.Username);
            if (user == null) return BadRequest(
                new ResponseDto
                {
                    Status = Status.Failure,
                    ErrorMessages = new List<string> { "Invalid username" }
                });

            var result = await _signInManager.CheckPasswordSignInAsync(user, userLoginDto.Password, false);
            if (!result.Succeeded) return BadRequest(
                new ResponseDto
                {
                    Status = Status.Failure,
                    ErrorMessages = new List<string> { "Invalid password" }
                });

            var token = await GenerateJwtToken(user);
            return Ok(new ResponseDto
            {
                Status = Status.Success,
                Result = new
                {
                    token,
                    User = new
                    {
                        user.Id,
                        user.UserName,
                        user.Email,
                        user.PhoneNumber,
                    }
                }
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logout successfully!");
        }

        private async Task<string> GenerateJwtToken(ApplicationUser loggedInUser)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Email == loggedInUser.Email);
            var roles = await _userManager.GetRolesAsync(user);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };
            roles.ToList().ForEach(x => claims.Add(new Claim(ClaimTypes.Role, x)));

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("Jwt:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = credentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
