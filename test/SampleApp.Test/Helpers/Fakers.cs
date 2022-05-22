using Bogus;
using SampleApp.Application.Domain.Entities;

namespace SampleApp.Test.Helpers;

public static class Fakers
{
    public static Product GetValidProduct()
    {
        var faker = new Faker<Product>("en_US")
            .RuleFor(product => product.Name, faker => faker.Person.FirstName)
            .RuleFor(product => product.Price, faker => faker.Random.Double())
            .RuleFor(product => product.AmountAvailable, faker => faker.Random.Int())
            .RuleFor(movie => movie.AmountAvailable, 10);

        return faker.Generate();
    }
}
