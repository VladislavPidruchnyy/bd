using Terminal.Gui;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
using EntitiesManager;


namespace GUIApp
{
    public class AddProduct : Dialog
    {
        protected TextField productNameInput;
        protected TextField priceInput;
        protected CheckBox availabilityInput;
        private SqliteConnection connection;

        private OrderRep OrderRep;
        private ProductRep ProductRep;
        private UserRep UserRep;

        public AddProduct()
        {
            this.Title = "Add product";
            Button okBtn = new Button("OK");
            okBtn.Clicked += OnCreateDialogSubmit;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            Label prodLbl = new Label(2, 2, "Product name: ");
            productNameInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(prodLbl),
                Width = 40
            };
            this.Add(prodLbl, productNameInput);

            Label prodLbl1 = new Label(2, 4, "price: ");
            priceInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(prodLbl1),
                Width = 40
            };
            this.Add(prodLbl1, priceInput);

            Label isAvaliableLbl = new Label(2, 8, "Avaliable: ");
            availabilityInput = new CheckBox("")
            {
                X = 20,
                Y = Pos.Top(isAvaliableLbl),
                Width = 40
            };
            this.Add(isAvaliableLbl, availabilityInput);
        }

        
        public void OnCreateDialogCanceled()
        {
            Application.RequestStop();
        }
        public void OnCreateDialogSubmit()
        {
            long n;
            if (long.TryParse(priceInput.Text.ToString(), out n))
            {
                Product p = new Product();
                p.productName = productNameInput.Text.ToString();
                p.price = int.Parse(priceInput.Text.ToString());
                if(p.price>0)
                {
                    if (availabilityInput.Checked.ToString() == "True")
                    {
                        p.availability = true;
                    }
                    else
                    {
                        p.availability = false;
                    }
                    p.createdAt = System.DateTime.Now;

                    ProductRep.Insert(p);
                    Application.RequestStop();
                }
                else
                {
                    MessageBox.ErrorQuery("Error", "Price should be positive!", "OK");
                }
            }
            else
            {
                MessageBox.ErrorQuery("Invalid price", "Invalid price entered.", "OK");
            }
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
    public class AddOrder : Dialog
    {
        protected TextField productIdInput;
        protected ListView productList;
        Label amount;
        private SqliteConnection connection;

        private OrderRep OrderRep;
        private ProductRep ProductRep;
        private UserRep UserRep;

        List<Product> products = new List<Product>();

        Order order = new Order();

        public AddOrder(long id)
        {
            order.customerId = id;

            this.Title = "Add order";
            Button okBtn = new Button("OK");
            okBtn.Clicked += OnCreateDialogSubmit;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            amount = new Label(2, 2, "Amount: 0                                                ");

            Label prodLbl = new Label(2, 4, "Product ID: ");
            productIdInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(prodLbl),
                Width = 40
            };

            Button addBtn = new Button(2, 6, "Add");
            Button remBtn = new Button(10, 6, "Remove");
            addBtn.Clicked += Add;
            remBtn.Clicked += Rem;

            productList = new ListView(new Rect(2, 8, 100, 10), new List<Product>());

            this.Add(prodLbl, productIdInput, addBtn, remBtn, amount, productList);
        }

        private void Add()
        {
            long n;
            if (long.TryParse(productIdInput.Text.ToString(), out n))
            {
                Product p = ProductRep.GetProductById(Convert.ToInt64(productIdInput.Text));
                if (p.id != 0)
                {
                    if(p.availability == true)
                    {
                        order.products.Add(p);
                        order.amount += p.price;
                        productList.SetSource(order.products);
                        amount.Text = "Amount " + order.amount;
                    }
                    else
                    {
                        MessageBox.Query("Error", "This item is out of stock. Choose another one!", "OK");
                    }
                    
                }
            }
            else
            {
                MessageBox.Query("Error", "Id should be number!", "OK");
            }
        }
        private void Rem()
        {
            long n;
            if (long.TryParse(productIdInput.Text.ToString(), out n))
            {
                Product p = ProductRep.GetProductById(Convert.ToInt64(productIdInput.Text));
                for (int a = 0; a < order.products.Count; a++)
                    if (order.products[a].id == p.id)
                    {
                        order.products.RemoveAt(a);
                        order.amount -= p.price;
                        productList.SetSource(order.products);
                        amount.Text = "Amount " + order.amount;
                        break;
                    }
            }
            else
            {
                MessageBox.ErrorQuery("Invalid ID", "Invalid ID entered.", "OK");
            }
        }

        public void OnCreateDialogCanceled()
        {
            Application.RequestStop();
        }
        public void OnCreateDialogSubmit()
        {
            order.orderDate = DateTime.Now;
            long id = OrderRep.Insert(order);
            foreach (Product p in order.products)
            {
                OrderRep.AddProductConection(p.id, id);
            }
            Application.RequestStop();
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
    public class EditProduct : Dialog
    {
        protected TextField productNameInput;
        protected TextField priceInput;
        protected CheckBox availabilityInput;
        private SqliteConnection connection;

        private OrderRep OrderRep;
        private ProductRep ProductRep;
        private UserRep UserRep;
        long id;

        public EditProduct(ProductRep productRep, long id)
        {
            this.id = id;
            Product p = productRep.GetProductById(id);
            this.Title = "Edit product";
            Button okBtn = new Button("OK");
            okBtn.Clicked += OnCreateDialogSubmit;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            Label prodLbl = new Label(2, 2, "Product name: ");
            productNameInput = new TextField(p.productName)
            {
                X = 20,
                Y = Pos.Top(prodLbl),
                Width = 40
            };
            this.Add(prodLbl, productNameInput);

            Label prodLbl1 = new Label(2, 4, "price: ");
            priceInput = new TextField(p.price.ToString())
            {
                X = 20,
                Y = Pos.Top(prodLbl1),
                Width = 40
            };
            this.Add(prodLbl1, priceInput);

            Label isAvaliableLbl = new Label(2, 8, "Avaliable: ");
            availabilityInput = new CheckBox("")
            {
                X = 20,
                Y = Pos.Top(isAvaliableLbl),
                Width = 40
            };
            availabilityInput.Checked = p.availability;
            this.Add(isAvaliableLbl, availabilityInput);
        }
        public void OnCreateDialogCanceled()
        {
            Application.RequestStop();
        }
        public void OnCreateDialogSubmit()
        {
            long n;
            if (long.TryParse(priceInput.Text.ToString(), out n))
            {
                Product p = new Product();
                p.productName = productNameInput.Text.ToString();
                p.price = int.Parse(priceInput.Text.ToString());
                if(p.price>0)
                {
                    if (availabilityInput.Checked.ToString() == "True")
                    {
                        p.availability = true;
                    }
                    else
                    {
                        p.availability = false;
                    }
                    p.createdAt = System.DateTime.Now;

                    ProductRep.Update(id,p);
                    Application.RequestStop();
                }
                else
                {
                    MessageBox.ErrorQuery("Error", "Price should be positive!", "OK");
                }
            }
            else
            {
                MessageBox.Query("Error", "Price should be number!", "OK");
            }
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
    public class EditOrder : Dialog
    {
        protected TextField productIdInput;
        protected ListView productList;
        Label amount;
        private SqliteConnection connection;

        private OrderRep OrderRep;
        private ProductRep ProductRep;
        private UserRep UserRep;

        List<Product> products = new List<Product>();

        Order order = new Order();

        public EditOrder(OrderRep orderRep, ProductRep productRep, long id)
        {
            order = orderRep.GetOrderById(id);
            foreach (long l in orderRep.GetProductIds(order))
            {
                order.products.Add(productRep.GetProductById(l));
            }

            this.Title = "Edit order";
            Button okBtn = new Button("OK");
            okBtn.Clicked += OnCreateDialogSubmit;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            amount = new Label(2, 2, $"Amount: {order.amount}                                                ");

            Label prodLbl = new Label(2, 4, "Product ID: ");
            productIdInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(prodLbl),
                Width = 40
            };

            Button addBtn = new Button(2, 6, "Add");
            Button remBtn = new Button(10, 6, "Remove");
            addBtn.Clicked += Add;
            remBtn.Clicked += Rem;

            productList = new ListView(new Rect(2, 8, 100, 10), new List<Product>());
            productList.SetSource(order.products);

            this.Add(prodLbl, productIdInput, addBtn, remBtn, amount, productList);
        }

        private void Add()
        {
            long n;
            if (long.TryParse(productIdInput.Text.ToString(), out n))
            {
                Product p = ProductRep.GetProductById(Convert.ToInt64(productIdInput.Text));
                if (p.id != 0)
                {
                    order.products.Add(p);
                    order.amount += p.price;
                    productList.SetSource(order.products);
                    amount.Text = "Amount " + order.amount;
                }
            }
            else
            {
                MessageBox.ErrorQuery("Invalid ID", "Invalid ID entered.", "OK");
            }
        }
        private void Rem()
        {
            long n;
            if (long.TryParse(productIdInput.Text.ToString(), out n))
            {
                Product p = ProductRep.GetProductById(Convert.ToInt64(productIdInput.Text));
                for (int a = 0; a < order.products.Count; a++)
                    if (order.products[a].id == p.id)
                    {
                        order.products.RemoveAt(a);
                        order.amount -= p.price;
                        productList.SetSource(order.products);
                        amount.Text = "Amount " + order.amount;
                        break;
                    }
            }
            else
            {
                MessageBox.ErrorQuery("Invalid ID", "Invalid ID entered.", "OK");
            }
        }

        public void OnCreateDialogCanceled()
        {
            Application.RequestStop();
        }
        public void OnCreateDialogSubmit()
        {
            OrderRep.DeleteProductConection(order.id);
            order.orderDate = DateTime.Now;
            OrderRep.Update(order.id, order);
            foreach (Product p in order.products)
            {
                OrderRep.AddProductConection(p.id, order.id);
            }
            Application.RequestStop();
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
    public class ExportDialog : Dialog
    {
        protected TextField exportPathInput;
        protected TextField orderIdInput;
        private SqliteConnection connection;

        private OrderRep OrderRep;
        private ProductRep ProductRep;
        private UserRep UserRep;

        public ExportDialog()
        {
            Title = "Export";
            Button okBtn = new Button("OK");
            okBtn.Clicked += OnCreateDialogSubmit;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            Label exportPathLabel = new Label(2, 2, "Export path: ");
            exportPathInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(exportPathLabel),
                Width = 40
            };
            this.Add(exportPathLabel, exportPathInput);

            Label orderIdLabel = new Label(2, 4, "OrderId: ");
            orderIdInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(orderIdLabel),
                Width = 40
            };
            this.Add(orderIdLabel, orderIdInput);
        }

        public void OnCreateDialogCanceled()
        {
            Application.RequestStop();
        }
        public void OnCreateDialogSubmit()
        {
            ExportAndImport.Export(Convert.ToInt64(orderIdInput.Text.ToString()), exportPathInput.Text.ToString(), OrderRep, ProductRep);
            Application.RequestStop();
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
    public class ImportDialog : Dialog
    {

        protected TextField fullNameInput;
        protected TextField productIdInput;
        private SqliteConnection connection;

        private OrderRep OrderRep;
        private ProductRep ProductRep;
        private UserRep UserRep;

        public ImportDialog()
        {
            Title = "Import";
            Button okBtn = new Button("OK");
            okBtn.Clicked += OnCreateDialogSubmit;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            Label exportPathLabel = new Label(2, 2, "File's full name: ");
            fullNameInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(exportPathLabel),
                Width = 40
            };
            this.Add(exportPathLabel, fullNameInput);

            Label orderIdLabel = new Label(2, 4, "ProductId: ");
            productIdInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(orderIdLabel),
                Width = 40
            };
            this.Add(orderIdLabel, productIdInput);
        }

        public void OnCreateDialogCanceled()
        {
            Application.RequestStop();
        }
        public void OnCreateDialogSubmit()
        {
            bool error;
            ExportAndImport.Import(Convert.ToInt64(productIdInput.Text.ToString()), fullNameInput.Text.ToString(), OrderRep,out error);
            if (error)
                MessageBox.ErrorQuery("Error", "Import failed. Note that an XML file is used for the import.", "OK");
            else
                MessageBox.Query("Succes", "Import completed.", "OK");
            Application.RequestStop();
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

    public class SearchProduct: Dialog
    {
        protected TextField searchInput;
        private SqliteConnection connection;
        private ProductRep ProductRep;

        public SearchProduct()
        {
            Title = "SearchProduct";
            Button okBtn = new Button("OK");
            okBtn.Clicked += OnCreateDialogSubmit;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCanceled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            Label searchProductLbl = new Label(2, 2, "Enter product name:  ");
            searchInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(searchProductLbl),
                Width = 40
            };
            this.Add(searchProductLbl, searchInput);
        }

        public void OnCreateDialogCanceled()
        {
            Application.RequestStop();
        }
        // List<Product> productList;
        // ListView products = new ListView();
        // int page;
        public void OnCreateDialogSubmit()
        {
            List<Product> list = this.ProductRep.GetExportProducts(this.searchInput.Text.ToString());
            if(list.Count == 0)
            {
                MessageBox.Query("Error", $"No products by such name!", "OK");
            }
            else
            {
                ShowTitlesSearchDialog dialog = new ShowTitlesSearchDialog(list);
                Application.Run(dialog);
            }
            
            
        }
        public void SetRepository(UserRep UserRep, OrderRep OrderRep, ProductRep ProductRep)
        {
            // this.UserRep = UserRep;
            // this.OrderRep = OrderRep;
            this.ProductRep = ProductRep;
        }
        public void SetConnection(SqliteConnection connection)
        {
            this.connection = connection;
        }
    }

    public class ShowTitlesSearchDialog : Dialog
    {
        private ListView allProductsView;
        public ShowTitlesSearchDialog(List<Product> products)
        {
            this.Title = "Results";
            FrameView frameView = new FrameView("Results")
            {
                X = 2,
                Y = 2,
                Width = Dim.Fill() - 1,
                Height = Dim.Fill() - 1,
            };
            allProductsView = new ListView(new List<Product>())
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            this.allProductsView.SetSource(products);
            frameView.Add(allProductsView);
            this.Add(frameView);

            Button okBtn = new Button("Back");
            okBtn.Clicked += OnCreateDialogSubmit;

            this.AddButton(okBtn);
        }
        private void OnCreateDialogSubmit()
        {
            Application.RequestStop();
        }
    }
}
