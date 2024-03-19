using Microsoft.AspNetCore.Mvc;
using Catalog.Entities.DbSet;
using Catalog.Entities.Models;
using Catalog.Entities.Dtos;
using System.Net;
using MediatR;
using Catalog.API.Queries;
using Catalog.API.Commands;

namespace Catalog.API.Controllers;

[ApiController]
[Route("/v1/[controller]")]
public class CatalogController : Controller
{
    private readonly IMediator _mediator;
    //private readonly ICatalogIntegrationEventService _catalogIntegrationEventService;

    public CatalogController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("items")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IEnumerable<CatalogItem>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<CatalogItem[]>> GetAllItems(int pageSize = 10, int pageIndex = 0)
    {
        var query = new GetAllItemsQuery(pageSize, pageIndex);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet]
    [Route("items/brand/{brandName}")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IEnumerable<CatalogItem>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<CatalogItem[]>> GetItemsWithBrand(string brandName, int pageSize = 10, int pageIndex = 0)
    {
        var query = new GetItemsWithBrandQuery(brandName, pageSize, pageIndex);
        var result = await _mediator.Send(query)
            ?? throw new BadHttpRequestException($"No CatalogItem with Brand: {brandName} found");

        return Ok(result);
    }

    [HttpGet]
    [Route("items/type/{typeName}")]
    [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IEnumerable<CatalogItem>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<CatalogItem[]>> GetItemsWithType(string typeName, int pageSize = 10, int pageIndex = 0)
    {
        var query = new GetItemsWithTypeQuery(typeName, pageSize, pageIndex);
        var result = await _mediator.Send(query)
            ?? throw new BadHttpRequestException($"No CatalogItem with Type: {typeName} found");

        return Ok(result);
    }

    [HttpGet]
    [Route("items/{id}")]
    [ProducesResponseType(typeof(CatalogItem), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<CatalogItem>> GetItemById(int id)
    {
        var query = new GetItemByIdQuery(id);
        var result = await _mediator.Send(query)
            ?? throw new BadHttpRequestException($"Item with id: {id} not found");

        return Ok(result);
    }

    [HttpGet]
    [Route("brands")]
    [ProducesResponseType(typeof(IEnumerable<CatalogBrand>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<CatalogBrand>> GetBrands()
    {
        var query = new GetBrandsQuery();
        var result = await _mediator.Send(query)
            ?? throw new BadHttpRequestException("No brands found");

        return Ok(result);
    }

    [HttpGet]
    [Route("types")]
    [ProducesResponseType(typeof(IEnumerable<CatalogType>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<CatalogType>> GetTypes()
    {
        var query = new GetTypesQuery();
        var result = await _mediator.Send(query)
            ?? throw new BadHttpRequestException("No types found");

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateItem(CatalogItemInputDto input)
    {
        if (!ModelState.IsValid)
            throw new BadHttpRequestException("Input model not complete");

        var command = new CreateItemCommand(input);
        var result = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetItemById), new { result.Id }, result);
    }
}

