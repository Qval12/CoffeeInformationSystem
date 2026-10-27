using CoffeeInformationSystem.Enums;
namespace CoffeeInformationSystem.Models
{    
    public sealed class Employee
    {
        public ulong ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Role RoleID { get; set; }
        public bool IsActive { get; set; }

        public Employee() { }

        public Employee(string name, string login, string passwordHash, Role role)
        {
            Name = name;
            Login = login;
            PasswordHash = passwordHash;
            RoleID = role;
        }        
    }
}

