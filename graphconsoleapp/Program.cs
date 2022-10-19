
using graphconsoleapp;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var settings = config.GetRequiredSection("M365Console").Get<M365ConsoleOptions>();

Console.WriteLine("Hello, " + settings.TenantID);
