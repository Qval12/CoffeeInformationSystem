namespace CoffeeInformationSystem.Models
{
    public enum Roles
    {
        Supervisor = 1,
        Administrator = 2,
        Cashier = 3,
        Cook = 4
    }
    public sealed class Employee
    {
        public ulong Id { get; set; }
        public string Name { get; set; } = "";
        public string Login { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public ulong RoleId { get; set; }
        public bool IsActive { get; set; }

        public Employee(string name, string login, string passwordHash, Roles role)
        {
            Name = name;
            Login = login;
            PasswordHash = passwordHash;
            RoleId = (ulong)role;
        }

        public Employee()
        {
            Name = "";
            Login = "";
            PasswordHash = "";
        }
    }
}

