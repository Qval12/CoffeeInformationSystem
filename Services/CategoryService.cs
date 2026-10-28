using CoffeeInformationSystem.Models;
using MySql.Data.MySqlClient;

namespace CoffeeInformationSystem.Services
{
    public sealed class CategoryService : BaseService
    {
        public ServiceResult<List<Category>> OutputAll()
        {
            var categories = new List<Category>();

            string query = """
                SELECT ID, Title FROM Categories;
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            try
            {
                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Category category = new()
                    {
                        ID = reader.GetUInt64("ID"),
                        Title = reader.GetString("Title")
                    };
                    categories.Add(category);
                }
                return ServiceResult<List<Category>>.Success(categories);
            }
            catch (Exception ex)
            {
                LogError(ex);
                return ServiceResult<List<Category>>.Failure($"Непредвиденная ошибка: {ex.Message}");
            }
        }
        public ServiceResult<bool> Create(string title)
        {
            string query = """
                INSERT INTO Categories (Title)
                VALUES (@title);
                """;

            using var connection = new MySqlConnection(ConnectionString);
            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@title", title);

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
