using Microsoft.AspNetCore.Mvc;
using WebApiMongoPOC.Models;
using WebApiMongoPOC.Services;

namespace WebApiMongoPOC.Controllers;

[Controller]
[Route("/PlayList")]
public class PlayListController : ControllerBase
{
    private readonly IPlayListService _playListService;

    public PlayListController(IPlayListService playListService)
    {
        _playListService = playListService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PlayList>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var response = await _playListService.GetPlayListsAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PlayList), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        try
        {
            var response = await _playListService.GetPlayListByIdAsync(id);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(PlayList), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] PlayList playList)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values);

        var ID = await _playListService.CreatePlayListAsync(playList);
        return CreatedAtAction(nameof(GetById), new { Id = ID }, playList);
    }

    [HttpPut]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] PlayList playList)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState.Values);

        try
        {
            await _playListService.UpdatePlayListAsync(playList);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        try
        {
            await _playListService.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}