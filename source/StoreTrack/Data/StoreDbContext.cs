using StoreTrack.Data.Entities;
using Microsoft.EntityFrameworkCore;
//using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using StoreTrack.Auth.Model;

namespace StoreTrack.Data
{
	public class StoreDbContext : IdentityDbContext<StoreRestUser>
	{
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Item>().ToTable("Item");
			modelBuilder.Entity<Owner>().ToTable("Owner");
			modelBuilder.Entity<Shop>().ToTable("Shop");
			// Other configurations
		}
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
			//"PostgreSQL"
			optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSQL"));
			//optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
		}
	}
}
