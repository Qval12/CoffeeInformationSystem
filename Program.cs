using CoffeeInformationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using CoffeeInformationSystem.Services;

namespace CoffeeInformationSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AuthService authService = new();
            authService.RegistrateEmployee("Ермолаев Иван", "erevan", "erevanpassword", Roles.Cashier);
        }
    }
}
