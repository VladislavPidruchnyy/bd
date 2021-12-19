using System;
using static System.Console;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;



namespace EntitiesManager
{
    public class ProductRep
    {
        private SqliteConnection connection;
        public ProductRep(SqliteConnection connection)
        {
            this.connection = connection;
        }


        public bool IsExistProducts()
        {
            connection.Open();
            WriteLine(connection.State);
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM products";
            SqliteDataReader reader = command.ExecuteReader();
            int count = 0;

            if (reader.Read() && reader.GetValue(0) != DBNull.Value)
            {
                count = int.Parse(reader.GetString(0));
            }
            connection.Close();
            if (count == 0)
            {
                return false;
            }
            return true;
        }

        public bool Insert(Product product)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"INSERT INTO products (productName, price, availability, createdAt) 
            VALUES ($productName, $price, $availability, $createdAt);
            SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$productName", product.productName);
            command.Parameters.AddWithValue("$price", product.price);
            command.Parameters.AddWithValue("$availability", product.availability);
            command.Parameters.AddWithValue("$createdAt", product.createdAt.ToString("o"));
            long lastId = (long)command.ExecuteScalar();
            connection.Close();
            if (lastId == 0)
            {
                return false;
            }

            return true;
        }

        public List<Product> GetAllGoods()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM products";

            SqliteDataReader reader = command.ExecuteReader();
            List<Product> products = new List<Product>();
            while (reader.Read())
            {
                Product product = GetProduct(reader);
                products.Add(product);
            }
            reader.Close();
            connection.Close();
            return products;
        }

        public Product GetProductById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM products WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            Product product = new Product();
            if (reader.Read())
            {
                product = GetProduct(reader);
            }
            connection.Close();

            return product;
        }

        static Product GetProduct(SqliteDataReader reader)
        {
            Product product = new Product();
            product.id = long.Parse(reader.GetString(0));
            product.productName = reader.GetString(1);
            product.price = int.Parse(reader.GetString(2));
            if (int.Parse(reader.GetString(3)) == 1)
            { product.availability = true; }
            else { product.availability = false; }
            product.createdAt = DateTime.Parse(reader.GetString(4));
            return product;
        }
        public int DeleteById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM products WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            if (nChanged == 0)
            {
                return 0;
            }

            return 1;
        }


        public bool Update(long id, Product product)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"UPDATE products SET productName = $productName, price = $price, availability = $availability, createdAt = $createdAt WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$productName", product.productName);
            command.Parameters.AddWithValue("$price", product.price);
            command.Parameters.AddWithValue("$availability", product.availability);
            command.Parameters.AddWithValue("$createdAt", product.createdAt);
            int rowChange = command.ExecuteNonQuery();
            connection.Close();
            if (rowChange == 0)
            {
                return false;
            }

            return true;
        }

        public List<Product> GetOrderProducts(List<long> prodIds)
        {
            List<Product> prodList = new List<Product>();
            connection.Open();
            foreach (long id in prodIds)
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM products WHERE id = $id";
                command.Parameters.AddWithValue("$id", id);
                SqliteDataReader reader = command.ExecuteReader();
                Product product = new Product();
                if (reader.Read())
                {
                    product = GetProduct(reader);
                    prodList.Add(product);
                }
            }
            connection.Close();

            return prodList;
        }

        public List<Product> GetExportProducts(string substring)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM products WHERE productName LIKE '%' || $value || '%' ";
            command.Parameters.AddWithValue("$value", substring);

            SqliteDataReader reader = command.ExecuteReader();

            List<Product> productsList = new List<Product>();
            while (reader.Read())
            {
                Product product = new Product();
                product.id = long.Parse(reader.GetString(0));
                product.productName = reader.GetString(1);
                product.price = int.Parse(reader.GetString(2));
                if (int.Parse(reader.GetString(3)) == 1)
                { product.availability = true; }
                else { product.availability = false; }
                product.createdAt = DateTime.Parse(reader.GetString(4));
                productsList.Add(product);
            }
            reader.Close();
            connection.Close();

            return productsList;
        }

    }
}