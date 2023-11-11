//using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using StoreTrack.Auth.Model;

namespace StoreTrack.Auth
{
	public static class AuthEndpoints
	{
		public static void AddAuthApi(this WebApplication app)
		{
			// register
			app.MapPost("api/register", async (UserManager<StoreRestUser> userManager, RegisterUserDto registerUserDto) =>
			{
				var user = await userManager.FindByNameAsync(registerUserDto.Username);
				if(user != null)
				{
					return Results.UnprocessableEntity("User name already taken");
				}
				
				var newUser = new StoreRestUser
				{
					Email = registerUserDto.Email,
					UserName = registerUserDto.Username
				};

				var createUserResult = await userManager.CreateAsync(newUser, registerUserDto.Password);

				if(!createUserResult.Succeeded)
				{
					return Results.UnprocessableEntity("ERROR");
				}

				

				await userManager.AddToRoleAsync(newUser, StoreRoles.StoreUser);

				return Results.Created("api/login", new UserDto(newUser.Id, newUser.UserName, newUser.Email));
			});

			// login
			app.MapPost("api/login", async (UserManager<StoreRestUser> userManager, JwtTokenService jwtTokenService, LoginDto loginDto) =>
			{
				var user = await userManager.FindByNameAsync(loginDto.Username);
				if (user == null)
				{
					return Results.UnprocessableEntity("User name or password was incorrect.");
				}

				var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);
				if (!isPasswordValid)
				{
					return Results.UnprocessableEntity("User name or password was incorrect.");
				}

				user.ForceRelogin = false;
				await userManager.UpdateAsync(user);

				var roles = await userManager.GetRolesAsync(user);

				var accesToken = jwtTokenService.CreateAccesToken(user.UserName, user.Id, roles);

				var refreshToken = jwtTokenService.CreateRefreshToken(user.Id);





				return Results.Ok(new SuccessfulLoginDto(accesToken, refreshToken));
			});


			//access token
			app.MapPost("api/accessToken", 
				async (UserManager<StoreRestUser> userManager, JwtTokenService jwtTokenService, RefreshAccessTokenDto refreshAccessTokenDto) =>
			{
				if(!jwtTokenService.TryParseRefreshToken(refreshAccessTokenDto.RefreshToken, out var claims))
				{
					return Results.UnprocessableEntity();
				}


				var userId = claims.FindFirstValue(JwtRegisteredClaimNames.Sub);

				var user = await userManager.FindByIdAsync(userId);

				if(user == null)
				{
					return Results.UnprocessableEntity("Invalid token");
				}

				if(user.ForceRelogin)
				{
					return Results.UnprocessableEntity();
				}

				var roles = await userManager.GetRolesAsync(user);

				var accesToken = jwtTokenService.CreateAccesToken(user.UserName, user.Id, roles);

				var refreshToken = jwtTokenService.CreateRefreshToken(user.Id);


				return Results.Ok(new SuccessfulLoginDto(accesToken, refreshToken));


			});

		}

		public record SuccessfulLoginDto(string AccesToken, string RefreshToken);
		public record UserDto(string UserId, string UserName, string Email);
		public record RegisterUserDto(string Username, string Email, string Password);

		public record LoginDto(string Username, string Password);

		public record RefreshAccessTokenDto(string RefreshToken);







	}
}
