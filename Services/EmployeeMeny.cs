using CoffeeInformationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeInformationSystem.Services
{
    internal class EmpoloyeeMenu
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
                    AdministratorMenu();
                    break;

                case (ulong)Roles.Cashier:
                    CashierMenu();
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
            Console.WriteLine("3. Управление сменами");
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

        private void AdministratorMenu()
        {
            Console.WriteLine("Меню Администратора");
            Console.WriteLine("1. Управление категориями блюд");
            Console.WriteLine("2. Управление блюдами");
            Console.WriteLine("3. Управление сотрудниками");
            Console.WriteLine("4. Управление сменами");
            Console.WriteLine("5. Выход");
            int choiсe;
            while (!int.TryParse(Console.ReadLine(), out choiсe))
            {
                Console.Clear();
                Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.");
            }
            if (choiсe > 5 || choiсe < 1)
            {
                Console.Clear();
                Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.(от 1 до 5)");
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

                    case 5:

                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.");
                        break;
                }
            }

        }

        private void CashierMenu()
        {
            Console.WriteLine("Меню Кассира");
            Console.WriteLine("1. Принять заказ");
            Console.WriteLine("2. выдать заказ");
            Console.WriteLine("3. Выход");
            int choiсe;
            while (!int.TryParse(Console.ReadLine(), out choiсe))
            {
                Console.Clear();
                Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.");
            }
            if (choiсe > 3 || choiсe < 1)
            {
                Console.Clear();
                Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.(от 1 до 3)");
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

                    default:
                        Console.Clear();
                        Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.");
                        break;
                }
            }

        }

        private void CookMenu()
        {
            Console.WriteLine("Меню Повара");
            Console.WriteLine("1. Просмотр заказов");
            Console.WriteLine("2. Заказ готовится");
            Console.WriteLine("3. Заказ готов");
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
