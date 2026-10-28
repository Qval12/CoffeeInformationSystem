namespace CoffeeInformationSystem.Models
{
    public sealed class Order
    {
        public ulong ID { get; set; }
        public ulong CashierID { get; set; }
        public ulong? StatusID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; } = default;
        public string EmployeeName { get; set; } = string.Empty;
        public string StatusTitle {  get; set; } = string.Empty;

        public Order() { }

        public Order(ulong cashierID)
        {
            CashierID = cashierID;
            CreatedAt = DateTime.Now;
        }
    }
}
