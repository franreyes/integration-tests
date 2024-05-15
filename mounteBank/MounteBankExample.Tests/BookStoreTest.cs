using System.Net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using MbDotNet;
using MbDotNet.Models;
using NUnit.Framework;

namespace MounteBankExample.Tests;

public class BookStoreTest
{
    private const int RemotePort = 4545;
    private IContainer _containersBuilder;

    [OneTimeSetUp]
    public Task Initialize()
    {
        _containersBuilder = new ContainerBuilder()
            .WithImage("bbyars/mountebank")
            .WithName("mountebank")
            .WithPortBinding(2525, 2525)
            .WithExposedPort(RemotePort)
            .WithPortBinding(RemotePort, RemotePort)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(2525))
            .WithHostname("localhost")
            .Build();
        return _containersBuilder.StartAsync();
    }

    [OneTimeTearDown]
    public Task Dispose()
    {
        return _containersBuilder.DisposeAsync().AsTask();
    }

    [Test]
    public async Task Get_Latest_Books()
    {
        var client = new MountebankClient();
        var books = new List<Book> { new(1, "War and Peace"), new(2, "1984"), };
        await client.CreateHttpImposterAsync(RemotePort, imposter =>
        {
            imposter.AddStub()
                .OnPathAndMethodEqual("/books", Method.Get)
                .ReturnsJson(HttpStatusCode.OK, books);

            imposter.AddStub().ReturnsStatus(HttpStatusCode.NotFound);
        });
        var bookStore = new BookStoreService($"http://localhost:{RemotePort}");

        var latestBooks = bookStore.GetLatestBooks();
        
        Assert.That(latestBooks.Count, Is.EqualTo(2));
        var imposter = await client.GetHttpImposterAsync(RemotePort);
        Assert.That(imposter.NumberOfRequests, Is.EqualTo(1));
    }
}