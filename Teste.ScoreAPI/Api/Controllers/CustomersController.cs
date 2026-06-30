using Teste.ScoreAPI.Application.Contracts;
using Teste.ScoreAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Teste.ScoreAPI.Api.Controllers;

[ApiController]
[Route("api/clientes")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CustomerResponse>> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var response = await _customerService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByCpf), new { cpf = response.Cpf }, response);
    }

    [HttpPut("{cpf}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerResponse>> UpdateByCpf(
        string cpf,
        [FromBody] UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerService.UpdateByCpfAsync(cpf, request, cancellationToken);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpGet("{cpf}")]
    [ProducesResponseType(typeof(CustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerResponse>> GetByCpf(string cpf, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetByCpfAsync(cpf, cancellationToken);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CustomerResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CustomerResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var customers = await _customerService.GetAllAsync(cancellationToken);
        return Ok(customers);
    }
}
