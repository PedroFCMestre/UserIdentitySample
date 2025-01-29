using Microsoft.AspNetCore.Identity.Data;

namespace UserIdentitySample.Web.ApiClients
{
    public class IdentityApiClient(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

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
            var queryString = "?useCookies=true";
            var response = await _httpClient.PostAsJsonAsync($"/login{queryString}", user);

            if (response.IsSuccessStatusCode)
            {
                // Extract the authentication cookie from API response
                if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
                {
                    foreach (var cookie in cookies)
                    {
                        _httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
                    }
                }

                return "ok";

            }

            //var responseContent = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
            return "notok";
        }

    }
}
