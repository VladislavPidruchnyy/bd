using Npgsql;
using System;


    public class GenerateData
    {
        public void GenerateGoods(int number, NpgsqlConnection connection)
        {
            string[] goodsArr = new string[] { "laptop", "Iphone", "PC", "XBOX360", "Playstation4", "PS5", "XBOXOne", "Samsung S10+" };
            string[] boolean = new string[] { "true", "false" };
            Random random = new Random();
            ProductRep productsRep = new ProductRep(connection);
            for (int i = 0; i < number; i++)
            {
                Product newGood = new Product();
                newGood.productName = goodsArr[random.Next(0, goodsArr.Length - 1)];
                newGood.price = random.Next(20, 2000);
                newGood.isPresent = Boolean.Parse(boolean[random.Next(0, 2)]);
                newGood.createdAt = DateTime.Now;
                productsRep.Insert(newGood);
            }
        }

        public void GenerateUsers(int number, NpgsqlConnection connection)
        {
            string[] names = new string[] { "Ira", "Jon", "Mark", "Olia", "Alex", "Dima", "Ruslan", "Artem", "Vera", "Luda", "Maria", "Filip", "Maks", "Kate", "Kristofer", "Ivan", "Vlad", "Diana", "Dasha", "Darina", "Amina", "Arsen", "Lili", "Oleg", "" };
            string[] lastNames = new string[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Walker", "Young", "Allen", "King", "Wright", "Scott", "Hill", "Flores", "Green", "Baker", "Hall" };
            Random random = new Random();
            UserRep usersRep = new UserRep(connection);
            for (int i = 0; i < number; i++)
            {
                User newUser = new User();
                newUser.fullname = names[random.Next(0, names.Length - 1)] + " " + lastNames[random.Next(0, lastNames.Length - 1)];
                newUser.username = "customer" + i.ToString();
                newUser.status = "customer";

                newUser.password = "12345";
                newUser.createdAt = DateTime.Now;
                usersRep.Insert(newUser);
            }

        }
        public void GenerateOrders(int number, NpgsqlConnection connection)
        {

            Random random = new Random();
            UserRep usersRep = new UserRep(connection);
            OrderRep orderRep = new OrderRep(connection);
            ProductRep productRep = new ProductRep(connection);
            ManyToManyConnection TableManyTomany = new ManyToManyConnection(usersRep, productRep, orderRep);
            for (int i = 0; i < number; i++)
            {
                long randomUserId = random.Next(1, (int)usersRep.GetCount());
                TableManyTomany.AddOrder(randomUserId, $"{random.Next(1, (int)productRep.GetCount())} {random.Next(1, (int)productRep.GetCount())}");
            }
        }
    }


