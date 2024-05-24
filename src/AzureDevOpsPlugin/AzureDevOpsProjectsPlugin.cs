using System.ComponentModel;
using System.Text;
using CustomCopilot.AzureDevOpsPlugin.Base;
using Microsoft.SemanticKernel;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;

namespace CustomCopilot.AzureDevOpsPlugin
{
    public class AzureDevOpsProjectsPlugin : AzureDevOpsBasePlugin
    {
        [KernelFunction, Description("Creates a new Azure DevOps team project if it doesn't exist")]
        [return: Description("If the Azure DevOps Team Project creation was successful or not")]
        public async Task<bool> CreateTeamsProject(
            [Description("Name of the new team project")] string name,
            [Description("Description of the new team project")] string description)
        {
            try
            {
                var result = await GetApiResponse($"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/_apis/projects?api-version=6.0");
                if (result != null)
                {
                    foreach (var project in result.value)
                    {
                        if (project.name == name)
                        {
                            return false;
                        }
                    }
                }

                var projectToCreate = new
                {
                    name,
                    description,
                    capabilities = new
                    {
                        versioncontrol = new { sourceControlType = "Git" },
                        processTemplate = new { templateTypeId = "6b724908-ef14-45cf-84f8-768b5384da45" }
                    }
                };

                using var client = CreateHttpClient();
                var response = await client.PostAsync(
                    $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/_apis/projects?api-version=6.0",
                    new StringContent(JsonConvert.SerializeObject(projectToCreate), Encoding.UTF8, "application/json")
                );

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        [KernelFunction, Description("Get all existing Azure DevOps team projects")]
        [return: Description("A list of names of existing Azure DevOps Team Projects")]
        public async Task<List<string>> GetTeamsProject()
        {
            try
            {
                var connection = new VssConnection(
                    new Uri(Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")!), new VssBasicCredential(string.Empty, Environment.GetEnvironmentVariable("AZURE_DEVOPS_PAT")));
                var projectHttpClient = connection.GetClient<ProjectHttpClient>();
                IPagedList<TeamProjectReference> projects = await projectHttpClient.GetProjects();
                var projectNames = projects.Select(project => project.Name).ToList();
                return projectNames;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        [KernelFunction, Description("Deletes an existing Azure DevOps team project")]
        [return: Description("If the Azure DevOps Team Project deletion was successful or not")]
        public async Task<bool> DeleteProject(
            [Description("Name of the team project to delete")] string name)
        {
            try
            {
                var result = await GetApiResponse(
                    $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/_apis/projects?api-version=6.0");

                if (result != null)
                {
                    foreach (var project in result.value)
                    {
                        if (project.name == name)
                        {
                            using var client = CreateHttpClient();
                            var response = await client.DeleteAsync(
                                $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/_apis/projects/{project.id}?api-version=6.0"
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
    }
}
