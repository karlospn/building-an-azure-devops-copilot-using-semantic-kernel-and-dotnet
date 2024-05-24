using System.ComponentModel;
using CustomCopilot.AzureDevOpsPlugin.Base;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace CustomCopilot.AzureDevOpsPlugin
{
    public class AzureDevOpsBuildsPlugin : AzureDevOpsBasePlugin
    {
        [KernelFunction, Description("Search for builds of a given repository on a team project")]
        [return: Description("Builds details")]
        public async Task<string> GetBuildsInProject(
            [Description("Azure DevOps Team Project")] string projectName,
            [Description("Azure DevOps Git repository")] string repositoryName)
        {
            try
            {
                var repoResult = await GetApiResponse(
                    $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/{projectName}/_apis/git/repositories/{repositoryName}?api-version=6.0");
                if (repoResult == null)
                {
                    Console.WriteLine("Failed to get repository ID");
                    return string.Empty;
                }

                string repositoryId = repoResult.id;
                var result = await GetApiResponse(
                    $"{Environment.GetEnvironmentVariable("AZURE_DEVOPS_ORG_URI")}/{projectName}/_apis/build/builds?repositoryId={repositoryId}&repositoryType=TfsGit&api-version=7.2-preview.7");

                if (result != null && result?.count > 0)
                {
                    var builds = new List<object>();
                    
                    foreach (var build in result!.value)
                    {
                        builds.Add(new
                        {
                            build.id,
                            build.buildNumber,
                            build.status,
                            build.result,
                            build.queueTime,
                            build.startTime,
                            build.finishTime,
                            build.sourceBranch,
                            build.sourceVersion,
                            build.url,
                            requestedFor = build.requestedFor.displayName
                        });
                    }
                    return JsonConvert.SerializeObject(builds);
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
