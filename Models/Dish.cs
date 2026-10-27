namespace CoffeeInformationSystem.Models
{
    public sealed class Dish
    {
        public ulong ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ulong CategoryID { get; set; }
        public bool IsActive { get; set; } = true;
        public string CategoryTitle { get; set; } = string.Empty;

        public Dish() { }

        public Dish(string title, decimal price, ulong categoryID)
        {
            Title = title;
            Price = price;
            CategoryID = categoryID;
            IsActive = true;
        }
    }
}
