using CoffeeInformationSystem.Configs;
using CoffeeInformationSystem.Models;
using MySql.Data.MySqlClient;
namespace CoffeeInformationSystem.Services
{
    public sealed class AuthService
    {
        DatabaseConfig dbConfig = new();
        public void RegistrateEmployee(string name, string login, string password, Roles role)
        {
            PasswordService passwordService = new();
            Employee newEmployee = new(name, login, password, role);
            using var connection = new MySqlConnection(dbConfig.connectionText);
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
        }
        public bool Login(string login, string password)
        {            
            PasswordService passwordService = new();
            using var connection = new MySqlConnection(dbConfig.connectionText);
            connection.Open();
            using var command = new MySqlCommand("""
                SELECT * FROM Employees
                WHERE Login = @login;
                """, connection);
            command.Parameters.AddWithValue("@login", login);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                Employee newEmployee = new();
                newEmployee.Id = reader.GetUInt64("Id");
                newEmployee.Name = reader.GetString("Name");
                newEmployee.Login = reader.GetString("Login");
                newEmployee.PasswordHash = reader.GetString("PasswordHash");
                newEmployee.RoleId = reader.GetUInt64("RoleId");
                return passwordService.Verify(newEmployee, password);
            }
            return false;
        }
        public Employee LoginEmployee(string login, string password)
        {
            PasswordService passwordService = new();
            using var connection = new MySqlConnection(dbConfig.connectionText);
            connection.Open();
            using var command = new MySqlCommand("""
                SELECT * FROM Employees
                WHERE Login = @login;
                """, connection);
            command.Parameters.AddWithValue("@login", login);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                Employee newEmployee = new();
                newEmployee.Id = reader.GetUInt64("Id");
                newEmployee.Name = reader.GetString("Name");
                newEmployee.Login = reader.GetString("Login");
                newEmployee.PasswordHash = reader.GetString("PasswordHash");
                newEmployee.RoleId = reader.GetUInt64("RoleId");
                return newEmployee;
            }
            return new Employee();
        }
    }
}
