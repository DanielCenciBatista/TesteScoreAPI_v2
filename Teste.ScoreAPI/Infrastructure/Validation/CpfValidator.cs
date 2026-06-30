using System.Text.RegularExpressions;
using Teste.ScoreAPI.Domain.Interfaces;

namespace Teste.ScoreAPI.Infrastructure.Validation;

public sealed class CpfValidator : ICpfValidator
{
    public string Normalize(string cpf)
    {
        return Regex.Replace(cpf, "[^0-9]", string.Empty);
    }

    public bool IsValid(string cpf)
    {
        cpf = Normalize(cpf);

        if (cpf.Length != 11)
        {
            return false;
        }

        if (cpf.Distinct().Count() == 1)
        {
            return false;
        }

        var firstDigit = CalculateVerifierDigit(cpf.AsSpan(0, 9), 10);
        var secondDigit = CalculateVerifierDigit(cpf.AsSpan(0, 10), 11);

        return cpf[9] - '0' == firstDigit && cpf[10] - '0' == secondDigit;
    }

    private static int CalculateVerifierDigit(ReadOnlySpan<char> digits, int factor)
    {
        var total = 0;
        for (var i = 0; i < digits.Length; i++)
        {
            total += (digits[i] - '0') * (factor - i);
        }

        var remainder = total % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}
