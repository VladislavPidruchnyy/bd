using System;
using System.Collections.Generic;


namespace EntitiesManager
{
    public class Order
    {
        public long id;
        public long customerId;
        public int amount;
        public DateTime orderDate;

        public User customer;
        public List<Product> products;

        public Order()
        {
            this.id = 0;
            this.customerId = 0;
            this.amount = 0;
            this.orderDate = default;
            this.customer = null;
            this.products = null;
            products = new List<Product>();
        }

        public override string ToString()
        {
            return $"[{id}] Customer: {customerId} - {amount}$ - ({orderDate})";
        }
        public string ForWriteLineUser()
        {
            return $"[{id}] - {amount}({orderDate.ToString()})";
        }
    }
}