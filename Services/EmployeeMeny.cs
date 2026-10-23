using CoffeeInformationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeInformationSystem.Services
{
    internal class EmpoloyeeMeny
    {
        public void ShowMenu()
        {
            Employee user = new Employee();
            switch (user.RoleId)
            {
                case (ulong)Roles.Supervisor:
                    SupervisorMenu();
                    break;

                case (ulong)Roles.Administrator:

                    break;

                case (ulong)Roles.Cashier:

                    break;

                case (ulong)Roles.Cook:

                    break;

                default:
                    Console.Clear();
                    Console.WriteLine("Нет данной роли. Обратитесь к администратору.");
                    break;
            }
        }
        private void SupervisorMenu()
        {
            Console.Clear();
            Console.WriteLine("Меню Супервайзера");
            Console.WriteLine("1. Просмотр статистики");
            Console.WriteLine("2. Управление сотрудниками");
            Console.WriteLine("3. Настройки системы");
            Console.WriteLine("4. Выход");
            int choiсe;
            while (!int.TryParse(Console.ReadLine(), out choiсe))
            {
                Console.Clear();
                Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.");
            }
            if (choiсe > 4 || choiсe < 1)
            {
                Console.Clear();
                Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.(от 1 до 4)");
            }
            else
            {
                switch (choiсe)
                {
                    case 1:

                        break;

                    case 2:
                        break;

                    case 3:

                        break;

                    case 4:

                        break;

                    default:
                        Console.Clear();
                        Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.");
                        break;
                }
            }



        }



    }
}
