using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Terminal.Gui;
using System.IO;
using EntitiesManager;
using System;

//#.. "name.h"
//#.. "name.c"
//#.. <stdlib.h>
namespace GUIApp
{
    public class ProcessGUI
    {
        public TextField usernameInput;
        public TextField fullnameInput;
        public TextField passwordInput;
        public TextField conpasswordInput;

        private SqliteConnection connection;
        private OrderRep OrderRep;
        private ProductRep ProductRep;
        private UserRep UserRep;

        public void LogIn()
        {
            Application.Init();

            string db_path = @"C:\All\KPI\bd\course work\data\data.db";
            SqliteConnection connection = new SqliteConnection();
            if (File.Exists(db_path))
                connection = new SqliteConnection($"Data Source={db_path}");
            else
            {
                MessageBox.ErrorQuery("DataBase", "The database on this path does not exist", "OK");
                Environment.Exit(0);
            }

            ProductRep products = new ProductRep(connection);
            UserRep users = new UserRep(connection);
            OrderRep orders = new OrderRep(connection);


            Toplevel top = Application.Top;
            Window win = new Window("Log in")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            top.Add(win);
            SetRepository(users, orders, products);

            Label firstLbl = new Label(53, 4, "Login To Your Account");

            Label UserNameLbl = new Label(45, 8, "Username: ");
            usernameInput = new TextField("")
            {
                X = 45,
                Y = 9,
                Width = 40,
            };

            Label passwordLbl = new Label(45, 12, "Password: ");
            passwordInput = new TextField("")
            {
                X = 45,
                Y = 13,
                Width = 40,
            };
            passwordInput.Secret = true;

            Button logInbtn = new Button(60, 15, "LogIn");
            logInbtn.Clicked += CheckLogin;

            Button signUpbtn = new Button(60, 17, "SignUp");
            signUpbtn.Clicked += ClickSignUp;

            Button Exitbtn = new Button(61, 19, "Exit");
            Exitbtn.Clicked += OnExitClicked;

            top.Add(UserNameLbl, usernameInput, passwordLbl, passwordInput, firstLbl, signUpbtn, logInbtn, Exitbtn);
            Application.Run();
        }

        public void ClickSignUp()
        {
            Application.Init();
            Toplevel top = Application.Top;
            //Rect frame = new Rect(45, 4, 60, 30);
            Window win = new Window("Sign up")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            top.Add(win);

            Label firstLbl = new Label(51, 4, "Registration");
            Label usernameLbl = new Label(45, 8, "Username: ");
            usernameInput = new TextField("")
            {
                X = 45,
                Y = 9,
                Width = 40,
            };
            Label fullnameLbl = new Label(45, 11, "FullName: ");
            fullnameInput = new TextField("")
            {
                X = 45,
                Y = 12,
                Width = 40,
            };
            Label passwordLbl = new Label(45, 14, "Password: ");
            passwordInput = new TextField("")
            {
                X = 45,
                Y = 15,
                Width = 40,
            };
            passwordInput.Secret = true;
            Label conpassLbl = new Label(45, 17, "Confirm Password:");
            conpasswordInput = new TextField("")
            {
                X = 45,
                Y = 18,
                Width = 40,
            };
            conpasswordInput.Secret = true;

            Button okBtn = new Button(61, 20, "Ok");
            okBtn.Clicked += OnSignUpClicked;
            Button BackBtn = new Button(60, 22, "Back");
            BackBtn.Clicked += LogIn;

            top.Add(usernameLbl, usernameInput, passwordLbl, passwordInput, firstLbl, fullnameLbl,
            fullnameInput, okBtn, conpassLbl, conpasswordInput, BackBtn);
            Application.Run();
        }
        public void OnSignUpClicked()
        {
            User user = new User();
            if (usernameInput.Text.ToString() != "" || fullnameInput.Text.ToString() != "")
            {
                if (conpasswordInput.Text == passwordInput.Text)
                {
                    if (UserRep.CheckUserName(usernameInput.Text.ToString()) == 0)
                    {
                        user.username = usernameInput.Text.ToString();
                        user.fullname = fullnameInput.Text.ToString();
                        user.pass = HashPassword.HashCode(passwordInput.Text.ToString());
                        user.status = "customer";
                        UserRep.Insert(user);
                        CheckLogin();
                    }
                    else
                    {
                        MessageBox.Query("SignUp", "Oops! This name is already in the base. Choose another", "OK");
                    }
                }
                else
                {
                    MessageBox.ErrorQuery("Error", "Passwords aren`t the same", "OK");
                }
            }
            else
            {
                MessageBox.Query("SignUp", "You have not entered anything, please try again!", "OK");
            }
        }

        User me;
        public void CheckLogin()
        {
            User user = new User();
            bool isHere = UserRep.CheckUserInDb(usernameInput.Text.ToString(), passwordInput.Text.ToString());
            if (isHere == true)
            {
                user = UserRep.GetUserById(UserRep.CheckUserName(usernameInput.Text.ToString()));
                me = user;
                if (user.status == "admin")
                {
                    AdminPanel(connection);
                }
                else
                {
                    ClientPanel(connection);
                }
            }
            else
            {
                MessageBox.Query("Log in", "Oops! We can not find you. Check your password and name, please.", "OK");
            }
        }

        public void AdminPanel(SqliteConnection connection)
        {
            Application.Init();
            Toplevel top = Application.Top;
            MenuBar menu = new MenuBar(new MenuBarItem[]
            {
            new MenuBarItem ("Menu", new MenuItem []
            {
                new MenuItem ("Exit", "", OnExitClicked),
                new MenuItem("Log out", "", LogOut),
            }),

            new MenuBarItem ("Options", new MenuItem []
            {
                new MenuItem("Import", "", OnImportClicked),
                new MenuItem("Export", "", OnExportClicked),
                new MenuItem("Generate graph", "", NotImplementedClick),
                new MenuItem("Generate document", "", NotImplementedClick),
            }),

            new MenuBarItem ("Help", new MenuItem []
            {
                new MenuItem("About the programm", "", HelpClick),
            }),
            });
            top.Add(menu);

            Window win = new Window("AdminPanel")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            top.Add(win);

            FrameView list = new FrameView("List")
            {
                X = 5,
                Y = 4,
                Width = 35,
                Height = 9,
            };
            top.Add(list);

            Button ListProductsBtn = new Button(12, 6, "List of products");
            ListProductsBtn.Clicked += OnListproductsClicked;
            Button ListUsersBtn = new Button(12, 8, "List of users");
            ListUsersBtn.Clicked += OnListusersClicked;
            Button ListOrdersBtn = new Button(12, 10, "List of orders");
            ListOrdersBtn.Clicked += OnListordersClicked;

            FrameView account = new FrameView("Account")
            {
                X = 55,
                Y = 4,
                Width = 35,
                Height = 9,
            };
            top.Add(account);

            Button AccBtn = new Button(61, 6, "Info about account");
            AccBtn.Clicked += OnInfoAdminClicked;

            FrameView add = new FrameView("Add")
            {
                X = 5,
                Y = 15,
                Width = 35,
                Height = 7,
            };
            top.Add(add);

            Button AddProductBtn = new Button(12, 17, "Add product");
            AddProductBtn.Clicked += OnAddproductClicked;
            Button AddOrderBtn = new Button(12, 19, "Add order");
            AddOrderBtn.Clicked += OnAddorderClicked;

            FrameView search = new FrameView("Search")
            {
                X = 5,
                Y = 22,
                Width = 35,
                Height = 4,
            };
            top.Add(search);

            Button SearchProductBtn = new Button(12, 23, "Search product");
            SearchProductBtn.Clicked += OnSearchProductClicked;


            top.Add(ListProductsBtn, ListUsersBtn, ListOrdersBtn, AccBtn, AddProductBtn, AddOrderBtn, SearchProductBtn );
            Application.Run();
        }
        public void ClientPanel(SqliteConnection connection)
        {
            Application.Init();
            Toplevel top = Application.Top;
            MenuBar menu = new MenuBar(new MenuBarItem[]
            {
            new MenuBarItem ("Menu", new MenuItem []
            {
                new MenuItem ("Exit", "", OnExitClicked),
                new MenuItem("Log out", "", LogOut),
            }),

            new MenuBarItem ("Help", new MenuItem []
            {
                new MenuItem("About the programm", "", HelpClick),
            }),
            });
            top.Add(menu);

            Window win = new Window("ClientPanel")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            top.Add(win);

            FrameView actions = new FrameView("Actions")
            {
                X = 30,
                Y = 6,
                Width = 35,
                Height = 12,
            };
            top.Add(actions);

            Button ListProductsBtn = new Button(35, 8, "List of products");
            ListProductsBtn.Clicked += OnListproductsClicked;
            Button AddOrderBtn = new Button(35, 10, "Add order");
            AddOrderBtn.Clicked += OnAddorderClicked;
            Button ListOrdersBtn = new Button(35, 12, "List of orders");
            ListOrdersBtn.Clicked += OnListordersClientClicked;
            Button AccBtn = new Button(35, 14, "Info about account");
            AccBtn.Clicked += OnInfoUserClicked;

            FrameView search = new FrameView("Search")
            {
                X = 30,
                Y = 20,
                Width = 35,
                Height = 4,
            };
            top.Add(search);

            Button SearchProductBtn = new Button(35, 22, "Search product");
            SearchProductBtn.Clicked += OnSearchProductClicked;


            top.Add(ListProductsBtn, ListOrdersBtn, AccBtn, AddOrderBtn, SearchProductBtn);
            Application.Run();
        }

        private void OnExportClicked()
        {
            ExportDialog dialog = new ExportDialog();
            dialog.SetRepository(UserRep, OrderRep, ProductRep);
            Application.Run(dialog);
        }
        private void OnImportClicked()
        {
            ImportDialog dialog = new ImportDialog();
            dialog.SetRepository(UserRep, OrderRep, ProductRep);
            Application.Run(dialog);
        }

        private void LogOut()
        {
            me = new User();
            LogIn();
        }

        ListView products = new ListView();
        ListView orders = new ListView();
        ListView users = new ListView();
        int page;

        Label labelName;

        List<Product> productsList;
        private void OnListproductsClicked()
        {
            page = 1;
            productsList = ProductRep.GetAllGoods();
            Application.Init();
            Toplevel top = Application.Top;
            MenuBar menu = new MenuBar(new MenuBarItem[]
            {
            new MenuBarItem ("Actions",new MenuItem[]
            {
                  new MenuItem("Back","",OnBackClicked),
                  new MenuItem("Delete","",DeleteProduct),
                  new MenuItem("Edit","",EditProduct)
            }),
            });
            top.Add(menu);
            Window win = new Window("Products")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            top.Add(win);
            products = new ListView(new Rect(2, 2, 100, 10), productsList.GetRange((page - 1) * 10, productsList.Count - 10 * page >= 0 ? 10 : productsList.Count - (page - 1) * 10));
            labelName = new Label(10, 12, $"Page { page,4 }/{productsList.Count / 10 + (productsList.Count % 10 == 0 ? 0 : 1),4}");
            Button btnBackWard = new Button(2, 12, "Back");
            btnBackWard.Clicked += OnBackPageProductClicked;
            Button btnForward = new Button(25, 12, "Next");
            btnForward.Clicked += OnNextProductClicked;
            top.Add(products, labelName, btnBackWard, btnForward);
            Application.Run();
        }

        List<Order> ordersList;
        private void OnListordersClientClicked()
        {
            page = 1;
            ordersList = OrderRep.GetAllById(me.id);
            Application.Init();
            Toplevel top = Application.Top;
            MenuBar menu = new MenuBar(new MenuBarItem[]
            {
            new MenuBarItem ("Actions",new MenuItem[]
            {
                  new MenuItem("Back","",OnBackClicked),
                  new MenuItem("Delete","",DeleteOrder),
                  new MenuItem("Edit","",EditUserOrder)
            }),
            });
            top.Add(menu);
            Window win = new Window("Orders")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            top.Add(win);
            orders = new ListView(new Rect(2, 2, 100, 10), ordersList.GetRange((page - 1) * 10, ordersList.Count - 10 * page >= 0 ? 10 : ordersList.Count - (page - 1) * 10));
            labelName = new Label(10, 12, $"Page { page,4 }/{ordersList.Count / 10 + (ordersList.Count % 10 == 0 ? 0 : 1),4}");
            Button btnBackWard = new Button(2, 12, "Back");
            btnBackWard.Clicked += OnBackPageOrderClicked;
            Button btnForward = new Button(25, 12, "Next");
            btnForward.Clicked += OnNextOrderClicked;
            top.Add(orders, labelName, btnBackWard, btnForward);
            Application.Run();
        }
        private void OnListordersClicked()
        {
            page = 1;
            ordersList = OrderRep.GetAllOrders();
            Application.Init();
            Toplevel top = Application.Top;
            MenuBar menu = new MenuBar(new MenuBarItem[]
            {
            new MenuBarItem ("Actions",new MenuItem[]
            {
                  new MenuItem("Back","",OnBackClicked),
                  new MenuItem("Delete","",DeleteOrder),
                  new MenuItem("Edit","",EditOrder)
            }),
            });
            top.Add(menu);
            Window win = new Window("Orders")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            top.Add(win);
            orders = new ListView(new Rect(2, 2, 100, 10), ordersList.GetRange((page - 1) * 10, ordersList.Count - 10 * page >= 0 ? 10 : ordersList.Count - (page - 1) * 10));
            labelName = new Label(10, 12, $"Page { page,4 }/{ordersList.Count / 10 + (ordersList.Count % 10 == 0 ? 0 : 1),4}");
            Button btnBackWard = new Button(2, 12, "Back");
            btnBackWard.Clicked += OnBackPageOrderClicked;
            Button btnForward = new Button(25, 12, "Next");
            btnForward.Clicked += OnNextOrderClicked;
            top.Add(orders, labelName, btnBackWard, btnForward);
            Application.Run();
        }

        List<User> usersList;
        private void OnListusersClicked()
        {
            page = 1;
            usersList = UserRep.GetAllUsers();
            Application.Init();
            Toplevel top = Application.Top;
            MenuBar menu = new MenuBar(new MenuBarItem[]
            {
            new MenuBarItem ("Actions",new MenuItem[]
            {
                  new MenuItem("Back","",OnBackClicked),
                  new MenuItem("Delete","",DeleteUser)
            }),
            });
            top.Add(menu);
            Window win = new Window("Users")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            top.Add(win);
            users = new ListView(new Rect(2, 2, 100, 10), usersList.GetRange((page - 1) * 10, usersList.Count - 10 * page >= 0 ? 10 : usersList.Count - (page - 1) * 10));
            labelName = new Label(10, 12, $"Page { page,4 }/{usersList.Count / 10 + (usersList.Count % 10 == 0 ? 0 : 1),4}");
            Button btnBackWard = new Button(2, 12, "Back");
            btnBackWard.Clicked += OnBackPageUserClicked;
            Button btnForward = new Button(25, 12, "Next");
            btnForward.Clicked += OnNextUserClicked;
            top.Add(users, labelName, btnBackWard, btnForward);
            Application.Run();
        }

        private void OnSearchProductClicked()
        {
            SearchProduct dialog = new SearchProduct();
            dialog.SetRepository(UserRep,OrderRep,ProductRep);
            Application.Run(dialog);
        }
        private void OnAddproductClicked()
        {
            AddProduct dialog = new AddProduct();
            dialog.SetRepository(UserRep, OrderRep, ProductRep);
            Application.Run(dialog);
        }
        private void OnAddorderClicked()
        {
            AddOrder dialog = new AddOrder(me.id);
            dialog.SetRepository(UserRep, OrderRep, ProductRep);
            Application.Run(dialog);
        }

        private void OnInfoUserClicked()
        {
            Application.Init();
            Toplevel top = Application.Top;

            SetRepository(UserRep, OrderRep, ProductRep);
            FrameView account = new FrameView("Account")
            {
                X = 70,
                Y = 6,
                Width = 35,
                Height = 12,
            };
            top.Add(account);

            Label fullnameLbl = new Label(75, 8, "FullName: " + me.fullname);
            Label loginLabel = new Label(75, 10, "Username: " + me.username);
            Label idLabel = new Label(75, 12, "ID: " + me.id);
            Label statusLabel = new Label(75, 14, "Status: " + me.status);
            Button hideBtn = new Button(85, 15, "Hide");
            top.Add(fullnameLbl, loginLabel, idLabel, statusLabel, hideBtn);
            hideBtn.Clicked += Rem;
            void Rem()
            {
                top.Remove(account);
                top.Remove(fullnameLbl);
                top.Remove(loginLabel);
                top.Remove(idLabel);
                top.Remove(statusLabel);
                top.Remove(hideBtn);
            }
        }
        private void OnInfoAdminClicked()
        {
            Application.Init();
            Toplevel top = Application.Top;

            SetRepository(UserRep, OrderRep, ProductRep);
            FrameView account = new FrameView("Account")
            {
                X = 55,
                Y = 15,
                Width = 35,
                Height = 12,
            };
            top.Add(account);

            Label fullnameLbl = new Label(60, 17, "FullName: " + me.fullname);
            Label loginLabel = new Label(60, 19, "Username: " + me.username);
            Label idLabel = new Label(60, 21, "ID: " + me.id);
            Label statusLabel = new Label(60, 23, "Status: " + me.status);
            Button hideBtn = new Button(70, 25, "Hide");
            top.Add(fullnameLbl, loginLabel, idLabel, statusLabel, hideBtn);
            hideBtn.Clicked += Rem;
            void Rem()
            {
                top.Remove(account);
                top.Remove(fullnameLbl);
                top.Remove(loginLabel);
                top.Remove(idLabel);
                top.Remove(statusLabel);
                top.Remove(hideBtn);
            }
        }

        private void OnBackClicked()
        {
            if (me.status == "admin")
                AdminPanel(connection);
            else
                ClientPanel(connection);
        }
        private void DeleteProduct()
        {
            int res = MessageBox.Query("Delete", "Delete this object?", "Yes", "No");
            if (me.status == "admin" & res == 0)
            {
                ProductRep.DeleteById(ProductRep.GetAllGoods()[products.SelectedItem + (page - 1) * 10].id);
                products.SetSource(ProductRep.GetAllGoods());
                OnListproductsClicked();
            }
        }

        private void EditProduct()
        {
            EditProduct dialog = new EditProduct(ProductRep, ProductRep.GetAllGoods()[products.SelectedItem + (page - 1) * 10].id);
            dialog.SetRepository(UserRep, OrderRep, ProductRep);
            Application.Run(dialog);
            OnListproductsClicked();
        }
        private void EditOrder()
        {
            EditOrder dialog = new EditOrder(OrderRep, ProductRep, OrderRep.GetAllOrders()[orders.SelectedItem + (page - 1) * 10].id);
            dialog.SetRepository(UserRep, OrderRep, ProductRep);
            Application.Run(dialog);
            OnListordersClicked();
        }
        private void EditUserOrder()
        {
            EditOrder dialog = new EditOrder(OrderRep, ProductRep, OrderRep.GetAllById(me.id)[orders.SelectedItem + (page - 1) * 10].id);
            dialog.SetRepository(UserRep, OrderRep, ProductRep);
            Application.Run(dialog);
            OnListordersClientClicked();
        }

        private void DeleteOrder()
        {
            int res = MessageBox.Query("Delete", "Delete this object?", "Yes", "No");
            if (res == 0)
                if (me.status != "admin")
                {
                    OrderRep.DeleteProductConection(OrderRep.GetAllById(me.id)[orders.SelectedItem + (page - 1) * 10].id);
                    OrderRep.DeleteById(OrderRep.GetAllById(me.id)[orders.SelectedItem + (page - 1) * 10].id);
                    orders.SetSource(OrderRep.GetAllById(me.id));
                    OnListordersClientClicked();
                }
                else
                {
                    OrderRep.DeleteProductConection(OrderRep.GetAllOrders()[orders.SelectedItem + (page - 1) * 10].id);
                    OrderRep.DeleteById(OrderRep.GetAllOrders()[orders.SelectedItem + (page - 1) * 10].id);
                    orders.SetSource(OrderRep.GetAllOrders());
                    OnListordersClicked();
                }
        }
        private void DeleteUser()
        {
            int res = MessageBox.Query("Delete", "Delete this object?", "Yes", "No");
            if (res == 0)
            {
                UserRep.DeleteById(usersList[users.SelectedItem + (page - 1) * 10].id);
                users.SetSource(UserRep.GetAllUsers());
                if (me.status != "admin")
                    OnListusersClicked();
                else
                    OnListusersClicked();
            }
        }

        private void OnBackPageProductClicked()
        {
            if (page > 1)
            {
                page--;
                products.SetSource(productsList.GetRange((page - 1) * 10, productsList.Count - 10 * page >= 0 ? 10 : productsList.Count - (page - 1) * 10));
                labelName.Text = $"Page { page,4 }/{productsList.Count / 10 + (productsList.Count % 10 == 0 ? 0 : 1),4}";
            }
        }
        private void OnNextProductClicked()
        {

            if (page < productsList.Count / 10 + (productsList.Count % 10 == 0 ? 0 : 1))
            {
                page++;
                products.SetSource(productsList.GetRange((page - 1) * 10, (productsList.Count - 10 * page) >= 0 ? 10 : productsList.Count - (page - 1) * 10));
                labelName.Text = $"Page { page,4 }/{productsList.Count / 10 + (productsList.Count % 10 == 0 ? 0 : 1),4}";
            }
        }

        private void OnBackPageOrderClicked()
        {
            if (page > 1)
            {
                page--;
                orders.SetSource(ordersList.GetRange((page - 1) * 10, ordersList.Count - 10 * page >= 0 ? 10 : ordersList.Count - (page - 1) * 10));
                labelName.Text = $"Page { page,4 }/{ordersList.Count / 10 + (ordersList.Count % 10 == 0 ? 0 : 1),4}";
            }
        }
        private void OnNextOrderClicked()
        {

            if (page < ordersList.Count / 10 + (ordersList.Count % 10 == 0 ? 0 : 1))
            {
                page++;
                orders.SetSource(ordersList.GetRange((page - 1) * 10, (ordersList.Count - 10 * page) >= 0 ? 10 : ordersList.Count - (page - 1) * 10));
                labelName.Text = $"Page { page,4 }/{ordersList.Count / 10 + (ordersList.Count % 10 == 0 ? 0 : 1),4}";
            }
        }

        private void OnBackPageUserClicked()
        {
            if (page > 1)
            {
                page--;
                users.SetSource(usersList.GetRange((page - 1) * 10, usersList.Count - 10 * page >= 0 ? 10 : usersList.Count - (page - 1) * 10));
                labelName.Text = $"Page { page,4 }/{usersList.Count / 10 + (usersList.Count % 10 == 0 ? 0 : 1),4}";
            }
        }
        private void OnNextUserClicked()
        {

            if (page < usersList.Count / 10 + (usersList.Count % 10 == 0 ? 0 : 1))
            {
                page++;
                users.SetSource(usersList.GetRange((page - 1) * 10, (usersList.Count - 10 * page) >= 0 ? 10 : usersList.Count - (page - 1) * 10));
                labelName.Text = $"Page { page,4 }/{usersList.Count / 10 + (usersList.Count % 10 == 0 ? 0 : 1),4}";
            }
        }

        private void HelpClick()
        {
            MessageBox.Query("Info", "Author is Vladyslav Pidruchnyy!", "OK");
        }

        private void OnExitClicked()
        {
            Application.RequestStop();
        }
        private void NotImplementedClick()
        {
            MessageBox.Query("Info", "This feature is not implemented!", "OK");
        }
        public void SetRepository(UserRep UserRep, OrderRep OrderRep, ProductRep ProductRep)
        {
            this.UserRep = UserRep;
            this.OrderRep = OrderRep;
            this.ProductRep = ProductRep;
        }
        public void SetConnection(SqliteConnection connection)
        {
            this.connection = connection;
        }
    }
}