using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SampleApp.Application.Domain.Entities;
using SampleApp.Application.Domain.Enums;
using SampleApp.Application.Domain.ValueObjects;
using SampleApp.Application.Dtos;
using SampleApp.Test.Helpers;
using Xunit;

namespace SampleApp.Test.Integration.Controllers;

public class UserControllerTest : IntegrationTestFixture
{
    private const string UserPath = "v1/user";
    private const string ValidPassword = "StrongPassword@123";
    private readonly Authentication _auth;

    public UserControllerTest()
    {
        _auth = TestServiceScope.ServiceProvider.GetRequiredService<Authentication>();
    }

    [Fact(DisplayName = "Should return created when user doesn't exist yet")]
    public async Task PostUser_WhenUserDoesNotExist_ShouldReturnCreated()
    {
        // arrange
        var requestBody = new UserDto
        {
            Username = "user-controller-1",
            Password = ValidPassword,
            ZipCode = "12345",
            Address = "1458 Sauer Courts Suite 328",
            GivenName = "John Doe"
        };
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(UserPath, content);

        // assert
        response.Should().Be201Created().And.BeAs(new
        {
            username = requestBody.Username
        });
    }

    [Fact(DisplayName = "Should return not found on get user when user doesn't exist")]
    public async Task GetUser_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // arrange
        var user = new User("get-user-admin-requester", ValidPassword, "12345",
            "1458 Sauer Courts Suite 328", "John Doe", Role.Admin);
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
        var accessToken = _auth.GenerateAccessToken(user.Username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        // act
        var response = await Client.GetAsync($"{UserPath}/fake-username");

        // assert
        response.Should().Be404NotFound().And.BeAs(new
        {
            type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
            title = "NOT_FOUND_ERROR",
            status = 404,
            traceId = "0HMH5DLVSLJDP",
            error = new
            {
                msg = "User not found"
            }
        },
            options => options.Excluding(source => source.traceId));
    }

    [Fact(DisplayName = "Should return ok on me when authorized")]
    public async Task GetUserMe_WhenAuthorizedUser_ShouldReturnOk()
    {
        // arrange
        var username = "get-user-me";
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        await DbContext.Users.AddAsync(new User(username, ValidPassword,
            "1458 Sauer Courts Suite 328", "John Doe", "12345"));
        await DbContext.SaveChangesAsync();

        // act
        var response = await Client.GetAsync($"{UserPath}/me");

        // assert
        response.Should().Be200Ok().And.BeAs(new { username });
    }

    [Fact(DisplayName = "Should delete and return ok on me when authorized user")]
    public async Task DeleteUserMe_WhenAuthorizedUser_ShouldReturnOk()
    {
        // arrange
        var username = "delete-user-me";
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        await DbContext.Users.AddAsync(new User(username, ValidPassword,
            "1458 Sauer Courts Suite 328", "John Doe", "12345"));
        await DbContext.SaveChangesAsync();

        // act
        var response = await Client.DeleteAsync($"{UserPath}/me");

        // assert
        response.Should().Be200Ok();
        var searchResult =
            await DbContext.Users.FirstOrDefaultAsync(user => user.Username == username);
        searchResult.Should().BeNull();
    }

    [Fact(DisplayName = "Should return unauthorized on post when user is not admin")]
    public async Task PostAdminUser_WhenUserIsNotAdmin_ShouldReturnUnauthorized()
    {
        // arrange
        var requestBody = new UserDto
        {
            Username = "post-user-admin",
            Password = ValidPassword,
            Address = "1458 Sauer Courts Suite 328",
            GivenName = "John Doe"
        };
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");
        var accessToken = _auth.GenerateAccessToken(requestBody.Username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        // act
        var response = await Client.PostAsync($"{UserPath}/admin", content);

        // assert
        response.Should().Be401Unauthorized();
    }

    [Fact(DisplayName = "Should return no content on put user when user exists")]
    public async Task PutUser_WhenUserExists_ShouldReturnNoContent()
    {
        // arrange
        var user = new User("put-user", ValidPassword, "12345", "1458 Sauer Courts Suite 328",
            "John Doe");
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();

        var requestBody = new UpdateUserDto
        {
            Address = "53641 Maximilian Harbors",
            GivenName = "John Doe Jr",
            Password = "StrongPass@123",
            ZipCode = "53641 Maximilian Harbors"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");
        var accessToken = _auth.GenerateAccessToken(user.Username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        // act
        var response = await Client.PutAsync(UserPath, content);

        // assert
        response.Should().Be204NoContent();
    }

    [Fact(DisplayName = "Should return not found on put user when user not found")]
    public async Task PutUser_WhenUserNotFound_ShouldReturnNotFound()
    {
        // arrange
        var requestBody = new UpdateUserDto
        {
            Address = "53641 Maximilian Harbors",
            GivenName = "John Doe Jr",
            Password = "StrongPass@123",
            ZipCode = "53641 Maximilian Harbors"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");
        var accessToken = _auth.GenerateAccessToken("put-user-not-found");
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        // act
        var response = await Client.PutAsync(UserPath, content);

        // assert
        response.Should().Be404NotFound().And.BeAs(new
        {
            type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
            title = "NOT_FOUND_ERROR",
            status = 404,
            traceId = "",
            error = new
            {
                msg = "User not found"
            }
        }, options => options.Excluding(source => source.traceId));
    }
}
