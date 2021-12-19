using System;
using static System.Console;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using EntitiesManager;

namespace GeneratorDB
{
    class Program
    {
        static void Main(string[] args)
        {
            Commond();
            // add item
            // add user
            // add order
            // get order inf
            // get user orders
            // generate goods +-
            // generate users 

            // string db_path = @"./data.db";

            // SqliteConnection connection = new SqliteConnection($"Data Source={db_path}");

            // ProductRep products = new ProductRep(connection);
            // UserRep users = new UserRep(connection);
            // OrderRep orders = new OrderRep(connection);

            // Application.Init();
            // LogIn log = new LogIn()
            // {
            //    X = 0,
            //    Y = 1,
            //    Width = Dim.Fill(),
            //    Height = Dim.Fill() - 1
            // };
            // Application.Top.Add(log);
            // log.SetRepository(users, orders, products);
            // log.SetConnection(connection);
            // Application.Run();


            //
            //ProcessGUI processgui = new ProcessGUI();
            //processgui.LogIn();
        }

        public static void Commond()
        {
            string databaseFileName = @"C:\All\KPI\bd\course work\data\data.db";
            SqliteConnection connection = new SqliteConnection($"Data Source={databaseFileName}");
            UserRep ur = new UserRep(connection);
            ProductRep gr = new ProductRep(connection);
            OrderRep or = new OrderRep(connection);
            Services services = new Services(ur, gr, or);
            //WriteLine("Commands: \r\nadd \r\ngenerate");
            while (true)
            {
                WriteLine("Enter command:");
                string input = ReadLine();
                //string input = "add user";
                //string input = "get user orders";
                //WriteLine(input);
                string[] inputArr = input.Split(" ");

                if (input == "add item")
                {
                    WriteLine("Write item information like: {name} {price} {description} {availability: true/false}");
                    Product pr = new Product();
                    //WriteLine("Enter id of product:");
                    //pr.id = int.Parse(ReadLine());
                    WriteLine("Enter name of product:");
                    pr.productName = "new";
                    WriteLine("Enter price of product:");
                    pr.price = 404;
                    WriteLine("Enter availability of product:");
                    pr.availability = true;
                    pr.createdAt = DateTime.Now;

                    gr.Insert(pr);
                    return;
                }
                else if (input == "add order")
                {
                    WriteLine($"Write userId: ");
                    long userId = 0;
                    try
                    {
                        userId = long.Parse(ReadLine());
                    }
                    catch
                    {
                        WriteLine("Id must be numeric.");
                        break;
                    }
                    services.AddOrder(userId);
                    return;

                }
                else if (input == "add user")
                {

                    User user = new User();
                    bool state1 = true;
                    while (state1)
                    {
                        WriteLine("Enter username:");
                        user.username = ReadLine();
                        if (ur.CheckUserName(user.username) == 0)
                        {
                            WriteLine("Enter fullname:");
                            user.fullname = ReadLine();
                            bool state = true;
                            while (state)
                            {
                                WriteLine("Enter status: {admin / customer}");
                                user.status = ReadLine();
                                if (user.status == "admin" || user.status == "customer")
                                {
                                    ur.Insert(user);
                                    WriteLine("User added!");
                                    state = false;
                                }
                                else
                                {
                                    WriteLine("Incorect status, try again!");
                                }
                            }
                            state1 = false;
                        }
                        else
                        {
                            WriteLine("A user with this name already exists, please try again!");
                        }
                    }

                    return;

                }
                else if (input == "get order inf")
                {
                    WriteLine("Write order number");
                    long orderNum = 0;
                    try
                    {
                        orderNum = long.Parse(ReadLine());
                    }
                    catch
                    {
                        WriteLine($"Order num must be numeric.");
                        break;
                    }


                    Order order = or.GetOrderById(orderNum);
                    if (order == null)
                    {
                        WriteLine($"Order with order number '{orderNum}' doesn`t exist.");
                        break;
                    }
                    WriteLine($"Order: {orderNum}");
                    User customer = ur.GetUserById(order.customerId);
                    WriteLine($"CustomerId: {customer.id}");
                    List<long> productIds = or.GetAllOrderProductsId(orderNum);
                    List<Product> prodList = gr.GetOrderProducts(productIds);
                    foreach (Product product in prodList)
                    {
                        WriteLine(product.WriteForOrder());
                    }
                    return;
                }
                else if (input == "get user orders")
                {
                    long userId = 0;
                    try
                    {
                        userId = long.Parse(ReadLine());
                    }
                    catch
                    {
                        WriteLine($"Order num must be numeric.");
                        break;
                    }
                    User user = ur.GetUserById(userId);
                    if (user == null)
                    {
                        WriteLine($"User with userId '{userId}' doesn`t exist.");
                        break;
                    }
                    WriteLine($"User: {userId}");
                    List<long> ordersId = ur.GetAllUserOrdersId(userId);
                    List<Order> ordersList = or.GetAllUserOrdersById(ordersId);
                    if (ordersList.Count == 0)
                    {
                        WriteLine($"User with id '{userId}' has no orders.");
                    }
                    foreach (Order order in ordersList)
                    {
                        WriteLine(order.ToString());
                    }
                    return;
                }
                else if (input == "deleteuser")
                {
                    ur.DeleteById(18);
                }

                else if (inputArr[0] == "generate")
                {
                    if (inputArr[1] == "products")
                    {
                        int numGoods = 0;
                        if (!Int32.TryParse(inputArr[2], out numGoods)) 
                        { 
                            WriteLine($"Incorect items number input(must be int): {input}"); return; 
                        }

                        Generator.GenerateGoods(numGoods, connection);
                        WriteLine("Generated");
                    }
                    else if (inputArr[1] == "users")
                    {
                        int numUsers = 0;
                        if (!Int32.TryParse(inputArr[2], out numUsers))
                        {
                            WriteLine($"Incorect items number input(must be int): {input}"); return; 
                        }

                        Generator.GenerateUsers(numUsers, connection);
                        WriteLine("Generated");
                    }
                    else
                    {
                        Error.WriteLine($"Incorrect input");
                    }
                }
                else if (input == "exit")
                {
                    break;
                }

            }
        }
    }
}
