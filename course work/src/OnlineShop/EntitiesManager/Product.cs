using System;
using System.Collections.Generic;

namespace EntitiesManager
{
    public class Product
    {
        public long id;
        public string productName;
        public int price;
        public bool availability;
        public DateTime createdAt;
        public List<Order> orders;


        public Product()
        {
            this.id = 0;
            this.productName = "";
            this.price = 0;
            this.availability = default;
            this.createdAt = default;
        }


        public override string ToString()
        {
            return $"[{this.id}] - {this.productName} \r\n       price :{this.price} \r\n     availability: {this.availability}";
        }

        public string WriteForOrder()
        {
            return $"[{this.id}] - {this.productName} \r\n       price :{this.price}";
        }
    }
}
