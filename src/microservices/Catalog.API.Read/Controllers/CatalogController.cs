using Microsoft.AspNetCore.Mvc;
using MediatR;
using Catalog.API.Read.Queries;

namespace Catalog.API.Read.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class CatalogController : Controller
{
    private readonly IMediator _mediator;

    public CatalogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("items")]
    public Task<IActionResult> GetAllItems([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        => HandleQuery(async () => await _mediator.Send(new GetAllItemsQuery(pageSize, pageIndex)));

    [HttpGet]
    [Route("items/brand/{brandName}")]
    public Task<IActionResult> GetItemsWithBrand(string brandName, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        => HandleQuery(async () => await _mediator.Send(new GetItemsWithBrandQuery(brandName, pageSize, pageIndex)));

    [HttpGet]
    [Route("items/type/{typeName}")]
    public Task<IActionResult> GetItemsWithType(string typeName, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        => HandleQuery(async () => await _mediator.Send(new GetItemsWithTypeQuery(typeName, pageSize, pageIndex)));

    [HttpGet]
    [Route("items/{id}")]
    public Task<IActionResult> GetItemById(string id)
        => HandleQuery(async () => await _mediator.Send(new GetItemByIdQuery(id)));

    [HttpGet]
    [Route("brands")]
    public Task<IActionResult> GetBrands()
        => HandleQuery(async () => await _mediator.Send(new GetBrandsQuery()));

    [HttpGet]
    [Route("types")]
    public Task<IActionResult> GetTypes()
        => HandleQuery(async () => await _mediator.Send(new GetTypesQuery()));

    private async Task<IActionResult> HandleQuery(Func<Task<Object?>> query)
    {
        try
        {
            var result = await query();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

