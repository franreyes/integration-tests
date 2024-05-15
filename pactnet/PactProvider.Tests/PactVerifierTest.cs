using System.Diagnostics;
using Microsoft.AspNetCore;
using NUnit.Framework;
using PactNet.Infrastructure.Outputters;
using PactNet.Verifier;
using ProviderWebApi;

namespace PactProvider.Tests;

public class PactVerifierTest
{
    [Test]
    public void Verify()
    {
        var pactServiceUri = new Uri("http://localhost:9999");
        using var webHost = WebHost.CreateDefaultBuilder()
            .UseStartup<Startup>()
            .UseUrls(pactServiceUri.ToString()).Build();
        webHost.Start();

        using var pactVerifier = new PactVerifier("MarvelProvider", CreateConfigForLog());

        pactVerifier
            .WithHttpEndpoint(pactServiceUri)
            .WithFileSource(PactFilePath())
            .Verify();
    }

    private static FileInfo PactFilePath()
    {
        var pactPath = Path.Combine("..", "..", "..", "..",
            "Consumer.Tests",
            "pacts",
            "MarvelExampleConsumer-MarvelProvider.json");
        return new FileInfo(pactPath);
    }

    private static PactVerifierConfig CreateConfigForLog()
    {
        return new PactVerifierConfig
        {
            Outputters = new List<IOutput>
            {
                new ContractTestOutput()
            }
        };
    }


    public void ExecuteHttp()
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        var client = new HttpClient(clientHandler);
        using var response = client.GetAsync($"http://localhost:7139/marvel/character/23").Result;
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        }

    }
}

public class ContractTestOutput : IOutput
{

    public void WriteLine(string line)
    {
        Console.WriteLine(line);
    }
}