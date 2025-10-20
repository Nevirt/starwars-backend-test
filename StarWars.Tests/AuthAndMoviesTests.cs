using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarWars.Api;
using Microsoft.EntityFrameworkCore;
using StarWars.Api.Data;

namespace StarWars.Tests;

public class CustomFactory : WebApplicationFactory<Program>
{
	protected override IHost CreateHost(IHostBuilder builder)
	{
		builder.UseEnvironment("Development");
		return base.CreateHost(builder);
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureServices(services =>
		{
			using var sp = services.BuildServiceProvider();
			using var scope = sp.CreateScope();
			var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			// Apply migrations so test DB has the required tables
			db.Database.Migrate();
		});
	}
}

public class AuthAndMoviesTests : IClassFixture<CustomFactory>
{
	private readonly HttpClient _client;

	public AuthAndMoviesTests(CustomFactory factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task Signup_and_Login_returns_token()
	{
		var email = $"test{Guid.NewGuid():N}@mail.com";
		var signup = await _client.PostAsJsonAsync("/api/auth/signup", new { email, password = "Passw0rd!", role = "Admin" });
		signup.EnsureSuccessStatusCode();
		var signupPayload = await signup.Content.ReadFromJsonAsync<AuthResponseDto>();
		signupPayload!.Token.Should().NotBeNullOrEmpty();

		var login = await _client.PostAsJsonAsync("/api/auth/login", new { email, password = "Passw0rd!" });
		login.EnsureSuccessStatusCode();
		var loginPayload = await login.Content.ReadFromJsonAsync<AuthResponseDto>();
		loginPayload!.Token.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public async Task Movies_CRUD_and_Authorization()
	{
		var email = $"user{Guid.NewGuid():N}@mail.com";
		var signup = await _client.PostAsJsonAsync("/api/auth/signup", new { email, password = "Passw0rd!", role = "Admin" });
		var auth = await signup.Content.ReadFromJsonAsync<AuthResponseDto>();
		_client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.Token);

		var create = await _client.PostAsJsonAsync("/api/movies", new { title = "A New Hope", description = "", releaseYear = 1977, director = "Lucas", producer = "Fox" });
		create.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
		var created = await create.Content.ReadFromJsonAsync<MovieDto>();

		var getAnon = await _client.GetAsync("/api/movies");
		getAnon.EnsureSuccessStatusCode();

		var getByIdNoAuthClient = new CustomFactory().CreateClient();
		var getByIdNoAuth = await getByIdNoAuthClient.GetAsync($"/api/movies/{created!.Id}");
		getByIdNoAuth.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);

		// Admin token should not allow GetById (regular users only)
		var getByIdAsAdmin = await _client.GetAsync($"/api/movies/{created.Id}");
		getByIdAsAdmin.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);

		var update = await _client.PutAsJsonAsync($"/api/movies/{created.Id}", new { title = "A New Hope (Remastered)", description = (string?)null, releaseYear = 1977, director = "Lucas", producer = "Fox" });
		update.EnsureSuccessStatusCode();

		var delete = await _client.DeleteAsync($"/api/movies/{created.Id}");
		delete.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
	}

	private sealed record AuthResponseDto(string Token, string Email, string Role);
	private sealed record MovieDto(int Id, string Title, string? Description, int? ReleaseYear, string? Director, string? Producer, string? ExternalId);
}
