using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShoppingAreas.Domain;
using ShoppingAreas.Domain.Models;
using ShoppingAreas.WebApi.Helpers;
using ShoppingAreas.WebApi.ViewModels.Account;

namespace ShoppingAreas.WebApi.Controllers
{
	[Route("api/[controller]")]
	public class AccountsController : Controller
	{
		private readonly ApplicationDbContext _appDbContext;
		private readonly UserManager<User> _userManager;
		private readonly IMapper _mapper;

		public AccountsController(UserManager<User> userManager, IMapper mapper, ApplicationDbContext appDbContext)
		{
			_userManager = userManager;
			_mapper = mapper;
			_appDbContext = appDbContext;
		}

		// POST api/accounts
		[HttpPost]
		public async Task<IActionResult> Post([FromBody]VmRegistration model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var userIdentity = _mapper.Map<User>(model);

			var result = await _userManager.CreateAsync(userIdentity, model.Password);

			if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

			var user = await _userManager.FindByNameAsync(userIdentity.UserName);

			var isAdmin = false;
			await _userManager.AddClaimAsync(user, new Claim(Constants.Strings.JwtClaimIdentifiers.Rol
				, isAdmin ? Constants.Strings.JwtClaims.ApiAdmin : Constants.Strings.JwtClaims.ApiAccess));
			await _userManager.UpdateAsync(user);

			return new OkObjectResult(new { Message = "Account created" });
		}
	}
}