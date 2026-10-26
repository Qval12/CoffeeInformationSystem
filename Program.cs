using CoffeeInformationSystem.Services;

namespace CoffeeInformationSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AuthService authService = new();
            EmpoloyeeMenu empoloyeeMenu = new();
            empoloyeeMenu.ShowMenu(authService.LoginEmployee("ivovich", "popovich"));
        }
    }
}
