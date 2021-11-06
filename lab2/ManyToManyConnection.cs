using static System.Console;
using System.Collections.Generic;
using Npgsql;
using System;





public class ManyToManyConnection
    {
        private UserRep userRep;
        private ProductRep productRep;
        private OrderRep orderRep;

        public ManyToManyConnection(UserRep userRep, ProductRep productRep, OrderRep orderRep)
        {
            this.userRep = userRep;
            this.productRep = productRep;
            this.orderRep = orderRep;
        }

        public void AddOrder(long userId, string orderInf)
        {
            User user = new User();
            user = userRep.GetUserById(userId);
            if (user == null)
            {
                WriteLine(@"User with id : {userId} dosn`t exist.");
                return;
            }


            // string orderInf = "1 3 4";
            string[] orderArr = orderInf.Split(' ');
            if (orderArr.Length < 1)
            {
                WriteLine(@"Incorrect input : {orderInf}.");
                return;
            }

            for (int i = 0; i < orderArr.Length; i++)
            {
                int parsed;
                if (!Int32.TryParse(orderArr[i], out parsed)) { WriteLine($"Incorrect input (productID should be a number): '{orderInf}'"); return; }
                Product orderProduct = productRep.GetProductById(long.Parse(orderArr[i]));
                if (orderProduct == null) { WriteLine($"Product with id {orderArr[i]} doesn`t exist. Order Not created."); return; }
                if (orderProduct.isPresent == false) { WriteLine($"Product with id '{orderArr[i]}' product out of stock. Please choose another products."); break; }
            }
            Order newOrder = new Order();
            newOrder.customerId = user.id;
            newOrder.orderDate = DateTime.Now;
            newOrder.amount = 0;
            for (int i = 0; i < orderArr.Length; i++)
            {
                Product orderProduct = productRep.GetProductById(long.Parse(orderArr[i]));
                newOrder.amount += orderProduct.price;
                if (i == 0)
                {
                    newOrder.id = orderRep.Insert(newOrder);
                    if (newOrder.id == 0)
                    {
                        WriteLine(@"Order doesn`t created.");
                        return; ;
                    }

                }
                BuildOrderProductLink(orderProduct.id, newOrder.id, productRep, orderRep);
            }

            if (orderRep.Update(newOrder.id, newOrder))
            {
                WriteLine(@"Order created.");
            }
            return;
        }

        private void BuildOrderProductLink(long productId, long orderId, ProductRep productRep, OrderRep orderRep)
        {

            Product productForAdding = productRep.GetProductById(productId);
            if (productForAdding == null)
            {
                WriteLine($"Product id '{productId}' doesn`t exist. Choose another product. ");
                return;
            }
            if (productForAdding.isPresent == false)
            {
                WriteLine($"Product with id '{productId}' product out of stock. Please choose another product.");
                return;
            }

            if (!orderRep.AddProductConection(productForAdding.id, orderId))
            {
                WriteLine("Connection NOT created!!!");
                return;
            }
            WriteLine("Connection created.");


        }


    }