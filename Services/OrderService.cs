using CoffeeInformationSystem.Enums;
using CoffeeInformationSystem.Models;
using MySql.Data.MySqlClient;

namespace CoffeeInformationSystem.Services
{
    public sealed class OrderService : BaseService
    {
        public ServiceResult<List<Order>> GetByStatus(Status status)
        {
            var orders = new List<Order>();

            string query = """
                SELECT Orders.ID AS ID, CashierID, StatusID, Employees.Name AS EmployeeName, Statuses.Title AS StatusTitle, CreatedAt FROM Orders
                JOIN Employees ON Orders.CashierID = Employees.ID
                JOIN Statuses ON Orders.StatusID = Statuses.ID                
                WHERE Orders.StatusID = @status
                ORDER BY Orders.ID                
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@status", (ulong)status);

            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Order order = new()
                    {
                        ID = reader.GetUInt64("ID"),
                        CashierID = reader.GetUInt64("CashierID"),
                        StatusID = reader.GetUInt64("StatusID"),
                        EmployeeName = reader.GetString("EmployeeName"),
                        StatusTitle = reader.GetString("StatusTitle"),
                        CreatedAt = reader.GetDateTime("CreatedAt")
                    };
                    orders.Add(order);
                }
                return ServiceResult<List<Order>>.Success(orders);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ServiceResult<List<Order>>.Failure($"Произошла непредвиденная ошибка при получении заказа по статусу: {ex.Message}");
            }
        }
        

        public ServiceResult<Order> GetActiveByID(ulong orderID)
        {
            string query = """
                SELECT Orders.ID AS ID, CashierID, StatusID, Employees.Name AS EmployeeName, Statuses.Title AS StatusTitle, CreatedAt FROM Orders
                JOIN Employees ON Orders.CashierID = Employees.ID
                JOIN Statuses ON Orders.StatusID = Statuses.ID                
                WHERE Orders.ID = @orderID AND Statuses.Title = 'Открыт'
                ORDER BY ID
                LIMIT 1;
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@orderID", orderID);            

            try
            {
                connection.Open();
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Order order = new()
                    {
                        ID = reader.GetUInt64("ID"),
                        CashierID = reader.GetUInt64("CashierID"),
                        StatusID = reader.GetUInt64("StatusID"),
                        EmployeeName = reader.GetString("EmployeeName"),
                        StatusTitle = reader.GetString("StatusTitle"),
                        CreatedAt = reader.GetDateTime("CreatedAt")
                    };
                    return ServiceResult<Order>.Success(order);
                }
                return ServiceResult<Order>.Failure("Не удалось найти заказ по ID.");
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ServiceResult<Order>.Failure($"Произошла непредвиденная ошибка при получении заказа по ID: {ex.Message}");
            }
        }

        public ServiceResult<bool> ChangeStatus(ulong orderID, Status status)
        {
            string query = """
                UPDATE Orders
                SET StatusID = @statusID
                WHERE Orders.ID = @id;
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@statusID", (ulong)status);
            command.Parameters.AddWithValue("@id", orderID);

            try
            {
                connection.Open();
                int affectedRows = command.ExecuteNonQuery();
                if (affectedRows == 0)
                {
                    return ServiceResult<bool>.Failure($"Заказ {orderID} не найден.");
                }
                return ServiceResult<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Непредвиденная ошибка при смене статуса: {ex.Message}");
            }
        }

        public ServiceResult<ulong> CreateEmpty(ulong cashierID)
        {
            string query = """
                INSERT INTO Orders(CashierID, CreatedAt)
                VALUES (@cashierID, @current_timestamp);
                SELECT LAST_INSERT_ID();
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@cashierID", cashierID);
            command.Parameters.AddWithValue("@current_timestamp", DateTime.Now);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    ulong newID = Convert.ToUInt64(result);
                    return ServiceResult<ulong>.Success(newID);
                }

                return ServiceResult<ulong>.Failure("Не удалось получить ID созданного заказа");
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ServiceResult<ulong>.Failure($"Непредвиденная ошибка: {ex.Message}");
            }
        }

        public ServiceResult<ulong> CreateWithDishes(ulong cashierID, List<Dish> dishes)
        {
            if (dishes == null || dishes.Count == 0)
            {
                return ServiceResult<ulong>.Failure("Невозможно создать заказ без блюд.");
            }

            List<ulong> dishIDs = dishes.Select(dish => dish.ID).ToList();
            DishService dishService = new();

            var validation = dishService.ValidateExist(dishIDs);

            if (!validation.IsSuccess)
            {
                return ServiceResult<ulong>.Failure(validation.ErrorMessage);
            }
            
            using var connection = new MySqlConnection(ConnectionString);

            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ServiceResult<ulong>.Failure($"Ошибка подключения к БД: {ex.Message}");
            }

            using var transaction = connection.BeginTransaction();

            try
            {
                string orderQuery = """
                    INSERT INTO Orders (CashierID, CreatedAt)
                    VALUES (@cashierID, @currentTimestamp);
                    SELECT LAST_INSERT_ID();
                    """;

                using var orderCommand = new MySqlCommand(orderQuery, connection, transaction);

                orderCommand.Parameters.AddWithValue("@cashierID", cashierID);
                orderCommand.Parameters.AddWithValue("@currentTimestamp", DateTime.Now);

                object result = orderCommand.ExecuteScalar();

                ulong orderID = (result != null) && (result != DBNull.Value) ? Convert.ToUInt64(result) : 0;

                if (orderID == 0)
                {
                    transaction.Rollback();
                    return ServiceResult<ulong>.Failure("Не удалось создать заказ.");
                }

                if (dishes == null || dishes.Count == 0)
                {
                    transaction.Rollback();
                    return ServiceResult<ulong>.Failure("Блюда не найдены");
                }

                string dishQuery = """
                    INSERT INTO DishesOrders(OrderID, DishID, Quantity)
                    VALUES (@orderID, @dishID, @quantity);
                    """;

                using var dishCommand = new MySqlCommand(dishQuery, connection, transaction);

                dishCommand.Parameters.Add("@orderID", MySqlDbType.UInt64).Value = orderID;
                dishCommand.Parameters.Add("@dishID", MySqlDbType.UInt64);
                dishCommand.Parameters.Add("@quantity", MySqlDbType.UInt16);

                foreach (var dish in dishes)
                {
                    if (dish.Quantity <= 0)
                    {
                        transaction.Rollback();
                        return ServiceResult<ulong>.Failure($"Ошибка: для блюда \"{dish.Title}\" было указано некорректное количество позиций: ({dish.Quantity} шт.)");
                    }

                    dishCommand.Parameters["@dishID"].Value = dish.ID;
                    dishCommand.Parameters["@quantity"].Value = dish.Quantity;

                    dishCommand.ExecuteNonQuery();
                }
                transaction.Commit();
                return ServiceResult<ulong>.Success(orderID);
            }
            catch (Exception ex)
            {
                try { transaction.Rollback(); }
                catch
                {
                    
                }
                LogError(ex);
                return ServiceResult<ulong>.Failure($"Непредвиденная ошибка добавления блюд в заказ: {ex.Message}");
            }
        }
    }
}
