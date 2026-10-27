using CoffeeInformationSystem.Models;
using MySql.Data.MySqlClient;
using System.Data.SqlTypes;

namespace CoffeeInformationSystem.Services
{
    public sealed class DishService : BaseService
    {
        public ServiceResult<Dish> GetByID(ulong id)
        {
            string query = """
                SELECT Dishes.ID AS ID, Dishes.Title AS Title, Price, CategoryID, IsActive, Categories.Title AS CategoryTitle FROM Dishes
                JOIN Categories ON Dishes.CategoryID = Categories.ID
                WHERE Dishes.ID = @id
                LIMIT 1;
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Dish dish = new()
                    {
                        ID = reader.GetUInt64("ID"),
                        Title = reader.GetString("Title"),
                        Price = reader.GetDecimal("Price"),
                        CategoryID = reader.GetUInt64("CategoryID"),
                        IsActive = reader.GetBoolean("IsActive"),
                        CategoryTitle = reader.GetString("CategoryTitle")
                    };
                    return ServiceResult<Dish>.Success(dish);
                }
                return ServiceResult<Dish>.Failure("Блюдо не найдено");
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ServiceResult<Dish>.Failure($"Непредвиденная ошибка: {ex.Message}");
            }
        }

        public ServiceResult<bool> ToggleActive(ulong id)
        {
            string query = """
                UPDATE Dishes
                SET IsActive = NOT IsActive
                WHERE ID = @id;
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", id);

            try
            {    
                connection.Open();
                int affectedRows = command.ExecuteNonQuery();
                return ServiceResult<bool>.Success(affectedRows > 0);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ServiceResult<bool>.Failure($"Непредвиденная ошибка: {ex.Message}");
            }  
        }

        public ServiceResult<List<Dish>> OutputAll()
        {
            var dishes = new List<Dish>();
    
            string query = """
                SELECT Dishes.ID AS ID, Dishes.Title AS Title, Price, CategoryID, IsActive, Categories.Title AS CategoryTitle FROM Dishes
                JOIN Categories ON Dishes.CategoryID = Categories.ID
                ORDER BY Dishes.ID;
                """;
    
            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);
    
            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();
    
                while (reader.Read())
                {
                    Dish dish = new()
                    {
                        ID = reader.GetUInt64("ID"),
                        Title = reader.GetString("Title"),
                        Price = reader.GetDecimal("Price"),
                        CategoryID = reader.GetUInt64("CategoryID"),
                        IsActive = reader.GetBoolean("IsActive"),
                        CategoryTitle = reader.GetString("CategoryTitle")
                    };
                    dishes.Add(dish);
                }
                return ServiceResult<List<Dish>>.Success(dishes);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ServiceResult<List<Dish>>.Failure($"Непредвиденная ошибка: {ex.Message}");
            }
        }
    public ServiceResult<bool> Create(string title, decimal price, ulong categoryID, bool isActive)
    {
        string query = """
            INSERT INTO Dishes (Title, Price, CategoryID, IsActive)
            VALUES (@title, @price, @categoryID, @isActive)
            """;
    
        using var connection = new MySqlConnection(ConnectionString);
        using var command = new MySqlCommand(query, connection);
    
        command.Parameters.AddWithValue("@title", title);
        command.Parameters.AddWithValue("@price", price);
        command.Parameters.AddWithValue("@categoryID", categoryID);
        command.Parameters.AddWithValue("@isActive", isActive);
    
        try
        {
            connection.Open();
            int affectedRows = command.ExecuteNonQuery();
            return ServiceResult<bool>.Success(affectedRows > 0);
        }
        catch (Exception ex)
        {
            LogError(ex);
            return ServiceResult<bool>.Failure($"Непредвиденная ошибка: {ex.Message}");
        }
        }
    }
}
