using CoffeeInformationSystem.Configs;

namespace CoffeeInformationSystem.Services
{
    public abstract class BaseService
    {
        protected readonly string ConnectionString = DatabaseConfig.connectionText;

        protected void LogError(Exception ex)
        {
            string serviceName = this.GetType().Name;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{serviceName} ERROR]: {ex.Message}");
            Console.ResetColor();
        }
    }
}
