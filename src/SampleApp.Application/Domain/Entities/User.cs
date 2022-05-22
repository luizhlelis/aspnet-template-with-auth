using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using SampleApp.Application.Domain.Enums;
using SampleApp.Application.Domain.ValueObjects;
using SampleApp.Application.Dtos;

namespace SampleApp.Application.Domain.Entities;

public class User
{
    public User(string username, string password, string zipCode, string address, string givenName,
        Role role = Role.Customer)
    {
        Username = username;
        ZipCode = zipCode;
        Address = address;
        GivenName = givenName;
        Password = new Password(password);
        PasswordHash = Password.Hash;
        Role = role;
    }

    public User(UserDto userDto, Role role = Role.Customer)
    {
        Username = userDto.Username;
        ZipCode = userDto.ZipCode;
        Address = userDto.Address;
        GivenName = userDto.GivenName;
        Password = new Password(userDto.Password);
        PasswordHash = Password.Hash;
        Role = role;
    }

    // Empty constructor required for EF
    public User() { }

    [Key][Required][MaxLength(20)] public string Username { get; private set; }

    [Required][MaxLength(10)] public string ZipCode { get; private set; }

    [Required][MaxLength(150)] public string Address { get; private set; }

    [Required][MaxLength(50)] public string GivenName { get; private set; }

    [Required][JsonIgnore] public string PasswordHash { get; private set; }

    [Required][JsonIgnore] public Role Role { get; private set; }

    [NotMapped][JsonIgnore] private Password Password;

    public void Bind(UpdateUserDto userDto)
    {
        ZipCode = userDto.ZipCode;
        Address = userDto.Address;
        GivenName = userDto.GivenName;
        Password = new Password(userDto.Password);
        PasswordHash = Password.Hash;
    }
}
