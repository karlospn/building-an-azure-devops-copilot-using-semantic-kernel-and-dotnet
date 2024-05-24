# **Building an Azure DevOps Copilot using Semantic Kernel and Azure OpenAi GPT-4o**

> **This repository doesn't contain a fully functional Azure DevOps Copilot, because the surface of the Azure DevOps REST API is too big. It implements only a small subset of endpoints of the API, so it can help you understand how simple is to build your own custom copilot.**


This repository tries to show you how easy is to build a custom Azure DevOps Copilot using C# Semantic Kernel and Azure OpenAI GPT-4o.

# **Prerequisites**

- An Azure DevOps instance.
- An Azure OpenAi instance with whatever model you prefer already deployed (I'll be using GPT-4o).


# **Content**

TODO


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