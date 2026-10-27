using CoffeeInformationSystem.Models;
using Microsoft.AspNetCore.Identity;

public sealed class PasswordService
{
    private readonly PasswordHasher<Employee> _hasher = new();

    public string Hash(Employee employee, string password)
    {
        return _hasher.HashPassword(employee, password);
    }

    public bool Verify(Employee employee, string password)
    {
        var result = _hasher.VerifyHashedPassword(employee, employee.PasswordHash, password);
        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}

