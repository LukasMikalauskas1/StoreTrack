using Microsoft.AspNetCore.Identity;

namespace StoreTrack.Auth.Model
{
	public class StoreRestUser : IdentityUser
	{
		public bool ForceRelogin { get; set; }

	}
}
