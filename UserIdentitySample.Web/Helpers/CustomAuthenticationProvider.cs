using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

using System.Security.Claims;

namespace UserIdentitySample.Web.Helpers
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider
    {
        public readonly ClaimsPrincipal _user;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            _user = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "ATuaMae"),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.NameIdentifier, "007"),
                }
            , IdentityConstants.ApplicationScheme
                ));
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //return await Task.FromResult(new AuthenticationState(_user));
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var user = httpContext.User; // Fetch the ClaimsPrincipal from the cookie
                return await Task.FromResult(new AuthenticationState(user));
            }

            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }

        public void SignIn()
        {
            //var identity = new ClaimsIdentity(new[]
            //    {
            //        new Claim(ClaimTypes.Name, "ATuaMae"),
            //        new Claim(ClaimTypes.Role, "Admin"),
            //        new Claim(ClaimTypes.NameIdentifier, "007")
            //    }, IdentityConstants.ApplicationScheme);


            //var principal = new ClaimsPrincipal(identity);
            //var state = new AuthenticationState(principal);
            //NotifyAuthenticationStateChanged(Task.FromResult(state));
        }

        public void SignOut()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            var state = new AuthenticationState(anonymous);
            NotifyAuthenticationStateChanged(Task.FromResult(state));
        }

        public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var user = httpContext.User; // Fetch the ClaimsPrincipal from the cookie
                var state = new AuthenticationState(user);
                NotifyAuthenticationStateChanged(Task.FromResult(state));
            }
        }
    }

}
