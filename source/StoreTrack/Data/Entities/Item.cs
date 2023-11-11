using System.ComponentModel.DataAnnotations;
using StoreTrack.Auth.Model;

namespace StoreTrack.Data.Entities
{
	public class Item
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public required string Description { get; set; }
		public required string Picture { get; set; }
		public required Shop shop { get; set; }

		[Required]
		public required string UserId { get; set; }
		public StoreRestUser User { get; set; }
	}

	public record ItemDto(int Id, string Name, string Description, string Picture, int ShopId);
}
