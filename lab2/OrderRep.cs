using static System.Console;
using System.Collections.Generic;
using Npgsql;
using System;



public class OrderRep
    {
        private NpgsqlConnection connection;
        public OrderRep(NpgsqlConnection connection)
        {
            this.connection = connection;
        }

        public long Insert(Order order)
        {
            connection.Open();

            var sql =
             @"INSERT INTO orders (customerId, orderDate, amount) 
            VALUES (@customerId, @orderDate, @amount);";

            using var command = new NpgsqlCommand(sql, connection);

            command.Parameters.AddWithValue("@customerId", order.customerId);
            command.Parameters.AddWithValue("@orderDate", order.orderDate);
            command.Parameters.AddWithValue("@amount", order.amount);

            long lastId =  command.ExecuteNonQuery();
            connection.Close();

            return lastId;
        }
        public bool AddProductConection(long productId, long orderId)
        {
            connection.Open();
            var sql =
            @"INSERT INTO products_orders (orderId, productId) 
            VALUES (@orderId, @productId);";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@orderId", orderId);
            command.Parameters.AddWithValue("@productId", productId);

            long lastId =  command.ExecuteNonQuery();

            connection.Close();
            if (lastId == 0)
            {
                return false;
            }
            return true;
        }

        public List<Order> GetAllOrders()
        {
            connection.Open();

            var sql = @"SELECT * FROM orders";
            using var command = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Order> orders = new List<Order>();
            while (reader.Read())
            {
                Order order = GetOrder(reader);
                orders.Add(order);
            }
            reader.Close();
            connection.Close();
            return orders;
        }
        public long GetCount()
        {
            connection.Open();


            var sql = @"SELECT COUNT(*) FROM products";
            using var command = new NpgsqlCommand(sql, connection);

            long count = (long)command.ExecuteScalar();
            return count;
        }

        public Order GetOrderById(long id)
        {
            connection.Open();

            var sql = @"SELECT * FROM orders WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            Order order = new Order();
            if (reader.Read())
            {
                order = GetOrder(reader);

            }
            connection.Close();
            return order;

        }

        public bool DeleteById(long id)
        {
            connection.Open();

            var sql = @"DELETE FROM orders WHERE id = @id";
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

        private bool DeleteByIdOrder(long id)
        {
            connection.Open();

            var sql = @"DELETE FROM products_orders WHERE orderId = @orderId";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            if (nChanged == 0)
            {
                return true;
            }

            return false;
        }

        public bool DeleteByIdProduct(long id, long orderId)
        {
            connection.Open();

            var sql = @"Select FROM products_orders WHERE productId = @productId";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@productId", id);

            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                return this.DeleteById(orderId);

            }
            reader.Close();
            connection.Close();
            return false;

        }
        public bool Update(long id, Order order)
        {
            connection.Open();

            var sql = @"UPDATE orders SET customerId = @customerId, orderDate = @orderDate, amount = @amount WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", order.id);
            command.Parameters.AddWithValue("@customerId", order.customerId);
            command.Parameters.AddWithValue("@orderDate", order.orderDate);
            command.Parameters.AddWithValue("@amount", order.amount);
            int rowChange = command.ExecuteNonQuery();
            connection.Close();
            if (rowChange == 0)
            {
                return false;
            }

            return true;
        }


        public static Order GetOrder(NpgsqlDataReader reader)
        {
            Order order = new Order();
            order.id = reader.GetInt32(0);
            order.customerId = reader.GetInt32(1);
            order.orderDate = reader.GetDateTime(2);
            order.amount = reader.GetDouble(3);
            return order;
        }



    }