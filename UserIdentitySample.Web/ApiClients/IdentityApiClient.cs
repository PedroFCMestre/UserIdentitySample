using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

using System.Security.Claims;

namespace UserIdentitySample.Web.ApiClients
{
    public class IdentityApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<string> RegisterUserAsync(RegisterRequest user)
        {
            var response = await _httpClient.PostAsJsonAsync("/register", user);

            if (response.IsSuccessStatusCode)
            {
                return "ok";

            }

            var content = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

            return content!.Errors.First().Value.First();
        }

        public async Task<string?> LoginUserAsync(LoginRequest user)
        {
            var queryString = "?useCookies=true&useSessionCookies=true";
            var response = await _httpClient.PostAsJsonAsync($"/login{queryString}", user);

            if (response.IsSuccessStatusCode)
            {
                // Extract the authentication cookie from API response
                if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
                {
                    foreach (var cookie in cookies)
                    {
                        _httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
                        if (cookie.Contains(IdentityConstants.ApplicationScheme))
                        {

                            var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Name, user.Email),
                                    new Claim("token", cookie)
                                };
                            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme));

                            var authProp = new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                            };

                            _httpContextAccessor.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
                                principal, authProp);
                            return cookie;
                        }
                    }
                }
                else
                {
                    var tokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
                    return tokenResponse?.AccessToken;
                }
            }

            return string.Empty;
        }

    }
}
