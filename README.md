# **Building an Azure DevOps Copilot using Semantic Kernel and Azure OpenAi GPT-4o**

> **This repository doesn't contain a fully functional Azure DevOps Copilot, because the surface of the Azure DevOps REST API is too big. It implements only a small subset of endpoints of the API, so it can help you understand how simple is to build your own custom copilot.**


This repository tries to show you how easy is to build a custom Azure DevOps Copilot using C# Semantic Kernel and Azure OpenAI GPT-4o.

# **Prerequisites**

- An Azure DevOps instance.
- An Azure OpenAi instance with whatever model you prefer already deployed (I'll be using GPT-4o).


# **Content**

The application is a basic chat client that uses Semantic Kernel alongside Plugins.

Plugins are a way to add functionality and capabilities to your Copilot. They are based on the OpenAI plugin specification.

Plugins contains both code and prompts. You can use plugins to access data and perform operations with another third party services.

In this application we're leveraging Semantic Kernel Plugins to interact with Azure DevOps REST Api.

## **Main application**

The main application is a .NET 8 console app that functions as a basic chat client. 

Users can ask questions related to their Azure DevOps instance, and the Copilot will utilize our custom Semantic Kernel Plugins to fetch data from the Azure DevOps instance and respond accordingly.


The most noteworthy part of the main application is likely the system prompt setup. It is crucial to properly ground the LLM and prevent it from attempting to guess or infer values when calling the SK Plugins, as it might guess incorrectly. A better approach is to ensure the LLM asks the user which values should be used when in doubt.

Here's the resulting system prompt:

```text
You are a virtual assistant specifically designed to manage an Azure DevOps instance. Your scope of conversation is strictly limited to this domain. Your responses should be concise, accurate, and directly related to the query at hand.
In order to provide the most accurate responses, you require precise inputs. 

If a function calling involves parameters that you do not have sufficient information about it, it is crucial that you do not attempt to guess or infer their values. Instead, your primary action should always be to ask the user to provide more detailed information about these parameters. This is a non-negotiable aspect of your function. Guessing or inferring values is not an acceptable course of action. Your goal is to avoid any potential misunderstandings and to provide the most accurate and helpful response possible.

Remember, when in doubt, always ask for more information. Never guess the values of a function parameter.

If a function call fails to produce any valid data, the response must always be: 'I'm sorry, but I wasn't able to retrieve any data.If the problem persists, you may want to contact your Azure DevOps administrator or support for further assistance'. 

Never fabricate a response if the function calling fails or returns invalid data.
```

## **Azure DevOps Plugins**

This repository doesn't contain a fully functional Azure DevOps Copilot, because the surface of the Azure DevOps REST API is too big. It implements only a small subset of endpoints of the API.

Let's review which functionality does this Copilot covers

### **Team Projects endpoints**

- Create Team Project.
- List Team Projects.
- Delete Team Projects.

**Demo**

![demo-tp](https://raw.githubusercontent.com/karlospn/building-an-azure-devops-copilot-using-semantic-kernel-and-dotnet/main/docs/azdo-copilot-projects.png)

### **Git repositories endpoints**

- List git repos on a given Team Project.
- Create a new git repo.
- Delete a git repo.
- List all files of a git repo.
- Explain a git repo (it fetches the README file on the main branch and passes it to the LLM).

**Demo 1**

![demo-repos-1](https://raw.githubusercontent.com/karlospn/building-an-azure-devops-copilot-using-semantic-kernel-and-dotnet/main/docs/azdo-copilot-repos-1.png)

**Demo 2**

![demo-repos-2](https://raw.githubusercontent.com/karlospn/building-an-azure-devops-copilot-using-semantic-kernel-and-dotnet/main/docs/azdo-copilot-repos-2.png)

**Demo 3**

![demo-repos-3](https://raw.githubusercontent.com/karlospn/building-an-azure-devops-copilot-using-semantic-kernel-and-dotnet/main/docs/azdo-copilot-repos-3.png)


### **Git Branches endpoints**

- List branches of a git repo.
- Get a branch info.
- Create a new branch on a give git repo.
- Delete a branch.

**Demo**

![demo-branches](https://raw.githubusercontent.com/karlospn/building-an-azure-devops-copilot-using-semantic-kernel-and-dotnet/main/docs/azdo-copilot-branches.png)

### **Builds endpoints**

- Search for builds from a given git repo.

**Demo**

![demo-builds](https://raw.githubusercontent.com/karlospn/building-an-azure-devops-copilot-using-semantic-kernel-and-dotnet/main/docs/azdo-copilot-builds.png)

### **CodeSearch endpoints**

- Search text in a given team project.

**Demo 1**

![demo-cs](https://raw.githubusercontent.com/karlospn/building-an-azure-devops-copilot-using-semantic-kernel-and-dotnet/main/docs/azdo-copilot-code-search-1.png)

**Demo 2**

![demo-cs](https://raw.githubusercontent.com/karlospn/building-an-azure-devops-copilot-using-semantic-kernel-and-dotnet/main/docs/azdo-copilot-code-search-2.png)


# **How to test it**

To run it, you need the following environment variables:

- ``AZURE_DEVOPS_PAT``: A personal access token from your Azure DevOps instance. It is easier if it has full access permissions because we are going to make use of multiple endpoints of the REST API.
- ``AZURE_DEVOPS_ORG_URI``: The URI of your Azure DevOps REST API. The format must be:`` https://dev.azure.com/{your-org}``
- ``AZURE_DEVOPS_ORG_ALT_URI``: The URI of your VSSPS Azure DevOps REST API. The format must be: ``https://vssps.dev.azure.com/{your-org}``
- ``AZURE_DEVOPS_ORG_ALM_URI``: The URI of your ALMSEARCH Azure DevOps REST API. The format must be: ``https://almsearch.dev.azure.com/{your-org}``
- ``OAI_MODEL_NAME``: The LLM name you're going to use. In my case, I'm using ``gpt-4o``. You can use another one of the multiple available models in Azure OpenAI.
- ``OAI_ENDPOINT``: The endpoint of your Azure OpenAI instance. It always has the same format: ``https://{service-name}.openai.azure.com/``
- ``OAI_APIKEY``: An Azure OpenAI Api Key.

Here's an example:
```json
    "AZURE_DEVOPS_PAT": "j093j4194ada123czxsaspdjapsijasfhpi213",
    "AZURE_DEVOPS_ORG_URI": "https://dev.azure.com/cpn",
    "AZURE_DEVOPS_ORG_ALT_URI": "https://vssps.dev.azure.com/cpn",
    "AZURE_DEVOPS_ORG_ALM_URI": "https://almsearch.dev.azure.com/cpn",
    "OAI_MODEL_NAME": "gpt-4o",
    "OAI_ENDPOINT": "https://mytechramblings.openai.azure.com/",
    "OAI_APIKEY": "123123012h032940h213123asdasd"
```