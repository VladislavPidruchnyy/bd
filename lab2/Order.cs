using System;
using System.Collections.Generic;

    public class Order
    {
        public long id;
        public long customerId;
        public double amount;
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

        public Order(long customerId, double amount, DateTime orderDate, List<Product> products)
        {
            this.customerId = customerId;
            this.amount = amount;
            this.orderDate = orderDate;

        }

        public override string ToString()
        {
            return $"[{id}] {customerId}  ({orderDate.ToString()}) {amount}$";
        }
    }