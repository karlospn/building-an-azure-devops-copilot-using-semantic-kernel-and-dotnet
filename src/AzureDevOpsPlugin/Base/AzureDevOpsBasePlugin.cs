using System.Text;
using Newtonsoft.Json;

namespace CustomCopilot.AzureDevOpsPlugin.Base
{
    public abstract class AzureDevOpsBasePlugin
    {
        protected static HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{string.Empty}:{Environment.GetEnvironmentVariable("AZURE_DEVOPS_PAT")}"))
            );
            return client;
        }

        protected async Task<dynamic?> GetApiResponse(string requestUri)
        {
            using var client = CreateHttpClient();
            var response = await client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject(responseBody) ?? throw new Exception();
            }
            return null;
        }

        protected async Task<dynamic?> PostApiResponse(string requestUri, HttpContent content)
        {
            using var client = CreateHttpClient();

            var response = await client.PostAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject(responseBody) ?? throw new Exception();
            }
            return null;
        }
    }
}