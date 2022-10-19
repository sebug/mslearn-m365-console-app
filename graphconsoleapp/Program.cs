
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

var graphRequest = client.Users.Request()
    .Select(u => new {
        u.DisplayName,
        u.Mail
    })
    .Top(15)
    .OrderBy("DisplayName desc");

var results = await graphRequest.GetAsync();

foreach (var user in results)
{
    Console.WriteLine($"{user.Id}: {user.DisplayName} <{user.Mail}>");
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