namespace Teste.ScoreAPI.Application.Contracts;

public sealed class CreateCustomerRequest
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public DateOnly? BirthDate { get; init; }
    public PhoneRequest? Phone { get; init; }
    public string? Cpf { get; init; }
    public AddressRequest? Address { get; init; }
    public decimal? AnnualIncome { get; init; }
}

public sealed class UpdateCustomerRequest
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public DateOnly? BirthDate { get; init; }
    public PhoneRequest? Phone { get; init; }
    public AddressRequest? Address { get; init; }
    public decimal? AnnualIncome { get; init; }
}

public sealed class PhoneRequest
{
    public string? Ddd { get; init; }
    public string? Number { get; init; }
}

public sealed class AddressRequest
{
    public string? Street { get; init; }
    public string? Number { get; init; }
    public string? Complement { get; init; }
    public string? ZipCode { get; init; }
    public string? State { get; init; }
}

public sealed class CustomerResponse
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Email { get; init; }
    public DateOnly BirthDate { get; init; }
    public PhoneResponse Phone { get; init; } = default!;
    public string Cpf { get; init; } = string.Empty;
    public AddressResponse Address { get; init; } = default!;
    public decimal AnnualIncome { get; init; }
    public int Score { get; init; }
}

public sealed class PhoneResponse
{
    public string Ddd { get; init; } = string.Empty;
    public string Number { get; init; } = string.Empty;
}

public sealed class AddressResponse
{
    public string? Street { get; init; }
    public string? Number { get; init; }
    public string? Complement { get; init; }
    public string? ZipCode { get; init; }
    public string State { get; init; } = string.Empty;
}
