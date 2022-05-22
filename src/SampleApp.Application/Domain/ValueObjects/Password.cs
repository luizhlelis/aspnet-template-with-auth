namespace SampleApp.Application.Domain.ValueObjects;

public class Password
{
    public Password(string password)
    {
        PlainText = password;
        Hash = BCrypt.Net.BCrypt.HashPassword(password);
    }

    public string Hash { get; }

    public string PlainText { get; }

    public bool HasMatchWith(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, Hash);
    }

    public bool HasHashMatchWith(string hash)
    {
        return BCrypt.Net.BCrypt.Verify(PlainText, hash);
    }
}
