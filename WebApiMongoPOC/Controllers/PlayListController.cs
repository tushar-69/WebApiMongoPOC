using Microsoft.AspNetCore.Mvc;
using WebApiMongoPOC.Models;
using WebApiMongoPOC.Services;

namespace WebApiMongoPOC.Controllers;

[Controller]
[Route("/PlayList")]
public class PlayListController : Controller
{
    private readonly IMongoDBService _mongoDBService;

    public PlayListController(IMongoDBService mongoDBService)
    {
        _mongoDBService = mongoDBService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var response = await _mongoDBService.GetPlayListsAsync();
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PlayList playList)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values);

        await _mongoDBService.CreatePlayListAsync(playList);
        return Created();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] PlayList playList)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values);

        try
        {
            await _mongoDBService.UpdatePlayListAsync(playList);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound();
        }

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound();
        }
    }
}