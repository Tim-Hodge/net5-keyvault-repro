using System;

using Azure.Identity;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace Net5Sdk4Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq("http://seq.staging.crezco.com")
                .CreateLogger();

            Log.Information("Starting .NET 5 test");
            Log.CloseAndFlush();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var useKeyVault = Environment.GetEnvironmentVariable("UseKeyVault") ?? "1";

                    if (useKeyVault == "1")
                    {
                        var keyVaultUri = Environment.GetEnvironmentVariable("KeyVault") ?? string.Empty;

                        //https://stackoverflow.com/questions/62503022/net-core-key-vault-configuration-using-azure-security-keyvault-secrets
                        var credential = new DefaultAzureCredential();
                        config.AddAzureKeyVault(new Uri(keyVaultUri), credential);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
