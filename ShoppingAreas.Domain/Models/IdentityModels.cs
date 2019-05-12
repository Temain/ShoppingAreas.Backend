using Microsoft.AspNetCore.Identity;

namespace ShoppingAreas.Domain.Models
{
	public class User : IdentityUser<long>
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}

	public class UserLogin : IdentityUserLogin<long> { }
	public class UserRole : IdentityUserRole<long> { }
	public class UserClaim : IdentityUserClaim<long> { }
	public class Role : IdentityRole<long> { }
	public class RoleClaim : IdentityRoleClaim<long> { }
	public class UserToken : IdentityUserToken<long> { }
}
