using System.Net;
using System.Text.Json;
using NUnit.Framework;
using PactNet;

namespace Consumer.Tests;

public class MarvelCharacterServiceTest
{
    private IPactBuilderV4 _pactBuilder;

    [SetUp]
    public void SetUp()
    {
        var pact = Pact.V4("MarvelExampleConsumer", "MarvelProvider", new PactConfig());
        _pactBuilder = pact.WithHttpInteractions();
    }
    [Test]
    public void Retrieve_Marvel_Character()
    {
        const int characterId = 23;
        var hulk = new Character(characterId, "Hulk");
        _pactBuilder
            .UponReceiving("A GET request to retrieve a marvel character")
            .Given($"There is a character with id {characterId}")
            .WithRequest(HttpMethod.Get, $"/marvel/character/{characterId}")
            .WithHeader("Accept", "application/json")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(hulk, ConfigureJsonOptions());
        
        _pactBuilder.Verify(ctx =>
        {
            var bookStoreService = new MarvelCharactersService(ctx.MockServerUri.AbsoluteUri);
            
            var character = bookStoreService.GetCharacter(characterId);
            
            Assert.That(character, Is.EqualTo(hulk));
        });
    }

    private JsonSerializerOptions ConfigureJsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower,
        };
    }
}