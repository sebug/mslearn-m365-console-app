
using graphconsoleapp;
using graphconsoleapp.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var settings = config.GetRequiredSection("M365Console").Get<M365ConsoleOptions>();

var client = GetAuthenticatedGraphClient(settings);

var graphRequest = client.Groups.Request().Top(5).Expand("members");

var results = await graphRequest.GetAsync();

foreach (var g in results)
{
    foreach (var user in g.Members)
    {
        Console.WriteLine($"{user.Id}: {((Microsoft.Graph.User)user).DisplayName}");
    }
}

Console.WriteLine(Environment.NewLine + "Graph Request:");
Console.WriteLine(graphRequest.GetHttpRequestMessage().RequestUri);

GraphServiceClient GetAuthenticatedGraphClient(M365ConsoleOptions options)
{
    var authenticationProvider = CreateAuthorizationProvider(options);
    var graphClient = new GraphServiceClient(authenticationProvider);
    return graphClient;
}

IAuthenticationProvider CreateAuthorizationProvider(M365ConsoleOptions options)
{
    var scopes = new List<string>()
    {
        "https://graph.microsoft.com/.default"
    };
    var authority = $"https://login.microsoftonline.com/{options.TenantID}/v2.0";
    var cca = ConfidentialClientApplicationBuilder.Create(options.ApplicationID)
        .WithAuthority(authority)
        .WithRedirectUri(options.RedirectUri)
        .WithClientSecret(options.ApplicationSecret)
        .Build();
    
    return new MsalAuthenticationProvider(cca, scopes.ToArray());
}