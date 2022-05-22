using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.Infrastructure;
using SampleApp.Web;

namespace SampleApp.Test.Helpers;

public class IntegrationTestFixture : IDisposable
{
    protected HttpClient Client;
    protected SampleAppContext DbContext;
    protected IServiceScope TestServiceScope;

    public IntegrationTestFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, false)
            .AddEnvironmentVariables()
            .Build();

        var server = new TestServer(new WebHostBuilder()
            .UseConfiguration(configuration)
            .UseStartup<Startup>()
            .ConfigureTestServices(services =>
            {
                services.AddSingleton<IConfiguration>(configuration);
                // remove the existing context configuration
                var descriptor = services.SingleOrDefault(service =>
                    service.ServiceType == typeof(DbContextOptions<SampleAppContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
                services.AddDbContext<SampleAppContext>(options =>
                    options.UseInMemoryDatabase("SampleApp"));
            })
        );

        TestServiceScope = server.Host.Services.CreateScope();
        DbContext = TestServiceScope.ServiceProvider.GetRequiredService<SampleAppContext>();

        Client = server.CreateClient();
        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        Client.DefaultRequestHeaders.Remove("Authorization");
        // DbContext.Products.Clear();
        DbContext.SaveChanges();
    }
}

public static class EntityExtensions
{
    public static void Clear<T>(this DbSet<T> dbSet) where T : class
    {
        dbSet.RemoveRange(dbSet);
    }
}
