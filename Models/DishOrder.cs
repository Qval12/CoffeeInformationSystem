namespace CoffeeInformationSystem.Models
{
    public sealed class DishOrder
    {
        public ulong ID { get; set; }
        public ulong DishID { get; set; }
        public ulong OrderID { get; set; }
        public short DishQuantity { get; set; }
        public decimal Cost { get; set; }

        public DishOrder() { }

        public DishOrder(ulong dishID, ulong orderID, short dishQuantity, decimal cost)
        {
            DishID = dishID;
            OrderID = orderID;
            DishQuantity = dishQuantity;
            Cost = cost;
        }
    }
}
