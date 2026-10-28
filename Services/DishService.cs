using CoffeeInformationSystem.Enums;
using CoffeeInformationSystem.Models;
using MySql.Data.MySqlClient;

namespace CoffeeInformationSystem.Services
{
    public sealed class DishService : BaseService
    {
        public ServiceResult<List<Dish>> GetByAvailability(Availability state)
        {
            var dishes = new List<Dish>();
            string query = """
                SELECT Dishes.ID AS ID, Dishes.Title AS Title, Dishes.Price AS Price, Dishes.CategoryID AS CategoryID, Dishes.IsActive AS IsActive, Categories.Title AS CategoryTitle 
                FROM Dishes
                JOIN Categories ON Dishes.CategoryID = Categories.ID
                WHERE Dishes.IsActive = @isActive
                ORDER BY Dishes.ID; 
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@isActive", (byte)state);

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
                        CategoryTitle = reader.GetString("CategoryTitle"),
                        Quantity = 0
                    };
                    dishes.Add(dish);
                }
                return ServiceResult<List<Dish>>.Success(dishes);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ServiceResult<List<Dish>>.Failure($"Непредвиденная ошибка при поиске блюд по доступности: {ex.Message}");
            }
        }

        public ServiceResult<bool> ValidateExist(List<ulong> dishIDs)
        {
            if (dishIDs.Count == 0 || dishIDs == null)
            {
                return ServiceResult<bool>.Success(true);
            }

            var uniqueIDs = dishIDs.Distinct().ToList();
            var paramNames = uniqueIDs.Select((id, index) => $"@id{index}").ToList();

            string inClause = string.Join(", ", paramNames);
            string query = $"""
                SELECT COUNT(Dishes.ID)
                FROM Dishes
                WHERE Dishes.IsActive = 1 AND WHERE Dishes.ID IN ({inClause});
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            for (int i = 0; i < uniqueIDs.Count; i++)
            {
                command.Parameters.AddWithValue(paramNames[i], uniqueIDs[i]);
            }

            try
            {
                connection.Open();
                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count != uniqueIDs.Count)
                {
                    return ServiceResult<bool>.Failure("Одно или несколько выбранных блюд не найдены в меню.");
                }

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ServiceResult<bool>.Failure($"Непредвиденная ошибка валидации блюд: {ex.Message}");
            }
        }

        public ServiceResult<Dish> GetActiveByID(ulong dishID)
        {
            string query = """
                SELECT Dishes.ID AS ID, Dishes.Title AS Title, Price, CategoryID, IsActive, Categories.Title AS CategoryTitle FROM Dishes
                JOIN Categories ON Dishes.CategoryID = Categories.ID
                WHERE Dishes.ID = @id AND IsActive = 1
                LIMIT 1;
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@id", dishID);

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
                return ServiceResult<Dish>.Failure($"Непредвиденная ошибка поиска блюда: {ex.Message}");
            }
        }

        public ServiceResult<List<Dish>> GetActiveByIDs(List<ulong> dishIDs)
        {            
            var uniqueIDs = dishIDs.Distinct().ToList();
            var paramNames = uniqueIDs.Select((id, index) => $"@id{index}").ToList();
            string inClause = string.Join(", ", paramNames);

            var dishes = new List<Dish>();

            string query = $"""
                SELECT Dishes.ID AS ID, Dishes.Title AS Title, Price, CategoryID, IsActive, Categories.Title AS CategoryTitle FROM Dishes
                JOIN Categories ON Dishes.CategoryID = Categories.ID
                WHERE Dishes.ID IN ({inClause}) AND IsActive = 1
                ORDER BY Dishes.ID;
                """;

            var connection = new MySqlConnection(ConnectionString);
            var command = new MySqlCommand(query, connection);

            for (int i = 0; i < uniqueIDs.Count; i++)
            {
                command.Parameters.AddWithValue(paramNames[i], uniqueIDs[i]);
            }

            try
            {
                connection.Open();
                var reader = command.ExecuteReader();

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
                return ServiceResult<List<Dish>>.Failure($"Ошибка получения блюд: {ex.Message}");
            }
        }

        public ServiceResult<List<Dish>> GetByOrderID(ulong orderID)
        {
            List<Dish> dishes = new List<Dish>();
            string query = """
                SELECT Dishes.ID AS ID, Dishes.Title AS Title, Dishes.Price, Dishes.CategoryID, Dishes.IsActive AS IsActive, Categories.Title AS CategoryTitle, DishesOrders.Quantity AS Quantity 
                FROM Dishes
                JOIN Categories ON Dishes.CategoryID = Categories.ID
                JOIN DishesOrders ON Dishes.ID = DishesOrders.DishID
                WHERE DishesOrders.OrderID = @orderID;
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@orderID", orderID);

            try
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Dish dish = new()
                    {
                        ID = reader.GetUInt64("ID"),
                        Title = reader.GetString("Title"),
                        Price = reader.GetDecimal("Price"),
                        CategoryID = reader.GetUInt64("CategoryID"),
                        IsActive = reader.GetBoolean("IsActive"),
                        CategoryTitle = reader.GetString("CategoryTitle"),
                        Quantity = reader.GetUInt16("Quantity")
                    };

                    dishes.Add(dish);
                }
                return ServiceResult<List<Dish>>.Success(dishes);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ServiceResult<List<Dish>>.Failure($"Непредвиденная ошибка поиска блюда по ID заказа: {ex.Message}");
            }
        }

        public ServiceResult<bool> ToggleActive(ulong id)
        {
            string query = """
                UPDATE Dishes
                SET IsActive = NOT IsActive
                WHERE Dishes.ID = @id;
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
                return ServiceResult<bool>.Failure($"Непредвиденная ошибка переключения активности блюда: {ex.Message}");
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
                return ServiceResult<List<Dish>>.Failure($"Непредвиденная ошибка получения списка всех блюд: {ex.Message}");
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
            return ServiceResult<bool>.Failure($"Непредвиденная ошибка создания блюда: {ex.Message}");
        }
        }
    }
}
