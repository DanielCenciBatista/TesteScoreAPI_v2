using Moq;
using Teste.ScoreAPI.Application.Contracts;
using Teste.ScoreAPI.Application.Exceptions;
using Teste.ScoreAPI.Application.Interfaces;
using Teste.ScoreAPI.Application.Services;
using Teste.ScoreAPI.Domain.Entities;
using Teste.ScoreAPI.Domain.Interfaces;
using Xunit;

namespace TesteScoreAPI.Test.Services;

public sealed class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock = new();
    private readonly Mock<ICpfValidator> _cpfValidatorMock = new();
    private readonly Mock<IScoreCalculator> _scoreCalculatorMock = new();
    private readonly CustomerService _customerService;

    private const string CpfValido = "52998224725";

    public CustomerServiceTests()
    {
        _cpfValidatorMock.Setup(x => x.Normalize(It.IsAny<string>())).Returns<string>(cpf => cpf);
        _cpfValidatorMock.Setup(x => x.IsValid(CpfValido)).Returns(true);
        _scoreCalculatorMock.Setup(x => x.Calculate(It.IsAny<Customer>(), It.IsAny<DateOnly>())).Returns(350);

        _customerService = new CustomerService(_repositoryMock.Object, _cpfValidatorMock.Object, _scoreCalculatorMock.Object);
    }

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_DadosValidos_RetornaResponseComScore()
    {
        _repositoryMock.Setup(x => x.ExistsByCpfAsync(CpfValido, default)).ReturnsAsync(false);

        var resultado = await _customerService.CreateAsync(RequestValido());

        Assert.Equal(CpfValido, resultado.Cpf);
        Assert.Equal(350, resultado.Score);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Customer>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CpfJaCadastrado_LancaConflictException()
    {
        _repositoryMock.Setup(x => x.ExistsByCpfAsync(CpfValido, default)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() => _customerService.CreateAsync(RequestValido()));
    }

    [Fact]
    public async Task CreateAsync_CpfInvalido_LancaValidationException()
    {
        _cpfValidatorMock.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);
        _repositoryMock.Setup(x => x.ExistsByCpfAsync(It.IsAny<string>(), default)).ReturnsAsync(false);

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.CreateAsync(RequestValido()));
    }

    [Fact]
    public async Task CreateAsync_DataNascimentoNula_LancaValidationException()
    {
        var request = new CreateCustomerRequest
        {
            BirthDate = null,
            Phone = new PhoneRequest { Ddd = "11", Number = "999999999" },
            Cpf = CpfValido,
            Address = new AddressRequest { State = "SP" },
            AnnualIncome = 80000m
        };

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_DataNascimentoFutura_LancaValidationException()
    {
        _repositoryMock.Setup(x => x.ExistsByCpfAsync(CpfValido, default)).ReturnsAsync(false);
        var request = new CreateCustomerRequest
        {
            BirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Phone = new PhoneRequest { Ddd = "11", Number = "999999999" },
            Cpf = CpfValido,
            Address = new AddressRequest { State = "SP" },
            AnnualIncome = 80000m
        };

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_RendaAnualNegativa_LancaValidationException()
    {
        _repositoryMock.Setup(x => x.ExistsByCpfAsync(CpfValido, default)).ReturnsAsync(false);
        var request = new CreateCustomerRequest
        {
            BirthDate = new DateOnly(1990, 1, 1),
            Phone = new PhoneRequest { Ddd = "11", Number = "999999999" },
            Cpf = CpfValido,
            Address = new AddressRequest { State = "SP" },
            AnnualIncome = -1m
        };

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_RendaAnualNula_LancaValidationException()
    {
        var request = new CreateCustomerRequest
        {
            BirthDate = new DateOnly(1990, 1, 1),
            Phone = new PhoneRequest { Ddd = "11", Number = "999999999" },
            Cpf = CpfValido,
            Address = new AddressRequest { State = "SP" },
            AnnualIncome = null
        };

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_TelefoneNulo_LancaValidationException()
    {
        var request = new CreateCustomerRequest
        {
            BirthDate = new DateOnly(1990, 1, 1),
            Phone = null,
            Cpf = CpfValido,
            Address = new AddressRequest { State = "SP" },
            AnnualIncome = 80000m
        };

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_DddVazio_LancaValidationException()
    {
        var request = new CreateCustomerRequest
        {
            BirthDate = new DateOnly(1990, 1, 1),
            Phone = new PhoneRequest { Ddd = "", Number = "999999999" },
            Cpf = CpfValido,
            Address = new AddressRequest { State = "SP" },
            AnnualIncome = 80000m
        };

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_NumeroTelefoneVazio_LancaValidationException()
    {
        var request = new CreateCustomerRequest
        {
            BirthDate = new DateOnly(1990, 1, 1),
            Phone = new PhoneRequest { Ddd = "11", Number = "" },
            Cpf = CpfValido,
            Address = new AddressRequest { State = "SP" },
            AnnualIncome = 80000m
        };

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_CpfVazio_LancaValidationException()
    {
        var request = new CreateCustomerRequest
        {
            BirthDate = new DateOnly(1990, 1, 1),
            Phone = new PhoneRequest { Ddd = "11", Number = "999999999" },
            Cpf = "",
            Address = new AddressRequest { State = "SP" },
            AnnualIncome = 80000m
        };

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_EnderecoNulo_LancaValidationException()
    {
        var request = new CreateCustomerRequest
        {
            BirthDate = new DateOnly(1990, 1, 1),
            Phone = new PhoneRequest { Ddd = "11", Number = "999999999" },
            Cpf = CpfValido,
            Address = null,
            AnnualIncome = 80000m
        };

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_UfVazia_LancaValidationException()
    {
        var request = new CreateCustomerRequest
        {
            BirthDate = new DateOnly(1990, 1, 1),
            Phone = new PhoneRequest { Ddd = "11", Number = "999999999" },
            Cpf = CpfValido,
            Address = new AddressRequest { State = "" },
            AnnualIncome = 80000m
        };

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.CreateAsync(request));
    }

    [Theory]
    [InlineData("S")]
    [InlineData("SAO")]
    public async Task CreateAsync_UfComTamanhoErrado_LancaValidationException(string uf)
    {
        var request = new CreateCustomerRequest
        {
            BirthDate = new DateOnly(1990, 1, 1),
            Phone = new PhoneRequest { Ddd = "11", Number = "999999999" },
            Cpf = CpfValido,
            Address = new AddressRequest { State = uf },
            AnnualIncome = 80000m
        };

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.CreateAsync(request));
    }

    #endregion

    #region UpdateByCpfAsync

    [Fact]
    public async Task UpdateByCpfAsync_ClienteExistente_RetornaResponseAtualizado()
    {
        _repositoryMock.Setup(x => x.GetByCpfAsync(CpfValido, default)).ReturnsAsync(ClienteExistente());
        _repositoryMock.Setup(x => x.UpdateByCpfAsync(CpfValido, It.IsAny<Customer>(), default)).ReturnsAsync(true);

        var resultado = await _customerService.UpdateByCpfAsync(CpfValido, UpdateRequestValido());

        Assert.NotNull(resultado);
        Assert.Equal(350, resultado!.Score);
        _repositoryMock.Verify(x => x.UpdateByCpfAsync(CpfValido, It.IsAny<Customer>(), default), Times.Once);
    }

    [Fact]
    public async Task UpdateByCpfAsync_ClienteNaoEncontrado_RetornaNull()
    {
        _repositoryMock.Setup(x => x.GetByCpfAsync(CpfValido, default)).ReturnsAsync((Customer?)null);

        var resultado = await _customerService.UpdateByCpfAsync(CpfValido, UpdateRequestValido());

        Assert.Null(resultado);
    }

    [Fact]
    public async Task UpdateByCpfAsync_CpfVazio_LancaValidationException()
    {
        await Assert.ThrowsAsync<ValidationException>(() => _customerService.UpdateByCpfAsync("", UpdateRequestValido()));
    }

    [Fact]
    public async Task UpdateByCpfAsync_DataNascimentoFutura_LancaValidationException()
    {
        _repositoryMock.Setup(x => x.GetByCpfAsync(CpfValido, default)).ReturnsAsync(ClienteExistente());
        var request = new UpdateCustomerRequest
        {
            BirthDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            Phone = new PhoneRequest { Ddd = "11", Number = "999999999" },
            Address = new AddressRequest { State = "SP" },
            AnnualIncome = 80000m
        };

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.UpdateByCpfAsync(CpfValido, request));
    }

    [Fact]
    public async Task UpdateByCpfAsync_RendaNegativa_LancaValidationException()
    {
        _repositoryMock.Setup(x => x.GetByCpfAsync(CpfValido, default)).ReturnsAsync(ClienteExistente());
        var request = new UpdateCustomerRequest
        {
            BirthDate = new DateOnly(1990, 1, 1),
            Phone = new PhoneRequest { Ddd = "11", Number = "999999999" },
            Address = new AddressRequest { State = "SP" },
            AnnualIncome = -500m
        };

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.UpdateByCpfAsync(CpfValido, request));
    }

    #endregion

    #region GetByCpfAsync

    [Fact]
    public async Task GetByCpfAsync_ClienteExistente_RetornaResponse()
    {
        _repositoryMock.Setup(x => x.GetByCpfAsync(CpfValido, default)).ReturnsAsync(ClienteExistente());

        var resultado = await _customerService.GetByCpfAsync(CpfValido);

        Assert.NotNull(resultado);
        Assert.Equal(CpfValido, resultado!.Cpf);
    }

    [Fact]
    public async Task GetByCpfAsync_ClienteNaoEncontrado_RetornaNull()
    {
        _repositoryMock.Setup(x => x.GetByCpfAsync(CpfValido, default)).ReturnsAsync((Customer?)null);

        var resultado = await _customerService.GetByCpfAsync(CpfValido);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task GetByCpfAsync_CpfVazio_LancaValidationException()
    {
        await Assert.ThrowsAsync<ValidationException>(() => _customerService.GetByCpfAsync(""));
    }

    [Fact]
    public async Task GetByCpfAsync_CpfInvalido_LancaValidationException()
    {
        _cpfValidatorMock.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

        await Assert.ThrowsAsync<ValidationException>(() => _customerService.GetByCpfAsync("00000000000"));
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ComClientes_RetornaTodosComScore()
    {
        _repositoryMock.Setup(x => x.GetAllAsync(default)).ReturnsAsync(new List<Customer> { ClienteExistente(), ClienteExistente() });

        var resultado = await _customerService.GetAllAsync();

        Assert.Equal(2, resultado.Count);
        Assert.All(resultado, c => Assert.Equal(350, c.Score));
    }

    [Fact]
    public async Task GetAllAsync_SemClientes_RetornaListaVazia()
    {
        _repositoryMock.Setup(x => x.GetAllAsync(default)).ReturnsAsync(new List<Customer>());

        var resultado = await _customerService.GetAllAsync();

        Assert.Empty(resultado);
    }

    #endregion

    #region Helpers

    private static CreateCustomerRequest RequestValido() => new()
    {
        Name = "João Silva",
        Email = "joao@email.com",
        BirthDate = new DateOnly(1990, 1, 1),
        Phone = new PhoneRequest { Ddd = "11", Number = "999999999" },
        Cpf = CpfValido,
        Address = new AddressRequest { Street = "Rua A", Number = "10", State = "SP", ZipCode = "01310100" },
        AnnualIncome = 80000m
    };

    private static UpdateCustomerRequest UpdateRequestValido() => new()
    {
        Name = "João Silva Atualizado",
        Email = "joao@email.com",
        BirthDate = new DateOnly(1990, 1, 1),
        Phone = new PhoneRequest { Ddd = "11", Number = "988888888" },
        Address = new AddressRequest { Street = "Rua B", Number = "20", State = "SP", ZipCode = "01310100" },
        AnnualIncome = 90000m
    };

    private static Customer ClienteExistente() => new(
        Guid.NewGuid(),
        "João Silva",
        "joao@email.com",
        new DateOnly(1990, 1, 1),
        new Phone("11", "999999999"),
        CpfValido,
        new Address("Rua A", "10", null, "01310100", "SP"),
        80000m);

    #endregion
}
