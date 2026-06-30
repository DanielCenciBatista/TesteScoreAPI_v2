using Teste.ScoreAPI.Domain.Entities;

namespace Teste.ScoreAPI.Application.Interfaces;

public interface IScoreCalculator
{
    int Calculate(Customer customer, DateOnly referenceDate);
}
