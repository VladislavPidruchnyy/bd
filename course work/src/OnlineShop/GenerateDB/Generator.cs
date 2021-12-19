using System;
using System.Collections.Generic;
using EntitiesManager;
using Microsoft.Data.Sqlite;

namespace GeneratorDB
{
    static class Generator
    {
        public static void GenerateGoods(int number, SqliteConnection connection)
        {
            string[] goodsArr = new string[] { "NoteBook", "Tablet", "Graphics card", "Monitor", "SSD", "HDD", "Keybord", "Router", "CPU", "Motherboard",
            "Projector", "Mobile Phone", "TV", "Air pods", "Flash memory card", "Iphone X", "Camera", "Apple Watch", "Smart Watch", "Play Station", "Mic" };
            string[] boolean = new string[] { "true", "false" };
            Random random = new Random();
            ProductRep productsRep = new ProductRep(connection);
            for (int i = 0; i < number; i++)
            {
                Product newGood = new Product();
                newGood.productName = goodsArr[random.Next(0, goodsArr.Length - 1)];
                newGood.price = random.Next(20, 2000);
                newGood.availability = Boolean.Parse(boolean[random.Next(0, 2)]);
                newGood.createdAt = DateTime.Now;
                productsRep.Insert(newGood);
            }
        }
        public static void GenerateUsers(int number, SqliteConnection connection)
        {
            string[] names = new string[] { "Ira", "Jon", "Mark", "Olia", "Alex", "Dima", "Ruslan", "Artem", "Vera", "Luda", "Maria", "Filip", "Maks", "Kate", "Kristofer", "Ivan", "Vlad", "Diana", "Dasha", "Darina", "Amina", "Arsen", "Lili", "Oleg", "" };
            string[] lastNames = new string[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Perez", "Walker", "Young", "Allen", "King", "Wright", "Scott", "Hill", "Flores", "Green", "Baker", "Hall" };
            Random random = new Random();
            UserRep usersRep = new UserRep(connection);
            for (int i = 0; i < number; i++)
            {
                User newUser = new User();
                newUser.fullname = names[random.Next(0, names.Length - 1)] + " " + lastNames[random.Next(0, lastNames.Length - 1)];
                newUser.username = "test" + i.ToString();
                newUser.status = "customer";
                usersRep.Insert(newUser);
            }

        }
    }
}
