namespace Teste.ScoreAPI.Domain.Entities;

public sealed class Customer
{
    public Guid Id { get; }
    public string? Name { get; }
    public string? Email { get; }
    public DateOnly BirthDate { get; }
    public Phone Phone { get; }
    public string Cpf { get; }
    public Address Address { get; }
    public decimal AnnualIncome { get; }

    public Customer(
        Guid id,
        string? name,
        string? email,
        DateOnly birthDate,
        Phone phone,
        string cpf,
        Address address,
        decimal annualIncome)
    {
        Id = id;
        Name = name;
        Email = email;
        BirthDate = birthDate;
        Phone = phone;
        Cpf = cpf;
        Address = address;
        AnnualIncome = annualIncome;
    }
}
