using CoffeeInformationSystem.Enums;
using CoffeeInformationSystem.Models;
using CoffeeInformationSystem.Services;
using Google.Protobuf;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeInformationSystem.Services
{
    internal class EmployeeServise : BaseService
    {
        private void OutCategory()
        {
            CategoryService categoryService = new();
            var result = categoryService.OutputAll();

            if (result.IsSuccess)
            {
                foreach (var category in result.Data!)
                {
                    Console.WriteLine($"{category.ID}. {category.Title}");
                }
            }
            else
            {
                Console.WriteLine("Категории:");
                Console.WriteLine(result.ErrorMessage);
            }
        }
        private void OutDish()
        {
            DishService dishService = new();
            var result = dishService.OutputAll();

            if (!result.IsSuccess)
            {
                Console.WriteLine(result.ErrorMessage);
                return;
            }

            if (result.Data == null || result.Data.Count == 0)
            {
                Console.WriteLine("Блюд пока нет.");
                return;
            }

            Console.WriteLine("Блюда:");

            foreach (var dish in result.Data)
            {
                string availability = dish.IsActive ? "Доступно" : "Недоступно";

                Console.WriteLine(
                    $"{dish.ID}. {dish.Title} | " +
                    $"Цена: {dish.Price:F2} | " +
                    $"Категория: {dish.CategoryTitle} | {availability}");
            }
        }
        private ulong choiceOfDish()
        {
            DishService dishService = new();
            var result = dishService.OutputAll();

            if (!result.IsSuccess)
            {
                Console.WriteLine(result.ErrorMessage);
                return 0;
            }

            Console.Write("Введите ID блюда: ");
            string? input = Console.ReadLine();

            if (!ulong.TryParse(input, out ulong dishID) || dishID == 0)
            {
                Console.WriteLine("Нужно ввести положительное целое число.");
                return 0;
            }

            if (result.Data == null || !result.Data.Any(d => d.ID == dishID))
            {
                Console.WriteLine("Блюда с таким ID нет.");
                return 0;
            }

            return dishID;
        }

        private ulong choiseOfCatedory()
        {
            CategoryService categoryService = new();
            var result = categoryService.OutputAll();

            if (!result.IsSuccess)
            {
                Console.WriteLine(result.ErrorMessage);
                return 0;
            }

            Console.Write("Введите ID категории: ");
            string? input = Console.ReadLine();

            if (!ulong.TryParse(input, out ulong categoryID))
            {
                Console.WriteLine("Нужно ввести целое неотрицательное число.");
                return 0;
            }

            if (result.Data == null || !result.Data.Any(c => c.ID == categoryID))
            {
                Console.WriteLine("Категории с таким ID нет.");
                return 0;
            }

            return categoryID;
        }
        private string StrIsNullOrEmpty(string message)
        {
            string? str;
            Console.WriteLine(message);
            str = Console.ReadLine();
            while (string.IsNullOrEmpty(str))
            {
                Console.WriteLine("Неподдерживаемый формат.\n", message);
                str = Console.ReadLine();
            }
            return str;
        }


        public void Menu()
        {
            string message = "Введите логин: ";
            string login = StrIsNullOrEmpty(message);
            message = "Введите пароль: ";
            string password = StrIsNullOrEmpty(message);
            Console.Clear();
            AuthService authService = new();
            ServiceResult<Employee> loginResult = authService.Login(login, password);
            Employee employee = loginResult.Data;

            switch(employee.RoleID)
            {
                case Enums.Role.Supervisor:
                    SupervisorMenu();
                    break;
                case Enums.Role.Administrator:
                    AdminMenu();
                    break;
                case Enums.Role.Cashier:
                    CashierMenu(employee.ID);
                    break;
                case Enums.Role.Cook:
                    CookMenu();
                    break;
                default:
                    Console.WriteLine("Ошибка: неизвестная роль сотрудника.");
                    break;
            }
        }

        private void SupervisorMenu()
        {
            Console.WriteLine("Меню supervisor");
            Console.WriteLine("""

                    Меню владельца кафе
                    1. Зарегистрировать сотрудника
                    2. Посмотреть категории
                    3. Добавить категорию
                    4. Посмотреть все блюда
                    5. Добавить блюдо
                    6. Переключить доступность блюда
                    7. Выход.
                    """);
            Console.Write("Выберите действие: ");
            int choice;
            while(!int.TryParse(Console.ReadLine(), out choice) || (choice < 1 || choice > 7))
            {
                Console.WriteLine("Ошибка: введите число от 1 до 10.");
                Console.Write("Выберите действие: ");
            }
            switch(choice)
            {

                case 1:
                    string message = "Введите логин сотрудника: ";
                    string employeeLogin = StrIsNullOrEmpty(message);
                    message = "Введите пароль сотрудника: ";
                    string employeePassword = StrIsNullOrEmpty(message);
                    message = "Введите имя сотрудника: ";
                    string employeeName = StrIsNullOrEmpty(message);
                    message = "Выберите роль сотрудника /n (1. Supervisor, 2.Administrator, 3.Cashier, 4.Cook): ";
                    Console.WriteLine(message);
                    int employeeRoleInt;
                    while (!int.TryParse(Console.ReadLine(), out employeeRoleInt) && Enum.IsDefined(typeof(Role), employeeRoleInt))
                    {
                        Console.WriteLine("Ошибка: введите число от 1 до 4.");
                        Console.Write("Выберите роль сотрудника: ");
                    }
                    Role employeeRole = (Role)employeeRoleInt;
                    AuthService beb = new AuthService();
                    beb.RegisterEmployee(employeeName, employeeLogin, employeePassword, employeeRole);
                    break;

                case 2:
                    OutCategory();
                    break;
                case 3:
                    CategoryService case2 = new();
                    message = "Введите название новой категории";
                    string UserOut= StrIsNullOrEmpty(message);
                    case2.Create(UserOut);
                    break;
                case 4:
                    OutDish();
                    break;
                case 5:
                    DishService Incase2 = new();
                    message = "Введите название блюда: ";
                    string DishName = StrIsNullOrEmpty(message);
                    message = "Введите цену: ";
                    Console.WriteLine(message);
                    decimal DishCost;
                    while(!decimal.TryParse(Console.ReadLine(), out DishCost))
                    {
                        Console.WriteLine("Некоректное занчение", message);
                    }
                    ulong CategoryId = choiseOfCatedory();
                    Console.WriteLine("Блюдо будет активно?");
                    bool IsActive;
                    while (!bool.TryParse(Console.ReadLine(),out IsActive))
                    {
                        Console.WriteLine("Неверный ввод. Напишите (1 или 0)");
                    }
                    Incase2.Create(DishName, DishCost, CategoryId, IsActive);
                    break;

                case 6:
                    DishService Incase3 = new();
                    OutDish();
                    ulong choiceDish = choiceOfDish();
                    Incase3.ToggleActive(choiceDish);
                    break;
                default:
                    break;
            }
        }

        private void AdminMenu()
        {
            Console.Clear();
            Console.WriteLine("Меню администратора");
            Console.WriteLine("""

                    Меню владельца кафе
                    1. Посмотреть категории
                    2. Добавить категорию
                    3. Посмотреть все блюда
                    4. Добавить блюдо
                    5. Переключить доступность блюда
                    6. Выход.
                    """);
            int chice;
            while (!int.TryParse(Console.ReadLine(), out chice) || (chice < 1 || chice > 7))
            {
                Console.WriteLine("Ошибка: введите число от 1 до 10.");
                Console.Write("Выберите действие: ");
            }
            switch (chice)
            {
                case 1:
                    OutCategory();
                    break;
                case 2:
                    CategoryService case2 = new();
                    string message = "Введите название новой категории";
                    string UserOut = StrIsNullOrEmpty(message);
                    case2.Create(UserOut);
                    break;
                case 3:
                    OutDish();
                    break;
                case 4:
                    DishService Incase2 = new();
                    message = "Введите название блюда: ";
                    string DishName = StrIsNullOrEmpty(message);
                    message = "Введите цену: ";
                    Console.WriteLine(message);
                    decimal DishCost;
                    while (!decimal.TryParse(Console.ReadLine(), out DishCost))
                    {
                        Console.WriteLine("Некоректное занчение", message);
                    }
                    ulong CategoryId = choiseOfCatedory();
                    Console.WriteLine("Блюдо будет активно?");
                    bool IsActive;
                    while (!bool.TryParse(Console.ReadLine(), out IsActive))
                    {
                        Console.WriteLine("Неверный ввод. Напишите (1 или 0)");
                    }
                    Incase2.Create(DishName, DishCost, CategoryId, IsActive);
                    break;

                case 5:
                    DishService Incase3 = new();
                    OutDish();
                    ulong choiceDish = choiceOfDish();
                    Incase3.ToggleActive(choiceDish);
                    break;
                default:
                    break;
            }
        }

        private void CashierMenu(ulong cashierID)
        {
            Console.WriteLine("""
            
            Меню кассира
            1. Посмотреть доступные блюда
            2. Создать заказ
            3. Посмотреть открытые заказы
            4. Посмотреть состав и сумму заказа
            5. Отметить заказ как выданный
            6. Закрыть заказ
            7. Выйти
            """);

            Console.Write("Выберите действие: ");
            int choice =-1;


            switch (choice)
            {
                case 1:
                    {
                        DishService dishService = new();
                        var result = dishService.GetByAvailability(Availability.Active);

                        if (!result.IsSuccess)
                        {
                            Console.WriteLine(result.ErrorMessage);
                            break;
                        }

                        if (result.Data == null || result.Data.Count == 0)
                        {
                            Console.WriteLine("Доступных блюд нет.");
                            break;
                        }

                        foreach (var dish in result.Data)
                        {
                            Console.WriteLine(
                                $"{dish.ID}. {dish.Title} | " +
                                $"{dish.Price:F2} | {dish.CategoryTitle}");
                        }

                        break;
                    }

                case 2:
                    {
                        List<Dish> selectedDishes = new();

                        while (true)
                        {
                            Console.Write("ID блюда (0 — закончить заказ): ");
                            string? dishInput = Console.ReadLine();

                            if (dishInput == null) return;

                            if (!ulong.TryParse(dishInput, out ulong dishID))
                            {
                                Console.WriteLine("Некорректный ID.");
                                continue;
                            }

                            if (dishID == 0) break;
                            DishService dishService = new();
                            var dishResult = dishService.GetActiveByID(dishID);

                            if (!dishResult.IsSuccess || dishResult.Data == null)
                            {
                                Console.WriteLine(dishResult.ErrorMessage);
                                continue;
                            }

                            Console.Write("Количество: ");

                            if (!ushort.TryParse(Console.ReadLine(), out ushort quantity)
                                || quantity == 0)
                            {
                                Console.WriteLine("Введите количество от 1 до 65535.");
                                continue;
                            }

                            var existingDish = selectedDishes
                                .FirstOrDefault(d => d.ID == dishID);

                            if (existingDish != null)
                            {
                                int totalQuantity = existingDish.Quantity + quantity;

                                if (totalQuantity > ushort.MaxValue)
                                {
                                    Console.WriteLine("Общее количество превышает 65535.");
                                    continue;
                                }

                                existingDish.Quantity = (ushort)totalQuantity;
                            }
                            else
                            {
                                Dish dish = dishResult.Data;
                                dish.Quantity = quantity;
                                selectedDishes.Add(dish);
                            }

                            Console.WriteLine("Блюдо добавлено в заказ.");
                        }

                        if (selectedDishes.Count == 0)
                        {
                            Console.WriteLine("Заказ не создан: блюда не выбраны.");
                            break;
                        }
                        OrderService orderService = new OrderService();
                        var result = orderService.CreateWithDishes(
                            cashierID, selectedDishes);

                        Console.WriteLine(result.IsSuccess
                            ? $"Заказ №{result.Data} создан."
                            : result.ErrorMessage);

                        break;
                    }

                case 3:
                    {
                        OrderService orderService = new OrderService();
                        var result = orderService.GetByStatus(Status.Opened);

                        if (!result.IsSuccess)
                        {
                            Console.WriteLine(result.ErrorMessage);
                            break;
                        }

                        if (result.Data == null || result.Data.Count == 0)
                        {
                            Console.WriteLine("Открытых заказов нет.");
                            break;
                        }

                        foreach (var order in result.Data)
                        {
                            Console.WriteLine(
                                $"Заказ №{order.ID} | " +
                                $"Кассир: {order.EmployeeName} | " +
                                $"{order.CreatedAt:dd.MM.yyyy HH:mm}");
                        }

                        break;
                    }

                case 4:
                    {
                        Console.Write("Введите ID заказа: ");

                        if (!ulong.TryParse(Console.ReadLine(), out ulong orderID)
                            || orderID == 0)
                        {
                            Console.WriteLine("Некорректный ID заказа.");
                            break;
                        }

                        DishService dishService = new();
                        var result = dishService.GetByOrderID(orderID);

                        if (!result.IsSuccess)
                        {
                            Console.WriteLine(result.ErrorMessage);
                            break;
                        }

                        if (result.Data == null || result.Data.Count == 0)
                        {
                            Console.WriteLine("Заказ не найден или не содержит блюд.");
                            break;
                        }

                        decimal total = 0;

                        foreach (var dish in result.Data)
                        {
                            decimal sum = dish.Price * dish.Quantity;
                            total += sum;

                            Console.WriteLine(
                                $"{dish.Title}: {dish.Quantity} шт. × " +
                                $"{dish.Price:F2} = {sum:F2}");
                        }

                        Console.WriteLine($"Итого по текущим ценам меню: {total:F2}");
                        break;
                    }

                case 5:
                case 6:
                    {
                        Console.Write("Введите ID заказа: ");

                        if (!ulong.TryParse(Console.ReadLine(), out ulong orderID)
                            || orderID == 0)
                        {
                            Console.WriteLine("Некорректный ID заказа.");
                            break;
                        }

                        Status newStatus = choice == 5
                            ? Status.Issued
                            : Status.Closed;
                        OrderService orderService = new OrderService();
                        var result = orderService.ChangeStatus(orderID, newStatus);

                        Console.WriteLine(result.IsSuccess && result.Data
                            ? choice == 5
                                ? "Заказ отмечен как выданный."
                                : "Заказ закрыт."
                            : result.ErrorMessage);

                        break;
                    }

                case 7:
                    return;
            }
        }


        private void CookMenu()
        {
            OrderService service = new();

            while (true)
            {
                List<Order> orders = new();

                foreach (Status status in Enum.GetValues<Status>())
                {
                    if (status == Status.Closed) continue;

                    var result = service.GetByStatus(status);

                    if (!result.IsSuccess)
                    {
                        Console.WriteLine(result.ErrorMessage);
                        return;
                    }

                    if (result.Data != null)
                        orders.AddRange(result.Data);
                }

                Console.WriteLine("\nНезакрытые заказы:");

                foreach (var order in orders.OrderBy(o => o.ID))
                    Console.WriteLine($"№{order.ID} — {order.StatusTitle}");

                if (orders.Count == 0)
                    Console.WriteLine("Заказов нет.");

                Console.Write("ID готового заказа (Enter — обновить, 0 — выйти): ");
                string? input = Console.ReadLine();

                if (input == null || input.Trim() == "0") return;
                if (string.IsNullOrWhiteSpace(input)) continue;

                if (!ulong.TryParse(input, out ulong id)
                    || !orders.Any(o => o.ID == id
                        && o.StatusID == (ulong)Status.Opened))
                {
                    Console.WriteLine("Выберите заказ со статусом «Открыт».");
                    continue;
                }

                var change = service.ChangeStatus(id, Status.Ready);

                Console.WriteLine(change.IsSuccess && change.Data
                    ? "Заказ готов."
                    : change.ErrorMessage);
            }
        }
    }
}
