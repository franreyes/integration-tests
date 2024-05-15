using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Consumer;

public class MarvelCharactersService
{
    private readonly string _remoteUrl;

    public MarvelCharactersService(string remoteUrl)
    {
        _remoteUrl = remoteUrl;
    }

    public Character GetCharacter(int id)
    {
        HttpClientHandler handler = new HttpClientHandler 
        { 
            AutomaticDecompression = DecompressionMethods.All 
        };
        var client = new HttpClient(handler);
        var result = Get(client, id).Result;
        return result;
    }

    private Task<Character> Get(HttpClient client, int id)
    {
        var requestUri = $"{_remoteUrl}marvel/character/{id}";
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        using var response = client.SendAsync(request).Result;
        if (response.IsSuccessStatusCode)
        {
            return response.Content.ReadFromJsonAsync<Character>();
        }

        return Task.FromResult(new Character(-1, "non-exist"));
    }
}