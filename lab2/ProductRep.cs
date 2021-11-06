using static System.Console;
using System.Collections.Generic;
using Npgsql;
using System;


public class ProductRep
    {
        private NpgsqlConnection connection;
        public ProductRep(NpgsqlConnection connection)
        {
            this.connection = connection;
        }

        public long GetCount()
        {
            connection.Close();
            connection.Open();


            var sql = @"SELECT COUNT(*) FROM products";
            using var command = new NpgsqlCommand(sql, connection);

            long count = (long)command.ExecuteScalar();
            return count;
        }

        public long Insert(Product product)
        {
            connection.Open();

            var sql =
           @"INSERT INTO products (productName, price, availability, createdAt) 
            VALUES (@productName, @price, @availability, @createdAt);";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@productName", product.productName);
            command.Parameters.AddWithValue("@price", product.price);
            command.Parameters.AddWithValue("@availability", product.isPresent);
            command.Parameters.AddWithValue("@createdAt", DateTime.Now);
            long lastId = command.ExecuteNonQuery();

            connection.Close();

            return lastId;
        }

        public List<Product> GetAllProducts()
        {
            connection.Open();

            var sql = @"SELECT * FROM products";
            using var command = new NpgsqlCommand(sql, connection);

            NpgsqlDataReader reader = command.ExecuteReader();
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

            var sql = @"SELECT * FROM products WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            Product product = new Product();
            if (reader.Read())
            {
                product = GetProduct(reader);
            }
            connection.Close();

            return product;
        }


        public bool DeleteById(long id)
        {
            connection.Open();

            var sql = @"DELETE FROM products WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            if (nChanged == 0)
            {
                return false;
            }

            return true;
        }


        public bool Update(long id, Product product)
        {
            connection.Open();

            var sql = @"UPDATE products SET productName = @productName, price = @price, availability = @availability, createdAt = @createdAt WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@productName", product.productName);
            command.Parameters.AddWithValue("@price", product.price);
            command.Parameters.AddWithValue("@availability", product.isPresent);
            command.Parameters.AddWithValue("@createdAt", product.createdAt);
            int rowChange = command.ExecuteNonQuery();
            connection.Close();
            if (rowChange == 0)
            {
                return false;
            }

            return true;
        }

        public List<Product> GetOrderProducts(List<long> productIds)
        {
            List<Product> productList = new List<Product>();
            connection.Open();
            foreach (long id in productIds)
            {

                var sql = @"SELECT * FROM products WHERE id = @id";
                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                NpgsqlDataReader reader = command.ExecuteReader();
                Product product = new Product();
                if (reader.Read())
                {
                    product = GetProduct(reader);
                    productList.Add(product);
                }
            }
            connection.Close();

            return productList;
        }
        static Product GetProduct(NpgsqlDataReader reader)
        {
            Product product = new Product();
            product.id = reader.GetInt32(0);
            product.productName = reader.GetString(1);
            product.price = reader.GetDouble(2);
            product.isPresent = reader.GetBoolean(3);


            return product;
        }
        



    }