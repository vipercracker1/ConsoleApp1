using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Step 2: Obtain an Access Token
        string tokenUrl = "https://ducle:3079/connect/token";
        var tokenPayload = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", "myclient"),
            new KeyValuePair<string, string>("client_secret", "mysecret"),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        using (var httpClient = new HttpClient())
        {
            var tokenResponse = await httpClient.PostAsync(tokenUrl, tokenPayload);
            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var accessToken = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessTokenResponse>(tokenContent).access_token;

            // Step 3: Make API Request
            string apiEndpoint = "https://ducle:3081/api/v1/pmquality/batches";

            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            var response = await httpClient.GetAsync(apiEndpoint);

            // Step 4: Process the Response
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API response:");
                Console.WriteLine(apiResponse); // Print the response content to the console
                // Process the API response as needed
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("API request failed: Unauthorized");
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                Console.WriteLine("API request failed: Forbidden");
            }
            else
            {
                Console.WriteLine($"API request failed: {response.StatusCode}");
            }
        }
    }
}

class AccessTokenResponse
{
    public string access_token { get; set; }
}
