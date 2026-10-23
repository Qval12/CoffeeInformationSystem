using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeInformationSystem.Models;
using MySql.Data.MySqlClient;
namespace CoffeeInformationSystem.Services
{
    public sealed class AuthService
    {
        public void RegistrateEmployee(string name, string login, string password, Roles role)
        {
            PasswordService passwordService = new();
            Employee newEmployee = new(name, login, password, role);
            using var connection = new MySqlConnection("Server=localhost;Database=praktika_is31;User ID=user;Password=pass;");
            connection.Open();
            using var command = new MySqlCommand("""
                INSERT INTO Employees(Name, Login, PasswordHash, RoleId)
                VALUES (@name, @login, @password_hash, @role);
                """, connection);

            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@login", login);
            command.Parameters.AddWithValue("@password_hash", passwordService.Hash(newEmployee, password));
            command.Parameters.AddWithValue("@role", role);

            int affectedRows = command.ExecuteNonQuery();
            Console.WriteLine($"Изменено строк: {affectedRows}");
        }
        public void Login(string login, string password)
        {

        }
    }
}
