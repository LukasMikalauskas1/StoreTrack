using System.ComponentModel.DataAnnotations;
using StoreTrack.Auth.Model;

namespace StoreTrack.Data.Entities
{
	public class Owner
	{
		public int Id { get; set; }	
		public required string Name { get; set; }
		public required string LastName { get; set; }

		[Required]
		public required string UserId { get; set; }
		public StoreRestUser User { get; set; }
	}

	public record OwnerDto(int Id, string Name, string LastName);
}
