using Teste.ScoreAPI.Application.Services;
using Teste.ScoreAPI.Domain.Entities;
using Xunit;

namespace TesteScoreAPI.Test.Services;

public sealed class ScoreCalculatorTests
{
    private readonly ScoreCalculator scoreCalculator = new();
    private static readonly DateOnly DataReferencia = new(2025, 6, 1);

    [Theory]
    [InlineData(150000, 1980, 500)]
    [InlineData(150000, 1990, 450)]
    [InlineData(150000, 2005, 350)]
    [InlineData(90000,  1980, 400)]
    [InlineData(90000,  1990, 350)]
    [InlineData(90000,  2005, 250)]
    [InlineData(30000,  1980, 300)]
    [InlineData(30000,  1990, 250)]
    [InlineData(30000,  2005, 150)]
    public void Calculate_TodasAsFaixas_RetornaScoreCorreto(decimal rendaAnual, int anoNascimento, int scoreEsperado)
    {
        var cliente = CriarCliente(rendaAnual, new DateOnly(anoNascimento, 1, 1));

        var resultado = scoreCalculator.Calculate(cliente, DataReferencia);

        Assert.Equal(scoreEsperado, resultado);
    }

    [Fact]
    public void Calculate_RendaExatamente120Mil_RetornaPontuacaoFaixa60a120()
    {
        var cliente = CriarCliente(120000m, new DateOnly(1980, 1, 1));

        var resultado = scoreCalculator.Calculate(cliente, DataReferencia);

        Assert.Equal(400, resultado);
    }

    [Fact]
    public void Calculate_RendaExatamente60Mil_RetornaPontuacaoFaixa60a120()
    {
        var cliente = CriarCliente(60000m, new DateOnly(1980, 1, 1));

        var resultado = scoreCalculator.Calculate(cliente, DataReferencia);

        Assert.Equal(400, resultado);
    }

    [Fact]
    public void Calculate_IdadeExatamente40Anos_RetornaPontuacaoFaixa25a40()
    {
        var clienteCom40Anos = CriarCliente(30000m, DataReferencia.AddYears(-40));

        var resultado = scoreCalculator.Calculate(clienteCom40Anos, DataReferencia);

        Assert.Equal(250, resultado);
    }

    [Fact]
    public void Calculate_IdadeExatamente25Anos_RetornaPontuacaoFaixa25a40()
    {
        var clienteCom25Anos = CriarCliente(30000m, new DateOnly(2000, 1, 1));

        var resultado = scoreCalculator.Calculate(clienteCom25Anos, DataReferencia);

        Assert.Equal(250, resultado);
    }

    [Fact]
    public void Calculate_AniversarioHoje_IdadeCompletaContabilizada()
    {
        var nascimento = new DateOnly(1983, 6, 1);
        var cliente = CriarCliente(30000m, nascimento);

        var resultado = scoreCalculator.Calculate(cliente, DataReferencia);

        Assert.Equal(300, resultado);
    }

    private static Customer CriarCliente(decimal rendaAnual, DateOnly dataNascimento) =>
        new(
            Guid.NewGuid(),
            "Teste",
            null,
            dataNascimento,
            new Phone("11", "999999999"),
            "52998224725",
            new Address(null, null, null, null, "SP"),
            rendaAnual);
}
