using Microsoft.AspNetCore.Mvc;

namespace SampleApp.Web.Controllers;

public abstract class BaseController : ControllerBase
{
    protected string GetUsernameFromToken()
    {
        var username =
            User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

        return username?.Value ?? "";
    }
}
