using System;
using System.Web;
using System.Web.Mvc;

public class CustomAuthorizeAttribute : AuthorizeAttribute
{
    private readonly string _loginUrl;

    public CustomAuthorizeAttribute(string loginUrl)
    {
        _loginUrl = loginUrl;
    }

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
        {
            filterContext.Result = new RedirectResult(_loginUrl);
        }
        else
        {
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}
