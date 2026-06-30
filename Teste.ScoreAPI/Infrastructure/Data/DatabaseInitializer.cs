using Microsoft.Data.SqlClient;

namespace Teste.ScoreAPI.Infrastructure.Data;

public sealed class DatabaseInitializer
{
    private readonly SqlConnectionFactory _connectionFactory;

    public DatabaseInitializer(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        const string sql = """
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Clientes')
            BEGIN
                CREATE TABLE Clientes (
                    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                    Nome NVARCHAR(200) NULL,
                    Email NVARCHAR(200) NULL,
                    DataNascimento DATE NOT NULL,
                    TelefoneDdd NVARCHAR(2) NOT NULL,
                    TelefoneNumero NVARCHAR(20) NOT NULL,
                    Cpf CHAR(11) NOT NULL,
                    EnderecoLogradouro NVARCHAR(200) NULL,
                    EnderecoNumero NVARCHAR(20) NULL,
                    EnderecoComplemento NVARCHAR(200) NULL,
                    EnderecoCep NVARCHAR(8) NULL,
                    EnderecoUf CHAR(2) NOT NULL,
                    RendaAnual DECIMAL(18,2) NOT NULL,
                    CONSTRAINT UQ_Clientes_Cpf UNIQUE (Cpf)
                );
            END
            """;

        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
