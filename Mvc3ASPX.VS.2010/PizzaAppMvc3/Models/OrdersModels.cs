using System;
using System.Collections.Generic;

namespace PizzaAppMvc3
{
    public class OrdersCollection
    {
        public IEnumerable<Order> Orders { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string PaymentId { get; set; }
        public string State { get; set; }
        public string AmountInUSD { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }
}