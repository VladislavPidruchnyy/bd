using System;
using static System.Console;
using Npgsql;
using System.Collections.Generic;


namespace lab2bd
{   
    class Program
    {
        static void Main(string[] args)
        {
            Commond();
        }

        public static void Commond()
        {
            NpgsqlConnection connection = new NpgsqlConnection("Server=127.0.0.1; Port=5432; User Id=postgres; Password=vladik0810; Database=postgres");
            UserRep ur = new UserRep(connection);
            ProductRep pr = new ProductRep(connection);
            OrderRep or = new OrderRep(connection);
            ManyToManyConnection services = new ManyToManyConnection(ur, pr, or);

            while (true)
            {
                string table;
                WriteLine("Enter command:");
                string input = ReadLine();
                
                if (input == "add" || input == "get" || input == "delete" || input == "update" || input == "generate" || input == "find")
                {
                    WriteLine("Enter table name: ");
                    string tableName = ReadLine();
                    if (tableName == "users" || tableName == "products" || tableName == "orders" || tableName == "products_orders") { WriteLine(tableName); }
                    else { WriteLine("This table was not found!"); }
                    table = tableName;
                    
                    if (input == "add" && table == "users")
                    {
                        WriteLine("write item information like: {userName} {name} {status}");
                        
                        User user = new User();
                        user.password = "123";
                        user.username = "User1";
                        user.status = "user";
                        user.fullname = "Boss";
                        long id = ur.Insert(user);
                        WriteLine(id);

                    }
                    
                    else if (input == "add" && table == "products")
                    {
                        WriteLine("write item information like: {userName} {name} {status}");
                        Product product = new Product();
                        product.productName = "123";
                        product.isPresent = true;
                        product.price = 100;
                        long id = pr.Insert(product);
                        WriteLine(id);

                    }
                    else if (input == "get" && table == "users")
                    {
                        List<User> users = ur.GetAllUsers();
                        foreach (User user in users)
                        {
                            WriteLine(user.ToString());
                        }
                    }

                    else if (input == "get" && table == "orders")
                    {
                        List<Order> orders = or.GetAllOrders();
                        foreach (Order order in orders)
                        {
                            WriteLine(order.ToString());
                        }
                    }
                    else if (input == "get" && table == "products")
                    {
                        List<Product> products = pr.GetAllProducts();
                        foreach (Product product in products)
                        {
                            WriteLine(product.ToString());
                        }
                    }
                    else if (input == "delete" && table == "orders")
                    {
                        WriteLine("Enter id: ");
                        int id = int.Parse(ReadLine());
                        if (id <= 0)
                        {
                            WriteLine("Id must be a positive number and > 0");
                        }
                        else
                        {
                            bool isdeleted = or.DeleteById(id);
                            if (isdeleted)
                            {
                                WriteLine("Order was deleted successfully");
                            }
                            else
                            {
                                WriteLine("Order wasn't deleted");
                            }
                        }
                    }
                    else if (input == "delete" && table == "users")
                    {
                        WriteLine("Enter id: ");
                        int id = int.Parse(ReadLine());
                        if (id <= 0)
                        {
                            WriteLine("Id must be a positive number and > 0");
                        }
                        else
                        {
                            bool isdeleted = ur.DeleteById(id);
                            if (isdeleted)
                            {
                                WriteLine("User was deleted successfully");
                            }
                            else
                            {
                                WriteLine("User wasn't deleted");
                            }
                        }
                    }
                    else if (input == "delete" && table == "products")
                    {
                        WriteLine("Enter id: ");
                        int id = int.Parse(ReadLine());
                        if (id <= 0)
                        {
                            WriteLine("Id must be a positive number and > 0");
                        }
                        else
                        {
                            bool isdeleted = pr.DeleteById(id);
                            if (isdeleted)
                            {
                                WriteLine("Product was deleted successfully");
                            }
                            else
                            {
                                WriteLine("Product wasn't deleted");
                            }
                        }

                    }
                    else if (input == "delete" && table == "orders")
                    {
                        WriteLine("Enter id: ");
                        int id = int.Parse(ReadLine());
                        if (id <= 0)
                        {
                            WriteLine("Id must be a positive number and > 0");
                        }
                        else
                        {
                            bool isdeleted = or.DeleteById(id);
                            if (isdeleted)
                            {
                                WriteLine("Order was deleted successfully");
                            }
                            else
                            {
                                WriteLine("Order wasn't deleted");
                            }
                        }

                    }
                    else if(input == "find" && table == "products")
                    {
                        throw new NotImplementedException();
                    }
                    else if (input == "update" && table == "products")
                    {
                        Product product = new Product();
                        product.productName = "321";
                        product.isPresent = false;
                        product.price = 200;
                        WriteLine("Enter id: ");
                        int id = int.Parse(ReadLine());
                        if (id <= 0)
                        {
                            WriteLine("Id must be a positive number and > 0");
                        }
                        else
                        {
                            pr.Update(id, product);
                        }
                    }
                    else if (input == "update" && table == "users")
                    {
                        User user = new User();
                        user.password = "321";
                        user.username = "User2";
                        user.status = "admin";
                        user.fullname = "MrBoss";
                        WriteLine("Enter id: ");
                        int id = int.Parse(ReadLine());
                        if (id <= 0)
                        {
                            WriteLine("Id must be a positive number and > 0");
                        }
                        else
                        {
                            ur.Update(id, user);
                        }
                    }


                    else if (input == "generate")
                    {
                        GenerateData generateData = new GenerateData();
                        WriteLine("Enter the number of values need to be generated");
                        string stringNumber = ReadLine();
                        int numGoods;
                        if (!Int32.TryParse(stringNumber, out numGoods)) { WriteLine($"Incorect items number input(must be int): {input}"); return; }
                        if (table == "products")
                        {
                            generateData.GenerateGoods(numGoods, connection);
                            WriteLine("Created");
                        }
                        int numUsers;
                        if (!Int32.TryParse(stringNumber, out numUsers)) { WriteLine($"Incorect items number input(must be int): {input}"); return; }
                        if (table == "users")
                        {
                            generateData.GenerateUsers(numUsers, connection);
                            WriteLine("Created");
                        }
                        int numOrders;

                        if (!Int32.TryParse(stringNumber, out numOrders)) { WriteLine($"Incorect items number input(must be int): {input}"); return; }
                        if (table == "orders")
                        {
                            generateData.GenerateOrders(numOrders, connection);
                            WriteLine("Created");
                        }

                    }

                    else
                    {
                        Error.WriteLine($"Incorrect input: {input}");
                    }
                }
                else if(input == "Exit")
                {
                    break;
                }
                else
                {
                    WriteLine("Incorect input, try again!");
                }
            }
        }
    }
}
