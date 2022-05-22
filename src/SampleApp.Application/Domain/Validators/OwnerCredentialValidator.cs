using FluentValidation;
using SampleApp.Application.Dtos;
using SampleApp.Application.Domain.ValueObjects;
using SampleApp.Application.Ports;

namespace SampleApp.Application.Domain.Validators;

public class OwnerCredentialValidator : AbstractValidator<OwnerCredentialDto>
{
    public OwnerCredentialValidator(IDatabaseDrivenPort databaseDrivenPort)
    {
        RuleFor(credentials => credentials.Username)
            .NotEmpty();

        RuleFor(credentials => credentials.Password)
            .NotEmpty();

        RuleFor(credentials => credentials)
            .MustAsync(AreCredentialsValid)
            .WithMessage("User or password mismatch")
            .WithErrorCode(ErrorCode.Forbidden);

        async Task<bool> AreCredentialsValid(OwnerCredentialDto credentials,
            CancellationToken cancellationToken)
        {
            var incomingPassword = new Password(credentials.Password);
            var user = await databaseDrivenPort.GetUserAsync(credentials.Username);

            return user is not null && incomingPassword.HasHashMatchWith(user.PasswordHash);
        }
    }
}
