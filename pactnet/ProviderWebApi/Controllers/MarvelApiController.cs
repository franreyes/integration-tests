using Microsoft.AspNetCore.Mvc;

namespace ProviderWebApi.Controllers;

[ApiController]
[Route("marvel/character/{id}")]
public class MarvelApiController : ControllerBase
{
    private readonly ILogger<MarvelApiController> _logger;

    public MarvelApiController(ILogger<MarvelApiController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "id")]
    public Character Get(int id)
    {
        return new Character(23, "Hulk");
    }
}