using FC.Codeflix.Catalog.Api.ApiModels.Category;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.DeleteCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Application.UseCases.Category.UpdateCategory;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Create([FromBody]CreateCategoryInput input, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(input, cancellationToken);

            return CreatedAtAction(nameof(Create), new {output.Id}, output);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(new GetCategoryInput(id), cancellationToken);

            return Ok(output);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListCategoriesOutput), StatusCodes.Status200OK)]
        public async Task<IActionResult> List(
            CancellationToken cancellationToken,
            [FromQuery] int? page = null, 
            [FromQuery] int? perPage = null, 
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] SearchOrder? sortDir = null
            )
        {
            var input = new ListCategoriesInput();
            if (page is not null) input.Page = page.Value;
            if (perPage is not null) input.PerPage = perPage.Value;
            if (!String.IsNullOrWhiteSpace(search)) input.Search = search ;
            if (!String.IsNullOrWhiteSpace(sortBy)) input.SortBy = sortBy;
            if (sortDir is not null) input.SortDir = sortDir.Value;

            var output = await _mediator.Send(input, cancellationToken);

            return Ok(output);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var output = await _mediator.Send(new DeleteCategoryInput(id), cancellationToken);

            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(CategoryModelOutput), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryApiInput apiInput, [FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var input = new UpdateCategoryInput(id, apiInput.Name, apiInput.Description, apiInput.IsActive);
            var output = await _mediator.Send(input, cancellationToken);

            return Ok(output);
        }
    }
}