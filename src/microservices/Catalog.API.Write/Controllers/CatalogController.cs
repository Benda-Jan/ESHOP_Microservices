using Microsoft.AspNetCore.Mvc;
using Catalog.Entities.Dtos;
using MediatR;
using Catalog.API.Write.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Catalog.API.Write.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class CatalogController : Controller
{
    private readonly IMediator _mediator;

    public CatalogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    public Task<IActionResult> CreateItem([FromBody]CatalogItemInputDto input)
        => HandleCommand(async () => await _mediator.Send(new CreateItemCommand(input)));

    [HttpPost]
    [Authorize]
    [Route("brand")]
    public Task<IActionResult> CreateBrand([FromBody]string brandName)
        => HandleCommand(async () => await _mediator.Send(new CreateBrandCommand(brandName)));

    [HttpPost]
    [Authorize]
    [Route("type")]
    public Task<IActionResult> CreateType([FromBody]string typeName)
        => HandleCommand(async () => await _mediator.Send(new CreateTypeCommand(typeName)));

    [HttpPut]
    [Authorize]
    public Task<IActionResult> UpdateItem([FromQuery]string itemId, [FromBody]CatalogItemInputDto input)
        => HandleCommand(async () => await _mediator.Send(new UpdateItemCommand(itemId, input)));

    [HttpDelete]
    [Authorize]
    public Task<IActionResult> DeleteItem([FromQuery]string itemId)
        => HandleCommand(async () => await _mediator.Send(new DeleteItemCommand(itemId)));

    [HttpDelete]
    [Authorize]
    [Route("brand")]
    public Task<IActionResult> DeleteBrand([FromQuery]string brandName)
        => HandleCommand(async () => await _mediator.Send(new DeleteBrandCommand(brandName)));

    [HttpDelete]
    [Authorize]
    [Route("type")]
    public Task<IActionResult> DeleteType([FromQuery]string typeName)
        => HandleCommand(async () => await _mediator.Send(new DeleteTypeCommand(typeName)));

    private async Task<IActionResult> HandleCommand(Func<Task<Object?>> command)
    {
        if (!ModelState.IsValid)
            return BadRequest("Input model not complete");

        try
        {
            var result = await command();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

