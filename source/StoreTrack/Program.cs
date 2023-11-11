using System;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using O9d.AspNet.FluentValidation;
using StoreTrack.Data;
using StoreTrack.Data.Entities;
using System.Net;
using System.Text.Json;
using StoreTrack.Helpers;
using Microsoft.AspNetCore.Identity;
using StoreTrack.Auth.Model;//new
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using StoreTrack.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StoreDbContext>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddTransient<JwtTokenService>();
builder.Services.AddScoped<AuthDbSeeder>();

//var app = builder.Build();

builder.Services.AddIdentity<StoreRestUser, IdentityRole>()
	.AddEntityFrameworkStores<StoreDbContext>()
	.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters.ValidAudience = builder.Configuration["Jwt:ValidAudience"];
	options.TokenValidationParameters.ValidIssuer = builder.Configuration["Jwt:ValidIssuer"];
	options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]));
});

builder.Services.AddAuthorization();

var app = builder.Build();

/*
	/api/v1/owners GET List 200
	/api/v1/owners/{id} GET One 200
	/api/v1/owners POST Create 201
	/api/v1/owners/{id} PUT/PATCH Modify 200
	/api/v1/owners/{id} DELETE Remove 200/204
*/
//----------OWNER----------
var ownersGroup = app.MapGroup("/api").WithValidationFilter();

ownersGroup.MapGet("owners", async ([AsParameters] SearchParameters searchParams, StoreDbContext dbContext, LinkGenerator linkGenerator, HttpContext httpContext,CancellationToken cancellationToken) =>
{
	var queryable = dbContext.Owners.AsQueryable().OrderBy(o => o.Name);
	var pagedList = await PagedList<Owner>.CreateAsync(queryable, searchParams.PageNumber!.Value, searchParams.PageSize!.Value);

	var previousPageLink =
		pagedList.HasPrevious
			? linkGenerator.GetUriByName(httpContext, "GetOwners",
				new { pageNumber = searchParams.PageNumber - 1, pageSize = searchParams.PageSize })
			: null;

	var nextPageLink =
		pagedList.HasNext
			? linkGenerator.GetUriByName(httpContext, "GetOwners",
				new { pageNumber = searchParams.PageNumber + 1, pageSize = searchParams.PageSize })
			: null;

	var paginationMetadata = new PaginationMetadata(pagedList.TotalCount, pagedList.PageSize,
		pagedList.CurrentPage, pagedList.TotalPages, previousPageLink, nextPageLink);

	httpContext.Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));


	return pagedList.Select(owner => new OwnerDto(owner.Id, owner.Name, owner.LastName));
}).WithName("GetOwners");

ownersGroup.MapGet("/owners/{ownerId:int}", async (int ownerId, StoreDbContext dbContext) =>
{
	var owner = await dbContext.Owners.FirstOrDefaultAsync(o => o.Id == ownerId); ;

	if (owner == null)
	{
		return Results.NotFound();
	}
	return Results.Ok(new OwnerDto(owner.Id, owner.Name, owner.LastName));
}).WithName("GetOwner");

ownersGroup.MapPost("/owners", [Authorize(Roles = StoreRoles.StoreUser)] async ([Validate] CreateOwnerDto createOwnerDto, StoreDbContext dbContext, LinkGenerator linkGenerator, HttpContext httpContext) =>
{
	var owner = new Owner()
	{
		Name = createOwnerDto.Name,
		LastName = createOwnerDto.LastName,
		UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
	};

	dbContext.Owners.Add(owner);

	await dbContext.SaveChangesAsync();

	var links = CreateLinks(owner.Id, httpContext, linkGenerator);
	var ownerDto = new OwnerDto(owner.Id, owner.Name, owner.LastName);
	var resource = new ResourceDto<OwnerDto>(ownerDto, links.ToArray());

	return Results.Created($"/api/cities/{owner.Id:int}", resource);

}).WithName("CreateOwner");

ownersGroup.MapPut("/owners/{ownerId:int}", [Authorize(Roles = StoreRoles.StoreUser)]  async (int ownerId, [Validate] UpdateOwnerDto dto, HttpContext httpContext, StoreDbContext dbContext) =>
{
	var owner = await dbContext.Owners.FirstOrDefaultAsync(c => c.Id == ownerId);
	if (owner == null)
	{
		return Results.NotFound();
	}

	if(!httpContext.User.IsInRole(StoreRoles.Admin) 
		&& httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != owner.UserId)
	{
		return Results.Forbid();
	}



	owner.Name = dto.Name;
	owner.LastName = dto.LastName;
	dbContext.Update(owner);



	await dbContext.SaveChangesAsync();
	return Results.Ok(new OwnerDto(owner.Id, owner.Name, owner.LastName));

}).WithName("EditOwner");

ownersGroup.MapDelete("/owners/{ownerId:int}", [Authorize(Roles = StoreRoles.StoreUser)] async (int ownerId, StoreDbContext dbContext, HttpContext httpContext) =>
{
	var owner = await dbContext.Owners.FirstOrDefaultAsync(c => c.Id == ownerId);
	if (owner == null)
	{
		return Results.NotFound();
	}

	if (!httpContext.User.IsInRole(StoreRoles.Admin)
		&& httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != owner.UserId)
	{
		return Results.Forbid();
	}

	dbContext.Remove(owner);
	await dbContext.SaveChangesAsync();

	return Results.NoContent();
}).WithName("DeleteOwner");

IEnumerable<LinkDto> CreateLinks(int ownerid, HttpContext httpcontext, LinkGenerator linkGenerator)
{
	yield return new LinkDto(linkGenerator.GetUriByName(httpcontext, "GetOwner", new { ownerid }), "self", "GET");
	yield return new LinkDto(linkGenerator.GetUriByName(httpcontext, "EditOwner", new { ownerid }), "edit", "PUT");
	yield return new LinkDto(linkGenerator.GetUriByName(httpcontext, "DeleteOwner", new { ownerid }), "delete", "DELETE");
}
//----------OWNER----------

//----------SHOP-----------
var shopsGroup = app.MapGroup("/api/owners/{ownerId}").WithValidationFilter();

shopsGroup.MapGet("/shops", async (int OwnerId, StoreDbContext dbContext, CancellationToken cancellationToken) =>
{
	return (await dbContext.Shops.Include(owner => owner.owner).ToListAsync(cancellationToken))
			.Where(shop => shop.owner.Id == OwnerId)
			.Select(shop => new ShopDto(shop.Id, shop.Name, shop.Address, shop.owner.Id));
});

shopsGroup.MapGet("shops/{shopId:int}", async (int OwnerId, int shopId, StoreDbContext dbcontext) =>
{
	var owner = await dbcontext.Owners.FirstOrDefaultAsync(o => o.Id == OwnerId);
	var shop = await dbcontext.Shops.FirstOrDefaultAsync(s => s.Id == shopId && s.owner.Id == OwnerId);
	if (owner == null || shop == null)
	{
		return Results.NotFound();
	}

	return Results.Ok(new ShopDto(shop.Id, shop.Name, shop.Address, shop.owner.Id));
});

shopsGroup.MapPost("shops/", [Authorize(Roles = StoreRoles.StoreUser)] async (int OwnerId, [Validate] CreateShopDto createShopDto, StoreDbContext dbcontext, HttpContext httpContext) =>
{
	var owner = await dbcontext.Owners.FirstOrDefaultAsync(o => o.Id == OwnerId); ;
	if (owner == null)
	{
		return Results.NotFound();
	}

	if (!httpContext.User.IsInRole(StoreRoles.Admin)
		&& httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != owner.UserId)
	{
		return Results.Forbid();
	}

	var shop = new Shop()
	{
		Name = createShopDto.Name,
		Address = createShopDto.Address,
		owner = owner,
		UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
	};
	dbcontext.Shops.Add(shop);
	await dbcontext.SaveChangesAsync();

	return Results.Created($"/api/owners/{createShopDto.OwnerId:int}",
							new ShopDto(shop.Id, shop.Name, shop.Address, shop.owner.Id));
});

shopsGroup.MapPut("shops/{shopId:int}", [Authorize(Roles = StoreRoles.StoreUser)] async (int OwnerId, int shopId, [Validate] UpdateShopDto regionDto, StoreDbContext dbcontext, HttpContext httpContext) =>
{
	//Console.WriteLine(OwnerId + "   " + shopId);
	var owner = await dbcontext.Owners.FirstOrDefaultAsync(o => o.Id == OwnerId);
	var shop = await dbcontext.Shops.Include(o => o.owner).FirstOrDefaultAsync(s => s.Id == shopId && s.owner.Id == OwnerId);
	if (owner == null || shop == null)
	{
		return Results.NotFound();
	}

	if (!httpContext.User.IsInRole(StoreRoles.Admin)
		&& httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != owner.UserId)
	{
		return Results.Forbid();
	}

	shop.Name = regionDto.Name;
	shop.Address = regionDto.Address;
	await dbcontext.SaveChangesAsync();
	return Results.Ok(new ShopDto(shop.Id, shop.Name, shop.Address, OwnerId));
});

shopsGroup.MapDelete("shops/{shopId:int}", [Authorize(Roles = StoreRoles.StoreUser)]  async (int OwnerId, int shopId, StoreDbContext dbcontext, HttpContext httpContext) =>
{
	var owner = await dbcontext.Owners.FirstOrDefaultAsync(o => o.Id == OwnerId);
	var shop = await dbcontext.Shops.Include(o => o.owner).FirstOrDefaultAsync(s => s.Id == shopId && s.owner.Id == OwnerId);
	if (owner == null || shop == null)
	{
		return Results.NotFound();
	}

	if (!httpContext.User.IsInRole(StoreRoles.Admin)
		&& httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != owner.UserId)
	{
		return Results.Forbid();
	}

	dbcontext.Remove(shop);
	await dbcontext.SaveChangesAsync();
	return Results.NoContent();
});

//----------SHOP-----------

//----------ITEM-----------
var itemGroup = app.MapGroup("/api/owners/{ownerId}/shops/{shopId:int}").WithValidationFilter();

itemGroup.MapGet("/items", async (int shopId, StoreDbContext dbContext, CancellationToken cancellationToken) =>
{
	return (await dbContext.Items.Include(item => item.shop).ToListAsync(cancellationToken))
			.Where(item => item.shop.Id == shopId)
			.Select(item => new ItemDto(item.Id, item.Name, item.Description, item.Picture, item.shop.Id));
});

itemGroup.MapGet("items/{itemId:int}", async (int ownerId, int shopId, int itemId, StoreDbContext dbcontext) =>
{
	var owner = await dbcontext.Owners.FirstOrDefaultAsync(o => o.Id == ownerId);
	var shop = await dbcontext.Shops.FirstOrDefaultAsync(s => s.Id == shopId && s.owner.Id == ownerId);
	var item = await dbcontext.Items.FirstOrDefaultAsync(i => i.Id == itemId && i.shop.Id == shopId && i.shop.owner.Id == ownerId);
	if (owner == null || shop == null || item == null)
	{
		return Results.NotFound();
	}

	return Results.Ok(new ItemDto(item.Id, item.Name, item.Description, item.Picture, item.shop.Id));
});

itemGroup.MapPost("items/", [Authorize(Roles = StoreRoles.StoreUser)] async (int shopId, [Validate] CreateItemDto createItemDto, StoreDbContext dbcontext, HttpContext httpContext) =>
{
	var shop = await dbcontext.Shops.Include(shop => shop.owner).FirstOrDefaultAsync(o => o.Id == shopId);
	
	var item = new Item()
	{
		Name = createItemDto.Name,
		Description = createItemDto.Description,
		Picture = createItemDto.Picture,
		shop = shop,
		UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
	};

	if (!httpContext.User.IsInRole(StoreRoles.Admin)
		&& httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != shop.owner.UserId)
	{
		return Results.Forbid();
	}

	dbcontext.Items.Add(item);
	await dbcontext.SaveChangesAsync();

	return Results.Created($"/api/cities/{createItemDto.Id:int}",
							new ItemDto(item.Id, item.Name, item.Description, item.Picture, item.shop.Id));
});

itemGroup.MapPut("items/{itemId:int}", [Authorize(Roles = StoreRoles.StoreUser)] async (int ownerId, int shopId, int itemId, [Validate] CreateItemDto createItemDto, StoreDbContext dbcontext, HttpContext httpContext) =>
{
	var owner = await dbcontext.Owners.FirstOrDefaultAsync(o => o.Id == ownerId);
	var shop = await dbcontext.Shops.FirstOrDefaultAsync(s => s.Id == shopId && s.owner.Id == ownerId);
	var item = await dbcontext.Items.FirstOrDefaultAsync(i => i.Id == itemId && i.shop.Id == shopId && i.shop.owner.Id == ownerId);

	if (owner == null || shop == null || item == null)
	{
		return Results.NotFound();
	}

	if (!httpContext.User.IsInRole(StoreRoles.Admin)
		&& httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != owner.UserId)
	{
		return Results.Forbid();
	}

	item.Name = createItemDto.Name;
	item.Description = createItemDto.Description;
	item.Picture = createItemDto.Picture;

	await dbcontext.SaveChangesAsync();
	return Results.Ok(new ItemDto(item.Id, item.Name, item.Description, item.Picture, item.shop.Id));
});

itemGroup.MapDelete("items/{itemId:int}", [Authorize(Roles = StoreRoles.StoreUser)] async (int ownerId, int shopId, int itemId, StoreDbContext dbcontext, HttpContext httpContext) =>
{
	var owner = await dbcontext.Owners.FirstOrDefaultAsync(o => o.Id == ownerId);
	var shop = await dbcontext.Shops.FirstOrDefaultAsync(s => s.Id == shopId && s.owner.Id == ownerId);
	var item = await dbcontext.Items.FirstOrDefaultAsync(i => i.Id == itemId && i.shop.Id == shopId && i.shop.owner.Id == ownerId);

	if (owner == null || shop == null || item == null)//owner == null || shop == null ||
	{
		return Results.NotFound();
	}

	if (!httpContext.User.IsInRole(StoreRoles.Admin)
		&& httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != owner.UserId)
	{
		return Results.Forbid();
	}

	dbcontext.Remove(item);
	await dbcontext.SaveChangesAsync();
	return Results.NoContent();
});

//----------ITEM-----------

app.AddAuthApi();

app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();

var dbSeeder = scope.ServiceProvider.GetRequiredService<AuthDbSeeder>();

await dbSeeder.SeedAsync();

app.Run();

//----------OWNER----------
public record CreateOwnerDto(string Name, string LastName);
public record UpdateOwnerDto(string Name, string LastName);
public class CreateOwnerDtoValidator : AbstractValidator<CreateOwnerDto>
{
	public CreateOwnerDtoValidator()
	{
		RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(min: 2, max: 100);
		RuleFor(dto => dto.LastName).NotEmpty().NotNull().Length(min: 2, max: 100);
	}
}

public class UpdateOwnerDtoValidator : AbstractValidator<UpdateOwnerDto>
{
	public UpdateOwnerDtoValidator()
	{
		RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(min: 2, max: 100);
		RuleFor(dto => dto.LastName).NotEmpty().NotNull().Length(min: 2, max: 100);
	}
}
//----------OWNER----------

//----------SHOP-----------
public record CreateShopDto(string Name, string Address, int OwnerId);
public record UpdateShopDto(string Name, string Address);
public class CreateShopDtoValidator : AbstractValidator<CreateShopDto>
{
	public CreateShopDtoValidator()
	{
		RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(min: 2, max: 100);
		RuleFor(dto => dto.Address).NotEmpty().NotNull().Length(min: 2, max: 100);
	}
}

public class UpdateShopDtoValidator : AbstractValidator<UpdateShopDto>
{
	public UpdateShopDtoValidator()
	{
		RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(min: 2, max: 100);
		RuleFor(dto => dto.Address).NotEmpty().NotNull().Length(min: 2, max: 100);
	}
}
//----------SHOP-----------

//----------ITEM-----------
public record CreateItemDto(int Id, string Name, string Description, string Picture, int ShopId);
public record UpdateItemDto(string Name, string Description, string Picture);
public class CreateItemDtoValidator : AbstractValidator<CreateItemDto>
{
	public CreateItemDtoValidator()
	{
		
		RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(min: 2, max: 100);
		RuleFor(dto => dto.Description).NotEmpty().NotNull().Length(min: 2, max: 200);
		RuleFor(dto => dto.Picture).NotEmpty().NotNull().Length(min: 2, max: 100);
	}
}

public class UpdateItemDtoValidator : AbstractValidator<UpdateItemDto>
{
	public UpdateItemDtoValidator()
	{
		RuleFor(dto => dto.Name).NotEmpty().NotNull().Length(min: 2, max: 100);
		RuleFor(dto => dto.Description).NotEmpty().NotNull().Length(min: 2, max: 200);
		RuleFor(dto => dto.Picture).NotEmpty().NotNull().Length(min: 2, max: 100);
	}
}
//----------ITEM-----------