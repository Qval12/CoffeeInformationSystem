using CoffeeInformationSystem.Models;
using CoffeeInformationSystem.Enums;
using MySql.Data.MySqlClient;

namespace CoffeeInformationSystem.Services
{
    public sealed class AuthService : BaseService
    {        
        public ServiceResult<bool> RegisterEmployee(string name, string login, string password, Role role)
        {
            PasswordService passwordService = new();

            Employee tempEmployee = new() { Name = name, Login = login, RoleID = role };
            string passwordHash = passwordService.Hash(tempEmployee, password);

            string query = """
                INSERT INTO Employees (Name, Login, PasswordHash, RoleID)
                VALUES (@name, @login, @password_hash, @role);
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@login", login);
            command.Parameters.AddWithValue("@password_hash", passwordHash);
            command.Parameters.AddWithValue("@role", role);

            try
            {
                connection.Open();
                int affectedRows = command.ExecuteNonQuery();
                return ServiceResult<bool>.Success(affectedRows > 0);
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                return ServiceResult<bool>.Failure("Сотрудник с таким логином уже существует");
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ServiceResult<bool>.Failure($"Ошибка регистрации: {ex.Message}");
            }
        }

        public ServiceResult<Employee> Login(string login, string password)
        {
            PasswordService passwordService = new();

            string query = """
                SELECT * FROM Employees
                WHERE Login = @login AND IsActive = 1
                LIMIT 1;
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@login", login);

            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Employee employee = new()
                    {
                        ID = reader.GetUInt64("ID"),
                        Name = reader.GetString("Name"),
                        Login = reader.GetString("Login"),
                        PasswordHash = reader.GetString("PasswordHash"),
                        RoleID = (Role)reader.GetUInt64("RoleID"),
                        IsActive = reader.GetBoolean("IsActive")
                    };   

                    if (passwordService.Verify(employee, password))
                    {
                        return ServiceResult<Employee>.Success(employee);
                    }
                }
                return ServiceResult<Employee>.Failure("Неверный логин или пароль");
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ServiceResult<Employee>.Failure($"Ошибка входа: {ex.Message}");
            }
        }
    }
}