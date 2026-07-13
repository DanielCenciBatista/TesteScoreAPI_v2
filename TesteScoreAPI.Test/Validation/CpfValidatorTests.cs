using Teste.ScoreAPI.Infrastructure.Validation;
using Xunit;

namespace TesteScoreAPI.Test.Validation;

public sealed class CpfValidatorTests
{
    private readonly CpfValidator _sut = new();

    [Theory]
    [InlineData("529.982.247-25")]
    [InlineData("52998224725")]
    [InlineData("111.444.777-35")]
    [InlineData("11144477735")]
    public void IsValid_CpfValido_RetornaTrue(string cpf)
    {
        var resultado = _sut.IsValid(cpf);

        Assert.True(resultado);
    }

    [Theory]
    [InlineData("000.000.000-00")]
    [InlineData("111.111.111-11")]
    [InlineData("999.999.999-99")]
    public void IsValid_TodosDigitosIguais_RetornaFalse(string cpf)
    {
        var resultado = _sut.IsValid(cpf);

        Assert.False(resultado);
    }

    [Theory]
    [InlineData("529.982.247-99")]
    [InlineData("111.444.777-00")]
    public void IsValid_DigitosVerificadoresErrados_RetornaFalse(string cpf)
    {
        var resultado = _sut.IsValid(cpf);

        Assert.False(resultado);
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("123456789012")]
    [InlineData("")]
    public void IsValid_TamanhoErrado_RetornaFalse(string cpf)
    {
        var resultado = _sut.IsValid(cpf);

        Assert.False(resultado);
    }

    [Fact]
    public void IsValid_ComLetras_RetornaFalse()
    {
        var resultado = _sut.IsValid("529.982.247-AB");

        Assert.False(resultado);
    }

    [Theory]
    [InlineData("529.982.247-25", "52998224725")]
    [InlineData("111.444.777-35", "11144477735")]
    [InlineData("52998224725", "52998224725")]
    public void Normalize_RemoveMascaras_RetornaSomenteDigitos(string cpf, string esperado)
    {
        var resultado = _sut.Normalize(cpf);

        Assert.Equal(esperado, resultado);
    }
}
