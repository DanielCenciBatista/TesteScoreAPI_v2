using Microsoft.Data.SqlClient;
using Teste.ScoreAPI.Domain.Entities;
using Teste.ScoreAPI.Domain.Interfaces;
using Teste.ScoreAPI.Infrastructure.Data;

namespace Teste.ScoreAPI.Infrastructure.Repositories;

public sealed class SqlCustomerRepository : ICustomerRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public SqlCustomerRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> ExistsByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new SqlCommand("SELECT 1 FROM Clientes WHERE Cpf = @Cpf", connection);
        command.Parameters.AddWithValue("@Cpf", cpf);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is not null;
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = """
            INSERT INTO Clientes
                (Id, Nome, Email, DataNascimento, TelefoneDdd, TelefoneNumero, Cpf, EnderecoLogradouro, EnderecoNumero, EnderecoComplemento, EnderecoCep, EnderecoUf, RendaAnual)
            VALUES
                (@Id, @Nome, @Email, @DataNascimento, @TelefoneDdd, @TelefoneNumero, @Cpf, @EnderecoLogradouro, @EnderecoNumero, @EnderecoComplemento, @EnderecoCep, @EnderecoUf, @RendaAnual)
            """;

        await using var command = new SqlCommand(sql, connection);
        AddCustomerParameters(command, customer);

        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            throw new InvalidOperationException("CPF já cadastrado.", ex);
        }
    }

    public async Task<bool> UpdateByCpfAsync(string cpf, Customer customer, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = """
            UPDATE Clientes
            SET Nome = @Nome,
                Email = @Email,
                DataNascimento = @DataNascimento,
                TelefoneDdd = @TelefoneDdd,
                TelefoneNumero = @TelefoneNumero,
                EnderecoLogradouro = @EnderecoLogradouro,
                EnderecoNumero = @EnderecoNumero,
                EnderecoComplemento = @EnderecoComplemento,
                EnderecoCep = @EnderecoCep,
                EnderecoUf = @EnderecoUf,
                RendaAnual = @RendaAnual
            WHERE Cpf = @WhereCpf
            """;

        await using var command = new SqlCommand(sql, connection);
        AddCustomerParameters(command, customer);
        command.Parameters.AddWithValue("@WhereCpf", cpf);

        var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
        return affectedRows > 0;
    }

    public async Task<Customer?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new SqlCommand(SelectColumns + " WHERE Cpf = @Cpf", connection);
        command.Parameters.AddWithValue("@Cpf", cpf);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return MapCustomer(reader);
    }

    public async Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new SqlCommand(SelectColumns, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var customers = new List<Customer>();
        while (await reader.ReadAsync(cancellationToken))
        {
            customers.Add(MapCustomer(reader));
        }

        return customers;
    }

    private const string SelectColumns = """
        SELECT Id, Nome, Email, DataNascimento, TelefoneDdd, TelefoneNumero, Cpf, EnderecoLogradouro, EnderecoNumero, EnderecoComplemento, EnderecoCep, EnderecoUf, RendaAnual
        FROM Clientes
        """;

    private static void AddCustomerParameters(SqlCommand command, Customer customer)
    {
        command.Parameters.AddWithValue("@Id", customer.Id);
        command.Parameters.AddWithValue("@Nome", (object?)customer.Name ?? DBNull.Value);
        command.Parameters.AddWithValue("@Email", (object?)customer.Email ?? DBNull.Value);
        command.Parameters.AddWithValue("@DataNascimento", customer.BirthDate.ToDateTime(TimeOnly.MinValue));
        command.Parameters.AddWithValue("@TelefoneDdd", customer.Phone.Ddd);
        command.Parameters.AddWithValue("@TelefoneNumero", customer.Phone.Number);
        command.Parameters.AddWithValue("@Cpf", customer.Cpf);
        command.Parameters.AddWithValue("@EnderecoLogradouro", (object?)customer.Address.Street ?? DBNull.Value);
        command.Parameters.AddWithValue("@EnderecoNumero", (object?)customer.Address.Number ?? DBNull.Value);
        command.Parameters.AddWithValue("@EnderecoComplemento", (object?)customer.Address.Complement ?? DBNull.Value);
        command.Parameters.AddWithValue("@EnderecoCep", (object?)customer.Address.ZipCode ?? DBNull.Value);
        command.Parameters.AddWithValue("@EnderecoUf", customer.Address.State);
        command.Parameters.AddWithValue("@RendaAnual", customer.AnnualIncome);
    }

    private static Customer MapCustomer(SqlDataReader reader)
    {
        return new Customer(
            reader.GetGuid(reader.GetOrdinal("Id")),
            reader.IsDBNull(reader.GetOrdinal("Nome")) ? null : reader.GetString(reader.GetOrdinal("Nome")),
            reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
            DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("DataNascimento"))),
            new Phone(
                reader.GetString(reader.GetOrdinal("TelefoneDdd")),
                reader.GetString(reader.GetOrdinal("TelefoneNumero"))),
            reader.GetString(reader.GetOrdinal("Cpf")),
            new Address(
                reader.IsDBNull(reader.GetOrdinal("EnderecoLogradouro")) ? null : reader.GetString(reader.GetOrdinal("EnderecoLogradouro")),
                reader.IsDBNull(reader.GetOrdinal("EnderecoNumero")) ? null : reader.GetString(reader.GetOrdinal("EnderecoNumero")),
                reader.IsDBNull(reader.GetOrdinal("EnderecoComplemento")) ? null : reader.GetString(reader.GetOrdinal("EnderecoComplemento")),
                reader.IsDBNull(reader.GetOrdinal("EnderecoCep")) ? null : reader.GetString(reader.GetOrdinal("EnderecoCep")),
                reader.GetString(reader.GetOrdinal("EnderecoUf"))),
            reader.GetDecimal(reader.GetOrdinal("RendaAnual")));
    }
}
