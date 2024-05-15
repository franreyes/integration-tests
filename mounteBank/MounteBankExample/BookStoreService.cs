using System.Net;
using System.Net.Http.Json;

namespace MounteBankExample;

public class BookStoreService
{
    private readonly string _server;

    public BookStoreService(string server)
    {
        _server = server;
    }

    public List<Book> GetLatestBooks()
    {
        HttpClientHandler handler = new HttpClientHandler 
        { 
            AutomaticDecompression = DecompressionMethods.All 
        };
        var client = new HttpClient(handler);
        var result = Get(client).Result;
        return result;
    }

    private Task<List<Book>> Get(HttpClient client)
    {
        using var response = client.GetAsync($"{_server}/books").Result;
        if (response.IsSuccessStatusCode)
        {
            return response.Content.ReadFromJsonAsync<List<Book>>();
        }

        return Task.FromResult(new List<Book>());
    }
}