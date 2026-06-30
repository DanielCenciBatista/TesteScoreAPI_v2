using Teste.ScoreAPI.Application.Contracts;

namespace Teste.ScoreAPI.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<CustomerResponse?> UpdateByCpfAsync(string cpf, UpdateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<CustomerResponse?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CustomerResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}
