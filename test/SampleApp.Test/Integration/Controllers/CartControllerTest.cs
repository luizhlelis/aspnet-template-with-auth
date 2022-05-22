using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SampleApp.Application.Domain.Entities;
using SampleApp.Application.Domain.ValueObjects;
using SampleApp.Application.Dtos;
using SampleApp.Test.Helpers;
using Xunit;

namespace SampleApp.Test.Integration.Controllers;

public class CartControllerTest : IntegrationTestFixture
{
    private const string CartPath = "v1/cart";
    private readonly IDistributedCache _cache;
    private readonly Authentication _auth;

    public CartControllerTest()
    {
        _auth = TestServiceScope.ServiceProvider.GetRequiredService<Authentication>();
        _cache = TestServiceScope.ServiceProvider.GetRequiredService<IDistributedCache>();
    }

    [Fact(DisplayName = "Should return created when cart doesn't exist yet")]
    public async Task PostCart_WhenCartDoesNotExist_ShouldReturnCreated()
    {
        // arrange
        var username = "cart-controller-0";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        // act
        var response = await Client.PostAsync(CartPath, null);

        // assert
        response.Should().Be201Created().And.BeAs(new Cart());
    }

    [Fact(DisplayName = "Should return bad request when cart already exists")]
    public async Task PostCart_WhenCartAlreadyExists_ShouldReturnBadRequest()
    {
        // arrange
        var username = "cart-controller-1";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        await _cache.SetAsync(username, new Cart().ToBytes());

        // act
        var response = await Client.PostAsync(CartPath, null);

        // assert
        response.Should().Be400BadRequest().And.BeAs(new
        {
            type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            title = "BAD_REQUEST_ERROR",
            status = 400,
            traceId = "0HMH5DLVSLJDP",
            error = new
            {
                msg = "There is another cart registered, please delete it first"
            }
        },
            options => options.Excluding(source => source.traceId));
    }

    [Fact(DisplayName = "Should return ok with cart when cart exists")]
    public async Task GetCart_WhenCartExists_ShouldReturnOk()
    {
        // arrange
        var username = "cart-controller-2";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        var registeredCart = new Cart();
        await _cache.SetAsync(username, registeredCart.ToBytes());

        // act
        var response = await Client.GetAsync(CartPath);

        // assert
        response.Should().Be200Ok().And.BeAs(registeredCart);
    }

    [Fact(DisplayName = "Should return not found when cart does not exist")]
    public async Task GetCart_WhenCartDoesNotExist_ShouldReturnNotFound()
    {
        // arrange
        var username = "cart-controller-3";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        // act
        var response = await Client.GetAsync(CartPath);

        // assert
        response.Should().Be404NotFound().And.BeAs(new
        {
            type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
            title = "NOT_FOUND_ERROR",
            status = 404,
            traceId = "0HMH5DLVSLJDP",
            error = new
            {
                msg = "There is no cart registered."
            }
        },
            options => options.Excluding(source => source.traceId));
    }

    [Fact(DisplayName =
        "Should return no content and add movie to cart when movie exists and is available")]
    public async Task PutCart_WhenMovieExistsAndAvailable_ShouldReturnNoContent()
    {
        // arrange
        var username = "cart-controller-4";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        await DbContext.Users.AddAsync(user);
        var movie = new Movie("Titanic", "Boat in sea", 1994, 5);
        movie.RentCategory = new RentCategory("FakeRent", 2);
        await DbContext.Movies.AddAsync(movie);
        await DbContext.SaveChangesAsync();
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        var registeredCart = new Cart();
        await _cache.SetAsync(username, registeredCart.ToBytes());

        var content = new StringContent(
            JsonConvert.SerializeObject(new CartDto { MovieId = movie.Id }),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PutAsync(CartPath, content);

        // assert
        response.Should().Be204NoContent();
    }

    [Fact(DisplayName =
        "Should return not found when movie is not available to rent")]
    public async Task PutCart_WhenMovieNotAvailable_ShouldReturnNotFound()
    {
        // arrange
        var username = "cart-controller-5";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        await DbContext.Users.AddAsync(user);
        var movie = new Movie("Titanic 2", "Boat in sea", 1994, 0);
        movie.RentCategory = new RentCategory("FakeRent2", 2);
        await DbContext.Movies.AddAsync(movie);
        await DbContext.SaveChangesAsync();
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        var registeredCart = new Cart();
        await _cache.SetAsync(username, registeredCart.ToBytes());

        var content = new StringContent(
            JsonConvert.SerializeObject(new CartDto { MovieId = movie.Id }),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PutAsync(CartPath, content);

        // assert
        response.Should().Be404NotFound().And.BeAs(
            new
            {
                type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                title = "NOT_FOUND_ERROR",
                status = 404,
                traceId = "0HMH5DLVSLJDP",
                error = new
                {
                    msg = "Movie not found or not available to rent"
                }
            },
            options => options.Excluding(source => source.traceId));
    }
}
