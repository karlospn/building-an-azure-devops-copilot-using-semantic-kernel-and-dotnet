using System.ComponentModel;
using System.Text;
using CustomCopilot.AzureDevOpsPlugin.Base;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomCopilot.AzureDevOpsPlugin
{
    public class AzureDevOpsRepositoriesPlugin : AzureDevOpsBasePlugin
    {
        [KernelFunction, Description("Get all existing git repositories on a given team project")]
        [return: Description("A list of names of existing git repositories on a given Azure DevOps Team Project")]
        public async Task<List<string>> ListGitRepositoriesInProject(
            [Description("Name of the team project")] string projectName)
        {
            try
            {
                var result = await GetApiResponse(
                    $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/{projectName}/_apis/git/repositories?api-version=6.0");

                if (result != null)
                {
                    var repositories = new List<string>();
                    foreach (var repo in result.value)
                    {
                        repositories.Add((string)repo.name);
                    }
                    return repositories;
                }
                return [];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        [KernelFunction, Description("Creates a new Git repository, if it doesn't exists, on a given Azure DevOps team project")]
        [return: Description("If the git repository creation was successful or not")]
        public async Task<bool> CreateGitRepositoryInProject(string projectName, string repositoryName)
        {
            try
            {
                using var client = CreateHttpClient();
                
                var projectResponse = await client.GetAsync(
                    $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/_apis/projects/{projectName}?api-version=6.0");

                if (projectResponse.IsSuccessStatusCode)
                {
                    var responseBody = await projectResponse.Content.ReadAsStringAsync();
                    var projectDetails = JObject.Parse(responseBody);
                    var projectId = projectDetails["id"]?.ToString();

                    var repositoryToCreate = new
                    {
                        name = repositoryName,
                        project = new { id = projectId}
                    };

                    var response = await client.PostAsync(
                        $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/_apis/git/repositories?api-version=7.1",
                        new StringContent(JsonConvert.SerializeObject(repositoryToCreate), Encoding.UTF8, "application/json")
                    );
                    
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

        [KernelFunction, Description("Deletes an existing git repository from a given Azure DevOps team project")]
        [return: Description("If the git repository deletion was successful or not")]
        public async Task<bool> DeleteGitRepositoryInProject(
            [Description("Name of the team project to delete")] string projectName,
            [Description("Name of the git repository to delete")] string repositoryName)
        {
            try
            {
                var result = await GetApiResponse($"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/{projectName}/_apis/git/repositories?api-version=6.0");
                
                if (result != null)
                {
                    foreach (var repo in result.value)
                    {
                        if (repo.name == repositoryName)
                        {
                            using var client = CreateHttpClient();
                            var response = await client.DeleteAsync(
                                $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/_apis/git/repositories/{repo.id}?api-version=6.0"
                            );
                            return response.IsSuccessStatusCode;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        [KernelFunction, Description("Get all files from an existing git repositories on a given team project")]
        [return: Description("A list of all files from an existing git repositories on a given Azure DevOps Team Project")]
        public async Task<List<string>> ListFilesInGitRepository(
            [Description("Name of the team project")] string projectName,
            [Description("Name of the git repository")] string repositoryName)
        {
            try
            {
                var result = await GetApiResponse(
                    $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/{projectName}/_apis/git/repositories/{repositoryName}/items?scopePath=/&recursionLevel=full&api-version=6.0");
                
                if (result != null)
                {
                    var files = new List<string>();
                    foreach (var item in result.value)
                    {
                        files.Add((string)item.path);
                    }
                    return files;
                }
                return [];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        [KernelFunction, Description("Explain the content of an existing git repositories on a given team project")]
        [return: Description("An explanation of what an existing git repositories on a given Azure DevOps Team Project is for")]
        public async Task<string> GetReadmeContentInGitRepository(
            [Description("Name of the team project")] string projectName,
            [Description("Name of the git repository")] string repositoryName)
        {
            try
            {
                var client = CreateHttpClient();
                var response = await client.GetAsync(
                    $"{
                        Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/{projectName}/_apis/sourceProviders/tfsGit/fileContents?commitOrBranch=main&repository={repositoryName}&path=/README.md&api-version=6.0-preview.1");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
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
