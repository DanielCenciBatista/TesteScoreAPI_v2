using Teste.ScoreAPI.Application.Interfaces;
using Teste.ScoreAPI.Domain.Entities;

namespace Teste.ScoreAPI.Application.Services;

public sealed class ScoreCalculator : IScoreCalculator
{
    public int Calculate(Customer customer, DateOnly referenceDate)
    {
        var annualIncomeScore = CalculateAnnualIncomeScore(customer.AnnualIncome);
        var age = CalculateAge(customer.BirthDate, referenceDate);
        var ageScore = CalculateAgeScore(age);

        return annualIncomeScore + ageScore;
    }

    private static int CalculateAnnualIncomeScore(decimal annualIncome)
    {
        return annualIncome switch
        {
            > 120000m => 300,
            >= 60000m and <= 120000m => 200,
            _ => 100
        };
    }

    private static int CalculateAgeScore(int age)
    {
        return age switch
        {
            > 40 => 200,
            >= 25 and <= 40 => 150,
            _ => 50
        };
    }

    private static int CalculateAge(DateOnly birthDate, DateOnly referenceDate)
    {
        var age = referenceDate.Year - birthDate.Year;
        if (referenceDate.DayOfYear < birthDate.DayOfYear)
        {
            age--;
        }

        return age;
    }
}
