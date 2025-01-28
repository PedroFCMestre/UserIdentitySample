using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Components.Authorization;

using System.Security.Claims;

namespace UserIdentitySample.Web.Helpers
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider
    {
        public readonly ClaimsPrincipal _user;

        public CustomAuthenticationStateProvider()
        {
            _user = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "ATuaMae"),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.NameIdentifier, "007"),
                }
            , "atuamae"
                ));
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return await Task.FromResult(new AuthenticationState(_user));
        }

        // You can call this method to notify that the authentication state has changed
        public void MarkUserAsAuthenticated()
        {
            ClaimsIdentity identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "ATuaMae"),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.NameIdentifier, "007"),
                }
            , "atuamae"
                );

            var principal = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(principal);
            NotifyAuthenticationStateChanged(Task.FromResult(state));
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            var state = new AuthenticationState(anonymous);
            NotifyAuthenticationStateChanged(Task.FromResult(state));
        }

        public void SignInWithToken(AccessTokenResponse tokenResponse)
        {
            var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "ATuaMae"),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.NameIdentifier, "007"),
                    new Claim("Bearer", tokenResponse.AccessToken),
                });
            var principal = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(principal);
            NotifyAuthenticationStateChanged(Task.FromResult(state));
        }

        public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
        {
            var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "ATuaMae"),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim(ClaimTypes.NameIdentifier, "007"),
                });
            var principal = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(principal);
            NotifyAuthenticationStateChanged(Task.FromResult(state));
        }
    }

}
