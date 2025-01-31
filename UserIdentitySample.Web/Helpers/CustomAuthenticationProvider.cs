using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

using System.IdentityModel.Tokens.Jwt;
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

        public void SignInWithToken(AccessTokenResponse accessToken)
        {
            List<Claim> claims = ParseAccessToken(accessToken.AccessToken);

            if (claims.Count > 0)
            {
                var identity = new ClaimsIdentity(claims, IdentityConstants.BearerScheme);
                var principal = new ClaimsPrincipal(identity);
                var state = new AuthenticationState(principal);
                NotifyAuthenticationStateChanged(Task.FromResult(state));
            }
        }

        public List<Claim> ParseAccessToken(string accessToken)
        {
            List<Claim> toReturn = new();
            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(accessToken))
            {
                var token = handler.ReadJwtToken(accessToken);

                // Extract claims
                var claims = token.Claims.ToDictionary(c => c.Type, c => c.Value);

                // Example: Extract specific claims
                var userId = claims.ContainsKey(ClaimTypes.NameIdentifier) ? claims[ClaimTypes.NameIdentifier] : null;
                var email = claims.ContainsKey(ClaimTypes.Email) ? claims[ClaimTypes.Email] : null;
                var roles = claims.Where(c => c.Key == ClaimTypes.Role).Select(c => c.Value).ToList();

                Console.WriteLine($"User ID: {userId}");
                Console.WriteLine($"Email: {email}");
                Console.WriteLine($"Roles: {string.Join(", ", roles)}");


                // Add claims to the list
                foreach (var claim in claims)
                {
                    toReturn.Add(new Claim(claim.Key, claim.Value));
                }
            }
            else
            {
                toReturn = new()
                    {
                        new Claim(ClaimTypes.Name, "ATuaMae"),
                        new Claim("FullName", "Não Autenticado"),
                        new Claim(ClaimTypes.NameIdentifier, "1")
                    };
                Console.WriteLine("Invalid JWT token.");
            }

            return toReturn;
        }
    }

}
