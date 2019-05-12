using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ShoppingAreas.Domain.Models;
using ShoppingAreas.WebApi.Helpers;
using ShoppingAreas.WebApi.Models;
using ShoppingAreas.WebApi.Services.Auth;
using ShoppingAreas.WebApi.ViewModels.Account;

namespace ShoppingAreas.WebApi.Controllers
{
	[Route("api/[controller]")]
	public class AuthController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly IJwtFactory _jwtFactory;
		private readonly JwtIssuerOptions _jwtOptions;

		public AuthController(UserManager<User> userManager, IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions)
		{
			_userManager = userManager;
			_jwtFactory = jwtFactory;
			_jwtOptions = jwtOptions.Value;
		}

		// POST api/auth/login
		[HttpPost("login")]
		public async Task<IActionResult> Post([FromBody]VmCredentials credentials)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var identity = await GetClaimsIdentity(credentials.UserName, credentials.Password);
			if (identity == null)
			{
				return BadRequest(Errors.AddErrorToModelState("login_failure", "Неверный логин или пароль.", ModelState));
			}

			var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.UserName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
			return new OkObjectResult(jwt);
		}

		private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
		{
			if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
				return await Task.FromResult<ClaimsIdentity>(null);

			// get the user to verifty
			var userToVerify = await _userManager.FindByNameAsync(userName);

			if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

			// check the credentials
			if (await _userManager.CheckPasswordAsync(userToVerify, password))
			{
				var claims = await _userManager.GetClaimsAsync(userToVerify);
				return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id, claims));
			}

			// Credentials are invalid, or account doesn't exist
			return await Task.FromResult<ClaimsIdentity>(null);
		}
	}
}