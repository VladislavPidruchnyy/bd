using System;
using System.Collections.Generic;

public class Product

    {
        public long id;
        public string productName;
        public double price;
        public bool isPresent;
        public DateTime createdAt;
        public List<Order> orders;


        public Product()
        {
            this.id = 0;
            this.productName = "";
            this.price = 0;
            this.isPresent = default;
            this.createdAt = default;
        }

        public Product(string productName, double price, bool availability, DateTime createdAt)
        {
            this.productName = productName;
            this.price = price;
            this.isPresent = availability;
            this.createdAt = createdAt;
        }

        public override string ToString()
        {
            return $"[{this.id}] - {this.productName}  price :{this.price}  availability: {this.isPresent}";
        }
    }