namespace Teste.ScoreAPI.Domain.Interfaces;

public interface ICpfValidator
{
    bool IsValid(string cpf);
    string Normalize(string cpf);
}
