namespace SampleApp.Application.Dtos;

/*  Resource Owner Password Credentials grant type was deprecated by OAuth2.0,
    this should be avoided in a production environment */

public class OwnerCredentialDto
{
    public string Username { get; set; }

    public string Password { get; set; }
}
