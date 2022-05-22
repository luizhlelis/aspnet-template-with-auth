using FluentValidation;
using Microsoft.AspNetCore.Http;
using SampleApp.Application.Dtos;
using SampleApp.Application.Ports;

namespace SampleApp.Application.Domain.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator(IDatabaseDrivenPort databaseDrivenPort,
        IHttpContextAccessor contextAccessor)
    {
        var claim = contextAccessor.HttpContext?.User.FindFirst(
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        var username = claim?.Value ?? "";

        RuleFor(user => user.Address)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(user => user.GivenName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(user => user.ZipCode)
            .NotEmpty()
            .Matches(@"\d{5}(?:[-\s]\d{4})?");

        RuleFor(user => user.Password)
            .MinimumLength(8)
            .WithMessage("The length of Password must be at least 8 characters")
            .Matches("[A-Z]")
            .WithMessage("Password must have at least one uppercase letter")
            .Matches("[0-9]")
            .WithMessage("Password must have at least one number")
            .Matches("[!@#$&*]")
            .WithMessage("Password must have at least one special character: !@#$&*");

        RuleFor(user => user)
            .MustAsync(HasAlreadyBeenRegistered)
            .WithMessage("User not found")
            .WithErrorCode(ErrorCode.NotFound);

        async Task<bool> HasAlreadyBeenRegistered(UpdateUserDto userDto,
            CancellationToken cancellationToken)
        {
            var user = await databaseDrivenPort.GetUserAsync(username);
            return user is not null;
        }
    }
}
