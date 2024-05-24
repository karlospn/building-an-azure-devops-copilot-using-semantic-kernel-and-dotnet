using System.ComponentModel;
using System.Text;
using CustomCopilot.AzureDevOpsPlugin.Base;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace CustomCopilot.AzureDevOpsPlugin
{
    public class AzureDevOpsBranchesPlugin : AzureDevOpsBasePlugin
    {
        [KernelFunction, Description("Get all branches from a given git repository on a given Azure DevOps team project")]
        [return: Description("A list of all branches from an existing git repositories on a given Azure DevOps Team Project")]
        public async Task<List<string>> ListBranchesInGitRepository(
            [Description("Name of the team project")] string projectName,
            [Description("Name of the git repository")] string repositoryName)
        {
            try
            {
                var result = await GetApiResponse(
                    $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/{projectName}/_apis/git/repositories/{repositoryName}/refs?filter=heads&api-version=6.0");

                if (result != null)
                {
                    var branches = new List<string>();
                    foreach (var branch in result.value)
                    {
                        branches.Add((string)branch.name);
                    }
                    return branches;
                }
                return [];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        [KernelFunction, Description("Get branch info from a given branch from a given git repository on a given Azure DevOps team project")]
        [return: Description("Branch details")]
        public async Task<string> GetBranchInfoInGitRepository(
            [Description("Name of the team project")] string projectName,
            [Description("Name of the git repository")] string repositoryName,
            [Description("Name of branch")] string branchName)
        {
            try
            {
                var result = await GetApiResponse(
                    $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/{projectName}/_apis/git/repositories/{repositoryName}/refs?filter=heads/{branchName}&api-version=6.0");

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

        [KernelFunction, Description("Creates a new branch from a given git repository on a given Azure DevOps team project")]
        [return: Description("If the branch creation process was successful or not")]
        public async Task<bool> CreateBranchInGitRepository(
            [Description("Name of the team project")] string projectName,
            [Description("Name of the git repository")] string repositoryName,
            [Description("Name of the source branch that will be used to create a new one")] string sourceBranchName,
            [Description("Name of the new branch that will be created")] string newBranchName)
        {
            try
            {
                var result = await GetApiResponse(
                    $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/{projectName}/_apis/git/repositories/{repositoryName}/refs?filter=heads/{sourceBranchName}&api-version=6.0");

                if (result != null && result?.count > 0)
                {
                    string oldObjectId = result!.value[0].objectId;
                    var content = new[]
                    {
                        new
                        {
                            name = $"refs/heads/{newBranchName}",
                            oldObjectId = "0000000000000000000000000000000000000000",
                            newObjectId = oldObjectId
                        }
                    };
                    using var client = CreateHttpClient();
                    var httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/{projectName}/_apis/git/repositories/{repositoryName}/refs?api-version=6.0", httpContent);
                    return response.IsSuccessStatusCode;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        [KernelFunction, Description("Deletes a branch from a given git repository on a given Azure DevOps team project")]
        [return: Description("If the branch deletion process was successful or not")]
        public async Task<bool> DeleteBranchInGitRepository(
            [Description("Name of the team project")] string projectName,
            [Description("Name of the git repository")] string repositoryName,
            [Description("Name of the branch that will be deleted")] string branchName)
        {
            try
            {
                var result = await GetApiResponse(
                    $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/{projectName}/_apis/git/repositories/{repositoryName}/refs?filter=heads/{branchName}&api-version=6.0");

                if (result != null && result?.count > 0)
                {
                    string oldObjectId = result!.value[0].objectId;
                    var content = new[]
                    {
                        new
                        {
                            name = $"refs/heads/{branchName}",
                            oldObjectId = oldObjectId,
                            newObjectId = "0000000000000000000000000000000000000000"
                        }
                    };
                    using var client = CreateHttpClient();
                    var httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri($"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/{projectName}/_apis/git/repositories/{repositoryName}/refs?api-version=6.0"),
                        Content = httpContent
                    };
                    var response = await client.SendAsync(request);
                    return response.IsSuccessStatusCode;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
