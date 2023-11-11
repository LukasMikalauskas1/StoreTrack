using System.ComponentModel.DataAnnotations;
using StoreTrack.Auth.Model;

namespace StoreTrack.Data.Entities
{
	public class Shop
	{
		public int Id { get; set; }

		public required string Name { get; set; }

		public required string Address { get; set; }

		public required Owner owner { get; set; }

		[Required]
		public required string UserId { get; set; }
		public StoreRestUser User { get; set; }
	}

	public record ShopDto(int Id, string Name, string Address, int OwnerId);
}
