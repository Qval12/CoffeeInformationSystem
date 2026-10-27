namespace CoffeeInformationSystem.Models
{
    public sealed class Category
    {
        public ulong ID { get; set; }
        public string Title { get; set; } = string.Empty;

        public Category() { }

        public Category(string title)
        {
            Title = title;
        }
    }
}
