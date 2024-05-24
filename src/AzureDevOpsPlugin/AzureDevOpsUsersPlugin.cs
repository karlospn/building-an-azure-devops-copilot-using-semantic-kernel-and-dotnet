using System.ComponentModel;
using CustomCopilot.AzureDevOpsPlugin.Base;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace CustomCopilot.AzureDevOpsPlugin
{
    public class AzureDevOpsUsersPlugin : AzureDevOpsBasePlugin
    {
        [KernelFunction, Description("Get information of a specific user given an email address")]
        [return: Description("User information details")]
        public async Task<string> GetUserByEmail(
            [Description("User email from where to obtain the information")] string email)
        {
            try
            {
                var result = await GetApiResponse(
                    $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_ALT_URI")}/_apis/graph/users?api-version=7.1-preview.1");

                if (result != null && result?.count > 0)
                {
                    return JsonConvert.SerializeObject(result!.value[0]);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
        }
    }
}