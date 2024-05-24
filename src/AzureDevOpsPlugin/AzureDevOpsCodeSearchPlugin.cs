using System.ComponentModel;
using System.Text;
using CustomCopilot.AzureDevOpsPlugin.Base;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomCopilot.AzureDevOpsPlugin
{
    public class AzureDevOpsCodeSearchPlugin : AzureDevOpsBasePlugin
    {
        [KernelFunction, Description("Search text in a given team project")]
        [return: Description("List of text coincidences")]
        public async Task<string> GetCodeSearchInProject(
            [Description("Azure DevOps Team Project")] string projectName,
            [Description("Text Code that must be searched")] string searchText)
        {
            try
            {
                var payload = new JObject
                {
                    ["searchText"] = searchText,
                    ["$skip"] = 0,
                    ["$top"] = 50,
                    ["filters"] = new JObject
                    {
                        ["Project"] = new JArray { projectName },
                    }
                };

                var content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");

                var result = await PostApiResponse(
                    $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_ALM_URI")}/{projectName}/_apis/search/codesearchresults?api-version=7.0", content);
                
                if (result != null && result?.count > 0)
                {
                    return JsonConvert.SerializeObject(result);
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