using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using SampleApp.Application.Domain.Entities;
using SampleApp.Test.Helpers;
using Xunit;

namespace SampleApp.Test.Integration.Controllers;

public class AuthenticationControllerTest : IntegrationTestFixture
{
    private const string PostTokenPath = "v1/authentication/token";
    private const string ValidPassword = "StrongPassword@123";

    [Fact(DisplayName = "Should return ok with token when valid owner credential")]
    public async Task PostToken_WhenValidOwnerCredential_ShouldReturnOkWithToken()
    {
        var user = new User("valid-username-1", ValidPassword, "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();

        // arrange
        var requestBody = new { user.Username, Password = ValidPassword };
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(PostTokenPath, content);

        // assert
        response.Should().Be200Ok();
    }

    [Theory(DisplayName = "Should return bad request when empty or null username")]
    [InlineData(null)]
    [InlineData("")]
    public async Task PostToken_WhenEmptyUsername_ShouldReturnBadRequest(string username)
    {
        // arrange
        var requestBody = new { Username = username, Password = "ValidPwd@123" };
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(PostTokenPath, content);

        // assert
        response.Should().Be400BadRequest();
    }

    [Theory(DisplayName = "Should return bad request when empty or null password")]
    [InlineData(null)]
    [InlineData("")]
    public async Task PostToken_WhenEmptyPassword_ShouldReturnBadRequest(string password)
    {
        // arrange
        var requestBody = new { Username = "valid-username", Password = password };
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(PostTokenPath, content);

        // assert
        response.Should().Be400BadRequest();
    }

    [Fact(DisplayName = "Should return forbidden when there is no user with the incoming username")]
    public async Task PostToken_WhenUserNotFound_ShouldReturnForbidden()
    {
        // arrange
        var requestBody = new { Username = "user-forbidden", Password = "Forbidden@123" };
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(PostTokenPath, content);

        // assert
        response.Should().Be403Forbidden().And.BeAs(new
        {
            type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
            title = "FORBIDDEN_ERROR",
            status = 403,
            traceId = "0HMH5DLVSLJDP",
            error = new
            {
                msg = "User or password mismatch"
            }
        },
            options => options.Excluding(source => source.traceId)
        );
    }

    [Fact(DisplayName = "Should return forbidden when password mismatch")]
    public async Task PostToken_WhenPasswordMismatch_ShouldReturnForbidden()
    {
        var user = new User("valid-username-4", ValidPassword, "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();

        // arrange
        var requestBody = new { user.Username, Password = "Fake@123" };
        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(PostTokenPath, content);

        // assert
        response.Should().Be403Forbidden().And.BeAs(new
        {
            type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
            title = "FORBIDDEN_ERROR",
            status = 403,
            traceId = "0HMH5DLVSLJDP",
            error = new
            {
                msg = "User or password mismatch"
            }
        },
            options => options.Excluding(source => source.traceId));
    }
}
