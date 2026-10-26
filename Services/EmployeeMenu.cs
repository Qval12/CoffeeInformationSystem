using CoffeeInformationSystem.Configs;
using CoffeeInformationSystem.Models;
using MySql.Data.MySqlClient;
using System.Data;
using System.Runtime.CompilerServices;
using CoffeeInformationSystem.Services;

namespace CoffeeInformationSystem.Services
{
    public sealed class EmpoloyeeMenu
    {
        DatabaseConfig dbConfig = new();
        public Employee usingEmployee = new();
        public EmpoloyeeMenu()
        {
            dbConfig = new DatabaseConfig();
            AuthService authService = new();
            Employee newEmployee = new();
            Console.Write("Логин: ");
            string login = Console.ReadLine();
            Console.Write("Пароль: ");
            string pass = Console.ReadLine();
            usingEmployee = authService.LoginEmployee(login, pass);
        }
        public void ShowMenu(Employee usingEmployee)
        {            
            using var connection = new MySqlConnection(dbConfig.connectionText);
            connection.Open();
            using var command = new MySqlCommand("""
                SELECT Login, PasswordHash FROM Employees
                WHERE @login = Login AND @password_hash = PasswordHash
                LIMIT 1;
                """, connection);
            command.Parameters.AddWithValue("@login", usingEmployee.Login);
            command.Parameters.AddWithValue("@password_hash", usingEmployee.PasswordHash);
            var reader = command.ExecuteReader();
            if (!reader.HasRows)
            {
                Console.WriteLine("Такого работника не существует в целом");
                usingEmployee.RoleId = 99;
            }
            switch (usingEmployee.RoleId)
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
                    CookMenu();
                    break;

                case 99:
                    break;

                default:
                    Console.Clear();
                    Console.WriteLine("Нет данной роли. Обратитесь к администратору.");
                    break;
            }
        }

        private void SupervisorMenu()
        {
            Console.WriteLine("Меню Супервайзера");
            Console.WriteLine("1. Просмотр статистики");
            Console.WriteLine("2. Выход");
            Console.Write("\nВведите: ");
            int choiсe;
            while (!int.TryParse(Console.ReadLine(), out choiсe) || (choiсe > 2 || choiсe < 1))
            {
                Console.Clear();
                Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.");
                Console.WriteLine("1. Просмотр статистики");
                Console.WriteLine("2. Выход");
                Console.Write("\nВведите: ");
            }
            switch (choiсe)
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("1. Сортировка блюд по количеству продаж");
                    Console.WriteLine("2. Все продажи в определенный период");
                    Console.WriteLine("3. Назад");
                    Console.Write("\nВведите: ");
                    int userInput;
                    while (!int.TryParse(Console.ReadLine(), out userInput) || (userInput > 3 || userInput < 1))
                    {
                        Console.Clear();
                        Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.");
                        Console.WriteLine("1. Сортировка блюд по количеству продаж");
                        Console.WriteLine("2. Все продажи в определенный период");
                        Console.WriteLine("3. Назад");
                        Console.Write("\nВведите: ");
                    }
                    switch (userInput)
                    {
                        case 1:
                            Console.Clear();
                            using (var connection = new MySqlConnection(dbConfig.connectionText))
                            {
                                connection.Open();
                                using (var command = new MySqlCommand(" SELECT Dishes.Title, SUM(DishQuantity) AS Amount" +
                                    " FROM DishesOrders " +
                                    "JOIN Dishes ON DishesOrders.DishId = Dishes.Id " +
                                    "GROUP BY Dishes.Title " +
                                    "ORDER BY Amount DESC;", connection))
                                {
                                    using (var reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Console.WriteLine($"{reader[0]}: {reader[1]} шт.");
                                        }
                                    }
                                }
                            }
                            break;
                        case 2:
                            Console.Clear();
                            string? dateStartStr = "";
                            while (String.IsNullOrEmpty(dateStartStr))
                            {
                                Console.Write("Дата начала (ДД/ММ/ГГГГ): ");
                                dateStartStr = Console.ReadLine();
                            }
                            DateTime dateStart = DateTime.Parse(dateStartStr);
                            string? dateEndStr = "";
                            while (String.IsNullOrEmpty(dateEndStr))
                            {
                                Console.Write("Дата конца (ДД/ММ/ГГГГ): ");
                                dateEndStr = Console.ReadLine();
                            }
                            DateTime dateEnd = DateTime.Parse(dateEndStr);
                            using (var connection = new MySqlConnection(dbConfig.connectionText))
                            {
                                connection.Open();
                                using (var command = new MySqlCommand("" +
                                    "SELECT Dishes.Title, SUM(DishQuantity) AS Amount FROM DishesOrders"
                                    + " JOIN Dishes ON DishesOrders.DishId = Dishes.Id"
                                    + " JOIN Payments ON DishesOrders.OrderId = Payments.OrderId"
                                    + " WHERE Payments.CreatedAt BETWEEN @dateStart AND @dateEnd"
                                    + " GROUP BY Dishes.Title"
                                    + " ORDER BY Amount DESC;"
                                    , connection))
                                {
                                    command.Parameters.AddWithValue("@dateStart", dateStart);
                                    command.Parameters.AddWithValue("@dateEnd", dateEnd);
                                    using (var reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            Console.WriteLine($"{reader[0]}: {reader[1]} шт.");
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            break;

                    }
                    break;


                case 2:

                    break;


                default:
                    Console.Clear();
                    Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.");
                    break;
            }

        }

        private void AdministratorMenu()
        {
            Console.WriteLine("Меню Администратора");
            Console.WriteLine("1. Управление категориями блюд");
            Console.WriteLine("2. Управление блюдами");
            Console.WriteLine("3. Выход");
            Console.Write("\nВведите: ");
            int choiсe;
            while (!int.TryParse(Console.ReadLine(), out choiсe) || (choiсe > 3 || choiсe < 1))
            {
                Console.Clear();
                Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова: ");
            }

            switch (choiсe)
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("1. Добавить категорию\n2. Добавить блюдо в категорию\n3. Переключть категорию\n4. Назад\n");
                    int userChoice = -1;
                    Console.Write("Введите действие: ");
                    while (!int.TryParse(Console.ReadLine(), out userChoice) || (userChoice > 3 || userChoice < 1))
                    {
                        Console.Clear();
                        Console.WriteLine("Недопустимое значение!\n\n1. Добавить категорию\n2. Добавить блюдо в категорию\n 3. Переключить категорию\n4. Назад\n");
                        Console.Write("Введите действие: ");
                    }
                    switch (userChoice)
                    {
                        case 1:
                            Console.Clear();
                            string? categoryName = "";
                            while (String.IsNullOrEmpty(categoryName))
                            {
                                Console.Write("Введите название категории: ");
                                categoryName = Console.ReadLine();
                            }
                            using (var connection = new MySqlConnection(dbConfig.connectionText))
                            {
                                connection.Open();
                                using (var command = new MySqlCommand("""                                    
                                    INSERT INTO Categories(Title)
                                    VALUES (@categoryName);                                    
                                    """, connection))
                                {
                                    command.Parameters.AddWithValue("@categoryName", categoryName);
                                    int affectedRows = command.ExecuteNonQuery();
                                }
                            }
                            break;
                        case 2:
                            Console.Clear();
                            int dishChoice = -1;
                            int categoryChoice = -1;
                            using (var connection = new MySqlConnection(dbConfig.connectionText))
                            {
                                Console.WriteLine("Блюда:");
                                connection.Open();
                                using (var command = new MySqlCommand("""
                                    SELECT Dishes.Id, Dishes.Title, Categories.Title FROM Dishes
                                    LEFT JOIN Categories ON Dishes.CategoryId = Categories.Id
                                    ORDER BY Id ASC;
                                    """, connection))
                                {
                                    var reader = command.ExecuteReader();
                                    if (!reader.HasRows)
                                    {
                                        Console.WriteLine("Нет блюд!");
                                    }
                                    List<string> rows = new List<string>();

                                    int counter = 0;
                                    while (reader.Read())
                                    {
                                        rows.Add(reader.GetUInt64(0).ToString() + ". " + reader.GetString(1) + " - " + reader.GetString(2));
                                        Console.WriteLine($"{rows[counter++]}");
                                    }
                                    Console.Write("Выбор блюда по id: ");
                                    while (!int.TryParse(Console.ReadLine(), out dishChoice))
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Недопустимое значение!");
                                        foreach (var row in rows)
                                        {
                                            Console.WriteLine(row);
                                        }
                                        Console.Write("Выбор блюда по id: ");
                                    }
                                    reader.Close();
                                }
                                using (var command = new MySqlCommand("""
                                    SELECT * FROM Categories
                                    ORDER BY Id;
                                    """, connection))
                                {
                                    List<string> categories = new List<string>();

                                    var reader = command.ExecuteReader();
                                    if (!reader.HasRows)
                                    {
                                        Console.WriteLine("Нет категорий!");
                                        break;
                                    }
                                    Console.Clear();
                                    Console.WriteLine("Категории:");
                                    int counter = 0;
                                    while (reader.Read())
                                    {
                                        categories.Add(reader.GetUInt64(0).ToString() + ". " + reader.GetString(1));
                                        Console.WriteLine(categories[counter++]);
                                    }
                                    reader.Close();
                                    Console.Write("Введите номер категории: ");
                                    while (!int.TryParse(Console.ReadLine(), out categoryChoice) || (categoryChoice < 1 || categoryChoice > counter))
                                    {
                                        Console.Clear();
                                        Console.Write("Неверный ввод!\n\n");
                                        foreach (var category in categories)
                                        {
                                            Console.WriteLine(category);
                                        }
                                        Console.Write("Введите номер категории: ");
                                    }
                                }
                                using (var command = new MySqlCommand("""
                                    UPDATE Dishes                                    
                                    SET CategoryId = @categoryChoice
                                    WHERE Id = @id
                                    """, connection))
                                {
                                    command.Parameters.AddWithValue("@id", dishChoice);
                                    command.Parameters.AddWithValue("@categoryChoice", categoryChoice);

                                    int affectedRows = command.ExecuteNonQuery();
                                }
                            }
                            break;
                        case 3:
                            break;
                    }

                    break;

                case 2:
                    Console.Clear();
                    Console.WriteLine("1. Добавить блюдо\n2. Изменить блюдо\n3. Назад\n");
                    int сhoice = -1;
                    Console.Write("Введите действие: ");
                    while (!int.TryParse(Console.ReadLine(), out сhoice) || (сhoice > 3 || сhoice < 1))
                    {
                        Console.Clear();
                        Console.WriteLine("Недопустимое значение!\n\n1. Добавить блюдо\n2. Изменить блюдо\n3. Назад\n");
                        Console.Write("Введите действие: ");
                    }
                    switch (сhoice)
                    {
                        case 1:
                            Console.Clear();
                            Console.Write("Введите название блюда: ");
                            string? dishName = "";
                            while (String.IsNullOrEmpty(dishName))
                            {
                                Console.Clear();
                                Console.Write("Введите название блюда: ");
                                dishName = Console.ReadLine();
                            }
                            Console.Clear();
                            int price = -1;
                            Console.Write("Введите цену (например: 50): ");
                            while (!int.TryParse(Console.ReadLine(), out price))
                            {
                                Console.Clear();
                                Console.Write("Введите цену (например: 50): ");
                            }
                            Console.Clear();
                            Console.WriteLine("Категории:");
                            using (var connection = new MySqlConnection(dbConfig.connectionText))
                            {
                                connection.Open();
                                using (var command = new MySqlCommand("""
                                    SELECT * FROM Categories
                                    ORDER BY Id;
                                    """, connection))
                                {
                                    var reader1 = command.ExecuteReader();
                                    if (!reader1.HasRows)
                                    {
                                        Console.WriteLine("Нет категорий!");
                                        break;
                                    }
                                    while (reader1.Read())
                                    {
                                        Console.WriteLine($"{reader1[0]}. {reader1[1]}");
                                    }
                                }
                            }
                            int categoryChoice = -1;
                            bool isActive = false;
                            Console.Write("Введите номер категории: ");
                            while (!int.TryParse(Console.ReadLine(), out categoryChoice))
                            {
                                Console.Clear();
                                Console.Write("Введите номер категории: ");
                            }
                            Console.Clear();
                            Console.Write("Включить ли блюдо в меню сразу? (Д/Н)");
                            if (Console.ReadLine().ToLower() == "д") isActive = true;
                            else isActive = false;
                            using (var connection = new MySqlConnection(dbConfig.connectionText))
                            {
                                connection.Open();
                                using (var command = new MySqlCommand("""
                                    INSERT INTO Dishes(Title, Price, CategoryId, IsActive)
                                    VALUES (@title, @price, @category, @is_active)
                                    """, connection))
                                {
                                    command.Parameters.AddWithValue("@title", dishName);
                                    command.Parameters.AddWithValue("@price", price);
                                    command.Parameters.AddWithValue("@category", categoryChoice);
                                    command.Parameters.AddWithValue("@is_active", isActive);
                                    command.ExecuteNonQuery();
                                }
                            }
                            break;
                        case 2:
                            Console.Clear();
                            using (var connection = new MySqlConnection(dbConfig.connectionText))
                            {
                                connection.Open();
                                using (var command = new MySqlCommand("""
                                    SELECT * FROM Dishes
                                    ORDER BY Id;
                                    """, connection))
                                {
                                    var reader1 = command.ExecuteReader();
                                    if (!reader1.HasRows)
                                    {
                                        Console.WriteLine("Нет категорий!");
                                        break;
                                    }
                                    while (reader1.Read())
                                    {
                                        Console.WriteLine($"{reader1[0]}. {reader1[1]}");
                                    }

                                }
                            }
                            
                            Console.Write("Введите название блюда: ");
                            string? dishName1 = "";
                            while (String.IsNullOrEmpty(dishName1))
                            {
                                Console.Clear();
                                Console.Write("Введите название блюда: ");
                                dishName = Console.ReadLine();
                            }
                            Console.Clear();
                            Console.WriteLine("1. изменить название.\n 2. изменить цену\n 3. изменить категорию\n4. выключить блюдо.\n");
                            Console.WriteLine("что вы хотитие сделать?");
                            int choise = 0;
                            while (!int.TryParse(Console.ReadLine(), out choise) || (choise > 4 || choise < 1))
                            {
                                Console.Clear();
                                Console.WriteLine("Недопустимое значение!\n\n 1. изменить название.\\n 2. изменить цену\\n 3. изменить категорию\n4. переключить бдлюдо.\\n");
                                Console.Write("Введите действие: ");
                            }

                            switch(choise)
                            {
                                case 1:
                                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                                    {
                                        connection.Open();
                                        using (var command = new MySqlCommand("""
                                    SELECT * FROM Dishes
                                    ORDER BY Id;
                                    """, connection))
                                        {
                                            var reader2 = command.ExecuteReader();
                                            if (!reader2.HasRows)
                                            {
                                                Console.WriteLine("Нет категорий!");
                                                break;
                                            }
                                            while (reader2.Read())
                                            {
                                                Console.WriteLine($"{reader2[0]}. {reader2[1]}");
                                            }

                                        }
                                    }
                                    Console.WriteLine("Выберите блюдо");
                                    int UserChoise = 0;
                                    while(!int.TryParse(Console.ReadLine(), out UserChoise))
                                    {
                                        Console.WriteLine("Неверный ввод.");
                                    }

                                    Console.WriteLine("Введите новое название");
                                    string? NewdishName1 = "";
                                    while (String.IsNullOrEmpty(NewdishName1))
                                    {
                                        Console.Clear();
                                        Console.Write("Введите название блюда: ");
                                        NewdishName1 = Console.ReadLine();
                                    }
                                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                                    {
                                        using (var command = new MySqlCommand("""
                                        UPDATE Dishes
                                        SET Title = @title
                                        WHERE id = @id
                                        """, connection))
                                        {
                                            command.Parameters.AddWithValue("@title", NewdishName1);
                                            command.Parameters.AddWithValue("@id", UserChoise);
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                    break;
                                case 2:
                                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                                    {
                                        connection.Open();
                                        using (var command = new MySqlCommand("""
                                    SELECT * FROM Dishes
                                    ORDER BY Id;
                                    """, connection))
                                        {
                                            var reader2 = command.ExecuteReader();
                                            if (!reader2.HasRows)
                                            {
                                                Console.WriteLine("Нет блюд!");
                                                break;
                                            }
                                            while (reader2.Read())
                                            {
                                                Console.WriteLine($"{reader2[0]}. {reader2[1]}");
                                            }

                                        }
                                    }
                                    Console.WriteLine("Выберите блюдо");
                                    int UserChoise1 = 0;
                                    while (!int.TryParse(Console.ReadLine(), out UserChoise1))
                                    {
                                        Console.WriteLine("Неверный ввод.");
                                    }
                                    Console.WriteLine("Введите новую цену");
                                    decimal Newdishcost = -1;
                                    while (!int.TryParse(Console.ReadLine(), out choise) || choise < 0 )
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Недопустимое значение!\n");
                                        Console.Write("Введите цену: ");
                                    }
                                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                                    {
                                        using (var command = new MySqlCommand("""
                                        UPDATE Dishes
                                        SET Price = @price
                                        WHERE id = @id
                                        """, connection))
                                        {
                                            command.Parameters.AddWithValue("@price", Newdishcost);
                                            command.Parameters.AddWithValue("@id", UserChoise1);
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                    break;
                                case 3:

                                    //using (var connection = new MySqlConnection(dbConfig.connectionText))
                                    //{
                                    //    connection.Open();

                                    //    using (var command = new MySqlCommand("""
                                    //    SELECT * FROM Category
                                    //    ORDER BY Id;
                                    //    """, connection))
                                    //    {
                                    //        using (var reader1 = command.ExecuteReader())
                                    //        {
                                    //            if (!reader1.HasRows)
                                    //            {
                                    //                Console.WriteLine("Нет категорий!");
                                    //            }
                                    //            else
                                    //            {
                                    //                while (reader1.Read())
                                    //                {
                                    //                    Console.WriteLine(
                                    //                        $"{reader1[0]}. {reader1[1]}");
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //    connection.Close();
                                    //    Console.WriteLine("Введите новою категорию");
                                    //    string? NewdishCategory = "";
                                    //    while (String.IsNullOrEmpty(NewdishCategory))
                                    //    {
                                    //        Console.Clear();
                                    //        Console.Write("Введите название блюда: ");
                                    //        NewdishCategory = Console.ReadLine();
                                    //    }
                                    //    using (var connection1 = new MySqlConnection(dbConfig.connectionText))
                                    //    {
                                    //        using (var command = new MySqlCommand("""
                                    //    INSERT INTO Dishes(Title)
                                    //    VALUES (@title)
                                    //    """, connection))
                                    //        {
                                    //            command.Parameters.AddWithValue("@title", NewdishCategory);
                                    //            command.ExecuteNonQuery();
                                    //        }
                                    //    }
                                    //}
                                    break;
                                case 4:
                                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                                    {
                                        connection.Open();
                                        using (var command = new MySqlCommand("""
                                    SELECT * FROM Dishes
                                    ORDER BY Id;
                                    """, connection))
                                        {
                                            var reader2 = command.ExecuteReader();
                                            if (!reader2.HasRows)
                                            {
                                                Console.WriteLine("Нет блюд!");
                                                break;
                                            }
                                            while (reader2.Read())
                                            {
                                                Console.WriteLine($"{reader2[0]}. {reader2[1]}");
                                            }

                                        }
                                        connection.Close();
                                    }
                                    
                                    Console.WriteLine("Выберите блюдо");
                                    int UserChoise3 = 0;
                                    while (!int.TryParse(Console.ReadLine(), out UserChoise3))
                                    {
                                        Console.WriteLine("Неверный ввод.");
                                    }
                                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                                    {
                                        connection.Open();
                                        using (var command = new MySqlCommand("""
                                        UPDATE Dishes
                                        SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END
                                        WHERE Id = @id;
                                        """, connection))
                                        {
                                            command.Parameters.AddWithValue("@id", UserChoise3);
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                        break;
                            }

                            break;
                            
                    }
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

        private void CashierMenu()
        {
            Console.WriteLine("Меню Кассира");
            Console.WriteLine("1. Принять заказ");
            Console.WriteLine("2. выдать заказ");
            Console.WriteLine("3. Выход");
            Console.Write("\nВведите: ");
            int choiсe;
            while (!int.TryParse(Console.ReadLine(), out choiсe) || (choiсe > 3 || choiсe < 1))
            {
                Console.Clear();
                Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова: ");
            }
            switch (choiсe)
            {
                case 1:
                    Console.Clear();
                    List<string> dishes = new List<string>();
                    List<int> prices = new List<int>();
                    List<int> choosedDishes = new List<int>();
                    List<int> choosedQuantities = new List<int>();
                    int choosedOrder = 0;
                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                    {
                        connection.Open();
                        using (var command = new MySqlCommand("""
                                    SELECT Id, Title, Price FROM Dishes
                                    ORDER BY Id;
                                    """, connection))
                        {
                            var reader = command.ExecuteReader();
                            if (!reader.HasRows)
                            {
                                Console.WriteLine("Нет блюд!");
                            }
                            while (reader.Read())
                            {
                                dishes.Add($"{reader.GetUInt64(0)}. {reader.GetString(1)}");
                                prices.Add((int)reader.GetDecimal(3));
                            }
                        }
                        connection.Close();
                    }
                    
                    int dishChoice = -1;
                    int quantityChoice = -1;
                    bool orderDone = false;
                    while (!orderDone)
                    {
                        while (dishChoice < 0 || !int.TryParse(Console.ReadLine(), out dishChoice))
                        {
                            Console.Clear();
                            foreach (var dish in dishes)
                            {
                                Console.WriteLine(dish);
                            }
                            Console.Write("Выберите блюдо: ");
                        }
                        while (quantityChoice < 0 || !int.TryParse(Console.ReadLine(), out quantityChoice))
                        {
                            Console.Clear();                            
                            Console.Write("Количество позиций: ");
                        }
                        choosedQuantities.Add(quantityChoice);
                        choosedDishes.Add(dishChoice);
                        Console.Write("Завершить заказ? (Д/Н): ");
                        orderDone = Console.ReadLine().Trim().ToLower() == "д" ? true : false; 
                    }
                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                    {
                        connection.Open();
                        int lastOrder = -1;
                        using (var command = new MySqlCommand("""
                            SELECT MAX(OrderNumber) FROM Orders;
                            """, connection))
                        {
                            var reader = command.ExecuteReader();
                            if (reader.Read())
                            {
                                lastOrder = reader.GetInt32(0);
                            }
                        }
                        using (var command = new MySqlCommand("""
                                    INSERT INTO Orders(CashierId, OrderNumber, StatusId)
                                    VALUES (@id, @order_number, @status_id)
                                    """, connection))
                        {
                            command.Parameters.AddWithValue("@id", usingEmployee.Id);
                            command.Parameters.AddWithValue("@order_number", lastOrder + 1);
                            command.Parameters.AddWithValue("@status_id", 1);
                        }

                        using (var command = new MySqlCommand("""
                                    SELECT MAX(CreatedAt) FROM Orders(CashierId, OrderNumber, StatusId)
                                    LIMIT 1;
                                    """, connection))
                        {
                            var reader = command.ExecuteReader();
                            if (reader.Read())
                            {
                                choosedOrder = reader.GetInt32(0);
                            }
                        }
                        int counter = 0;
                        foreach (var dish in choosedDishes)
                        {
                            using (var command = new MySqlCommand("""
                                    INSERT INTO DishesOrders(DishId, OrderId, DishQuantity)
                                    VALUES (@id, @order_number, @status_id)
                                    """, connection))
                            {
                                command.Parameters.AddWithValue("@id", dish);
                                command.Parameters.AddWithValue("@order_number", choosedOrder);
                                command.Parameters.AddWithValue("@status_id", choosedQuantities[counter++]);
                            }
                        }
                        connection.Close();
                    }

                    break;

                case 2:
                    Console.Clear();
                    List<int> readyOrders = new List<int>();
                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                    {                        
                        connection.Open();
                        using (var command = new MySqlCommand("""
                                    SELECT Id FROM Orders
                                    WHERE StatusId = 3 AND ClosedAt IS NULL;
                                    ORDER BY Id;
                                    """, connection))
                        {
                            var reader = command.ExecuteReader();
                            if (!reader.HasRows)
                            {
                                Console.WriteLine("Нету заказов");
                                break;
                            }
                            while (reader.Read())
                            {
                                readyOrders.Add(reader.GetInt32(0));
                            }
                        }
                        connection.Close();
                    }
                    int orderChoice = -1;
                    while (orderChoice < 0 || int.TryParse(Console.ReadLine(), out orderChoice))
                    {
                        Console.Clear();
                        foreach (var order in readyOrders)
                        {
                            Console.WriteLine($"{order}. Заказ {order} приготовлен.");
                        }
                        Console.Write("Выберите заказ для выдачи: ");
                    }
                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                    {
                        connection.Open();
                        using (var command = new MySqlCommand("""
                                    UPDATE Orders
                                    SET ClosedAt = CURRENT_TIMESTAMP, StatusId = 4;
                                    WHERE Id = @order_id
                                    """, connection))
                        {
                            command.Parameters.AddWithValue("@order_id", orderChoice);
                            command.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                    foreach (var order in readyOrders)
                    {
                        Console.WriteLine(order);
                    }
                    break;

                case 3:

                    break;

                default:
                    Console.Clear();
                    Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова.");
                    break;
            }

        }

        private void CookMenu()
        {
            Console.WriteLine("Меню Повара");
            Console.WriteLine("1. Просмотр заказов");
            Console.WriteLine("2. Заказ готовится");
            Console.WriteLine("3. Заказ готов");
            Console.WriteLine("4. Выход");
            Console.Write("\nВведите: ");
            int choiсe;
            while (!int.TryParse(Console.ReadLine(), out choiсe) || (choiсe > 4 || choiсe < 1))
            {
                Console.Clear();
                Console.WriteLine("Неверный выбор. Пожалуйста, выберите снова: ");
            }

            switch (choiсe)
            {
                case 1:
                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                    {
                        connection.Open();
                        using (var command = new MySqlCommand("""
                                        SELECT
                                            o.Id AS OrderId,
                                            o.OrderNumber,
                                            d.Id AS DishId,
                                            d.Title,
                                            do.DishQuantity,
                                            s.Title AS Status
                                        FROM DishesOrders AS do
                                        JOIN Dishes AS d ON d.Id = do.DishId
                                        JOIN Orders AS o ON o.Id = do.OrderId
                                        JOIN Statuses AS s ON s.Id = o.StatusId
                                        WHERE s.Title IN ('Новый', 'Готовится')
                                        ORDER BY o.Id, d.Id;
                                        """, connection))
                        {
                            var reader2 = command.ExecuteReader();
                            if (!reader2.HasRows)
                            {
                                Console.WriteLine("Нет блюд!");
                                break;
                            }
                            while (reader2.Read())
                            {
                                Console.WriteLine($"{reader2[0]}. {reader2[1]}");
                            }
                        }
                    }

                        break;

                case 2:

                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                    {

                        using (var command = new MySqlCommand("""
                                        SELECT o.*
                                        FROM Orders AS o
                                        JOIN Statuses AS s ON s.Id = o.StatusId
                                        WHERE s.Title = 'Новый'
                                        ORDER BY o.Id;
                                        """, connection))
                        {
                            var reader2 = command.ExecuteReader();
                            if (!reader2.HasRows)
                            {
                                Console.WriteLine("Нет блюд!");
                                break;
                            }
                            while (reader2.Read())
                            {
                                Console.WriteLine($"{reader2[0]}. {reader2[1]}");
                            }
                        }
                        connection.Close();
                    }
                    Console.WriteLine("Выберите заказ");
                    int UserChoise = 0;
                    while (!int.TryParse(Console.ReadLine(), out UserChoise))
                    {
                        Console.WriteLine("Неверный ввод.");
                    }

                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                    {

                        using (var command = new MySqlCommand("""
                                        UPDATE Orders AS o
                                        JOIN Statuses AS currentStatus ON currentStatus.Id = o.StatusId
                                        JOIN Statuses AS nextStatus ON nextStatus.Title = 'Готовится'
                                        SET o.StatusId = nextStatus.Id
                                        WHERE o.Id = @id
                                          AND currentStatus.Title = 'Новый';
                                        """, connection))
                        {
                        }
                        connection.Close();
                    }
                    Console.WriteLine("Готово.");

                    break;

                case 3:
                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                    {

                        using (var command = new MySqlCommand("""
                                        SELECT o.*
                                        FROM Orders AS o
                                        JOIN Statuses AS s ON s.Id = o.StatusId
                                        WHERE s.Title = 'Готовится'
                                        ORDER BY o.Id;
                                        """, connection))
                        {
                            var reader2 = command.ExecuteReader();
                            if (!reader2.HasRows)
                            {
                                Console.WriteLine("Нет блюд!");
                                break;
                            }
                            while (reader2.Read())
                            {
                                Console.WriteLine($"{reader2[0]}. {reader2[1]}");
                            }
                        }
                        connection.Close();
                    }
                    Console.WriteLine("Выберите заказ");
                    int UserChoise1 = 0;
                    while (!int.TryParse(Console.ReadLine(), out UserChoise1))
                    {
                        Console.WriteLine("Неверный ввод.");
                    }

                    using (var connection = new MySqlConnection(dbConfig.connectionText))
                    {

                        using (var command = new MySqlCommand("""
                                        UPDATE Orders AS o
                                        JOIN Statuses AS currentStatus ON currentStatus.Id = o.StatusId
                                        JOIN Statuses AS nextStatus ON nextStatus.Title = 'Готов'
                                        SET o.StatusId = nextStatus.Id
                                        WHERE o.Id = @id
                                          AND currentStatus.Title = 'Готовится';
                                        """, connection))
                        {
                        }
                        connection.Close();
                    }
                    Console.WriteLine("Готово.");

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