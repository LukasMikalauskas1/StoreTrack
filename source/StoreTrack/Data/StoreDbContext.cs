using StoreTrack.Data.Entities;
using Microsoft.EntityFrameworkCore;
//using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using StoreTrack.Auth.Model;

namespace StoreTrack.Data
{
	public class StoreDbContext : IdentityDbContext<StoreRestUser>
	{
		private readonly IConfiguration _configuration;
		public DbSet<Owner> Owners { get; set; }
		public DbSet<Shop> Shops { get; set; }
		public DbSet<Item> Items { get; set; }

		public StoreDbContext(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
		}
	}
}
