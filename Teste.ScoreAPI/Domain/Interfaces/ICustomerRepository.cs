using Teste.ScoreAPI.Domain.Entities;

namespace Teste.ScoreAPI.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<bool> ExistsByCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task<bool> UpdateByCpfAsync(string cpf, Customer customer, CancellationToken cancellationToken = default);
    Task<Customer?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
}
