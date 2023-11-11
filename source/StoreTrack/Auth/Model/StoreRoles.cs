namespace StoreTrack.Auth.Model
{
	public static class StoreRoles
	{
		public const string Admin = nameof(Admin);
		public const string StoreUser = nameof(StoreUser);

		public static readonly IReadOnlyCollection<string> All = new[] { Admin, StoreUser };

	}
}
