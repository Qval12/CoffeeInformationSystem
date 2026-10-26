using MySql.Data.MySqlClient;

namespace CoffeeInformationSystem.Services;

internal sealed class ShiftService
{
    private readonly string _connectionString;

    public ShiftService(string connectionString)
    {
        _connectionString = connectionString;
    }

    private void ShowEmployeesAndShifts()
    {
        using var connection = new MySqlConnection(_connectionString);
        connection.Open();

        Console.WriteLine("\n=== Сотрудники ===");

        using (var command = new MySqlCommand(
            "SELECT Id, Name FROM Employees ORDER BY Id;", connection))
        using (var reader = command.ExecuteReader())
        {
            if (!reader.HasRows)
                Console.WriteLine("Сотрудников нет.");

            while (reader.Read())
                Console.WriteLine($"ID: {reader["Id"]} | {reader["Name"]}");
        }

        Console.WriteLine("\n=== Открытые смены ===");

        using (var command = new MySqlCommand("""
        SELECT Id, StartTime FROM Shifts
        WHERE EndTime IS NULL AND StartTime <= CURRENT_TIMESTAMP
        ORDER BY Id DESC;
        """, connection))
        using (var reader = command.ExecuteReader())
        {
            if (!reader.HasRows)
                Console.WriteLine("Открытых смен нет.");

            while (reader.Read())
            {
                Console.WriteLine(
                    $"ID: {reader["Id"]} | " +
                    $"Начало: {reader.GetDateTime(1):dd.MM.yyyy HH:mm}");
            }
        }
    }

    public void ManageShifts()
    {
        while (true)
        {
            Console.WriteLine("\n1. Назначить сотрудника на смену");
            Console.WriteLine("2. Снять сотрудника со смены");
            Console.WriteLine("0. Назад");
            Console.Write("Выберите действие: ");
            string? choice = Console.ReadLine()?.Trim();
            if (choice is null or "0") return;
            if (choice != "1" && choice != "2")
            {
                Console.WriteLine("Введите число от 0 до 2.");
                continue;
            }

            Console.Write("ID смены: ");
            if (!ulong.TryParse(Console.ReadLine(), out ulong shiftId) || shiftId == 0)
            {
                Console.WriteLine("Нужен положительный ID смены.");
                continue;
            }

            Console.Write("ID сотрудника: ");
            if (!ulong.TryParse(Console.ReadLine(), out ulong employeeId) || employeeId == 0)
            {
                Console.WriteLine("Нужен положительный ID сотрудника.");
                continue;
            }

            string sql = choice == "1"
                ? """
                    INSERT INTO EmployeesShifts
                        (EmployeeId, ShiftId, StartedAt, EndedAt)
                    SELECT e.Id, s.Id, CURRENT_TIMESTAMP, NULL
                    FROM Employees AS e
                    JOIN Shifts AS s ON s.Id = @shiftId
                    WHERE e.Id = @employeeId AND e.IsActive = 1
                      AND s.EndTime IS NULL AND s.StartTime <= CURRENT_TIMESTAMP;
                    """
                : """
                    UPDATE EmployeesShifts AS es
                    JOIN Shifts AS s ON s.Id = es.ShiftId
                    SET es.EndedAt = CURRENT_TIMESTAMP
                    WHERE es.EmployeeId = @employeeId AND es.ShiftId = @shiftId
                      AND es.EndedAt IS NULL AND s.EndTime IS NULL;
                    """;

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                connection.Open();
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@employeeId", employeeId);
                command.Parameters.AddWithValue("@shiftId", shiftId);

                int count = command.ExecuteNonQuery();
                Console.WriteLine(count > 0
                    ? "Готово. Время записано."
                    : "Изменений нет. Проверьте ID, состояние смены и сотрудника.");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                Console.WriteLine("Сотрудник уже был записан в эту смену.");
            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Не удалось выполнить действие. Код MySQL: {ex.Number}.");
            }
        }
    }
}