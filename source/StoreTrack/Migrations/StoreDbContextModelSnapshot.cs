using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StoreTrack.Data;

#nullable disable

namespace StoreTrack.Migrations
{
	[DbContext(typeof(StoreDbContext))]
	partial class StoreDbContextModelSnapshot : ModelSnapshot
	{
		protected override void BuildModel(ModelBuilder modelBuilder)
		{
			modelBuilder
				.HasAnnotation("ProductVersion", "7.0.13")
				.HasAnnotation("Relational:MaxIdentifierLength", 128);

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
			{
				b.Property<string>("Id")
					.HasColumnType("character varying(450)");

				b.Property<string>("ConcurrencyStamp")
					.IsConcurrencyToken()
					.HasColumnType("text");

				b.Property<string>("Name")
					.HasMaxLength(256)
					.HasColumnType("character varying(256)");

				b.Property<string>("NormalizedName")
					.HasMaxLength(256)
					.HasColumnType("character varying(256)");

				b.HasKey("Id");

				b.HasIndex("NormalizedName")
					.IsUnique()
					.HasDatabaseName("RoleNameIndex")
					.HasFilter("[NormalizedName] IS NOT NULL");

				b.ToTable("AspNetRoles", "public");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
			{
				b.Property<int>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("integer");

				SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

				b.Property<string>("ClaimType")
					.HasColumnType("character varying(450)");

				b.Property<string>("ClaimValue")
					.HasColumnType("character varying(450)");

				b.Property<string>("RoleId")
					.IsRequired()
					.HasColumnType("character varying(450)");

				b.HasKey("Id");

				b.HasIndex("RoleId");

				b.ToTable("AspNetRoleClaims", "public");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
			{
				b.Property<int>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("integer");

				SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

				b.Property<string>("ClaimType")
					.HasColumnType("character varying(450)");

				b.Property<string>("ClaimValue")
					.HasColumnType("character varying(450)");

				b.Property<string>("UserId")
					.IsRequired()
					.HasColumnType("character varying(450)");

				b.HasKey("Id");

				b.HasIndex("UserId");

				b.ToTable("AspNetUserClaims", "public");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
			{
				b.Property<string>("LoginProvider")
					.HasColumnType("character varying(450)");

				b.Property<string>("ProviderKey")
					.HasColumnType("character varying(450)");

				b.Property<string>("ProviderDisplayName")
					.HasColumnType("text");

				b.Property<string>("UserId")
					.IsRequired()
					.HasColumnType("character varying(450)");

				b.HasKey("LoginProvider", "ProviderKey");

				b.HasIndex("UserId");

				b.ToTable("AspNetUserLogins", "public");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
			{
				b.Property<string>("UserId")
					.HasColumnType("character varying(450)");

				b.Property<string>("RoleId")
					.HasColumnType("character varying(450)");

				b.HasKey("UserId", "RoleId");

				b.HasIndex("RoleId");

				b.ToTable("AspNetUserRoles", "public");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
			{
				b.Property<string>("UserId")
					.HasColumnType("character varying(450)");

				b.Property<string>("LoginProvider")
					.HasColumnType("character varying(450)");

				b.Property<string>("Name")
					.HasColumnType("character varying(450)");

				b.Property<string>("Value")
					.HasColumnType("text");

				b.HasKey("UserId", "LoginProvider", "Name");

				b.ToTable("AspNetUserTokens", "public");
			});

			modelBuilder.Entity("StoreTrack.Auth.Model.StoreRestUser", b =>
			{
				b.Property<string>("Id")
					.HasColumnType("character varying(450)");

				b.Property<int>("AccessFailedCount")
					.HasColumnType("integer");

				b.Property<string>("ConcurrencyStamp")
					.IsConcurrencyToken()
					.HasColumnType("text");

				b.Property<string>("Email")
					.HasMaxLength(256)
					.HasColumnType("character varying(256)");

				b.Property<bool>("EmailConfirmed")
					.HasColumnType("boolean");

				b.Property<bool>("ForceRelogin")
					.HasColumnType("boolean");

				b.Property<bool>("LockoutEnabled")
					.HasColumnType("boolean");

				b.Property<DateTimeOffset?>("LockoutEnd")
					.HasColumnType("timestamp with time zone");

				b.Property<string>("NormalizedEmail")
					.HasMaxLength(256)
					.HasColumnType("character varying(256)");

				b.Property<string>("NormalizedUserName")
					.HasMaxLength(256)
					.HasColumnType("character varying(256)");

				b.Property<string>("PasswordHash")
					.HasColumnType("text");

				b.Property<string>("PhoneNumber")
					.HasColumnType("text");

				b.Property<bool>("PhoneNumberConfirmed")
					.HasColumnType("boolean");

				b.Property<string>("SecurityStamp")
					.HasColumnType("text");

				b.Property<bool>("TwoFactorEnabled")
					.HasColumnType("boolean");

				b.Property<string>("UserName")
					.HasMaxLength(256)
					.HasColumnType("character varying(256)");

				b.HasKey("Id");

				b.HasIndex("NormalizedEmail")
					.HasDatabaseName("EmailIndex");

				b.HasIndex("NormalizedUserName")
					.IsUnique()
					.HasDatabaseName("UserNameIndex")
					.HasFilter("[NormalizedUserName] IS NOT NULL");

				b.ToTable("AspNetUsers", "public");
			});

			modelBuilder.Entity("StoreTrack.Data.Entities.Item", b =>
			{
				b.Property<int>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("integer");

				b.Property<string>("Description")
					.IsRequired()
					.HasColumnType("text");

				b.Property<string>("Name")
					.IsRequired()
					.HasColumnType("text");

				b.Property<string>("Picture")
					.IsRequired()
					.HasColumnType("text");

				b.Property<string>("UserId")
					.IsRequired()
					.HasColumnType("character varying(450)");

				b.Property<int>("shopId")
					.HasColumnType("integer");

				b.HasKey("Id");

				b.HasIndex("UserId");

				b.HasIndex("shopId");

				b.ToTable("Items", "public");
			});

			modelBuilder.Entity("StoreTrack.Data.Entities.Owner", b =>
			{
				b.Property<int>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("integer");

				b.Property<string>("LastName")
					.IsRequired()
					.HasColumnType("text");

				b.Property<string>("Name")
					.IsRequired()
					.HasColumnType("text");

				b.Property<string>("UserId")
					.IsRequired()
					.HasColumnType("character varying(450)");

				b.HasKey("Id");

				b.HasIndex("UserId");

				b.ToTable("Owners", "public");
			});

			modelBuilder.Entity("StoreTrack.Data.Entities.Shop", b =>
			{
				b.Property<int>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("integer");

				b.Property<string>("Address")
					.IsRequired()
					.HasColumnType("text");

				b.Property<string>("Name")
					.IsRequired()
					.HasColumnType("text");

				b.Property<string>("UserId")
					.IsRequired()
					.HasColumnType("character varying(450)");

				b.Property<int>("ownerId")
					.HasColumnType("integer");

				b.HasKey("Id");

				b.HasIndex("UserId");

				b.HasIndex("ownerId");

				b.ToTable("Shops", "public");
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
			{
				b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
					.WithMany()
					.HasForeignKey("RoleId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
			{
				b.HasOne("StoreTrack.Auth.Model.StoreRestUser", null)
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
			{
				b.HasOne("StoreTrack.Auth.Model.StoreRestUser", null)
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
			{
				b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
					.WithMany()
					.HasForeignKey("RoleId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();

				b.HasOne("StoreTrack.Auth.Model.StoreRestUser", null)
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
			{
				b.HasOne("StoreTrack.Auth.Model.StoreRestUser", null)
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("StoreTrack.Data.Entities.Item", b =>
			{
				b.HasOne("StoreTrack.Auth.Model.StoreRestUser", "User")
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();

				b.HasOne("StoreTrack.Data.Entities.Shop", "Shop")
					.WithMany()
					.HasForeignKey("shopId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();

				b.Navigation("User");

				b.Navigation("Shop");
			});

			modelBuilder.Entity("StoreTrack.Data.Entities.Owner", b =>
			{
				b.HasOne("StoreTrack.Auth.Model.StoreRestUser", "User")
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();

				b.Navigation("User");
			});

			modelBuilder.Entity("StoreTrack.Data.Entities.Shop", b =>
			{
				b.HasOne("StoreTrack.Auth.Model.StoreRestUser", "User")
					.WithMany()
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();

				b.HasOne("StoreTrack.Data.Entities.Owner", "Owner")
					.WithMany()
					.HasForeignKey("ownerId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();

				b.Navigation("User");

				b.Navigation("Owner");
			});
		}
	}
}
