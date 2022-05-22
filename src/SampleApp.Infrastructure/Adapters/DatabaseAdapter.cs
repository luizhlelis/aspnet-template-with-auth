using Microsoft.EntityFrameworkCore;
using SampleApp.Application.Domain.Entities;
using SampleApp.Application.Dtos;
using SampleApp.Application.Ports;

namespace SampleApp.Infrastructure.Adapters;

// Database Adapter: implements the driven port
public class DatabaseAdapter : IDatabaseDrivenPort
{
    private readonly SampleAppContext _dbContext;

    public DatabaseAdapter(SampleAppContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> AddUserAsync(User user)
    {
        var entry = await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<User> UpdateUserAsync(string username, UpdateUserDto userDto)
    {
        var entry = await _dbContext.Users.FirstAsync(user => user.Username == username);
        entry.Bind(userDto);
        await _dbContext.SaveChangesAsync();
        return entry;
    }

    public async Task<User?> GetUserAsync(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(user => user.Username == username);
    }

    public async Task<User> DeleteUserAsync(User user)
    {
        var entry = _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return entry.Entity;
    }
}
