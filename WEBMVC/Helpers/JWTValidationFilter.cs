using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace WEBMVC.Helpers
{
    public class JwtValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var jwtToken = context.HttpContext.Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);

                // Set claims to HttpContext
                context.HttpContext.User = new System.Security.Claims.ClaimsPrincipal(
                    new System.Security.Claims.ClaimsIdentity(token.Claims, "jwt")
                );
            }
            catch
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}
