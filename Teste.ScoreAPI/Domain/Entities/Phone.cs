namespace Teste.ScoreAPI.Domain.Entities;

public sealed class Phone
{
    public string Ddd { get; }
    public string Number { get; }

    public Phone(string ddd, string number)
    {
        Ddd = ddd;
        Number = number;
    }
}
