using SampleApp.Application.Domain.Entities;
using SampleApp.Application.Dtos;

namespace SampleApp.Application.Ports;

public interface IDatabaseDrivenPort
{
    Task<User> AddUserAsync(User user);

    Task<User> UpdateUserAsync(string username, UpdateUserDto user);

    Task<User?> GetUserAsync(string username);

    Task<User> DeleteUserAsync(User user);
}
