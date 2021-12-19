using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace EntitiesManager
{
    public class OrderRep
    {
        private SqliteConnection connection;
        public OrderRep(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public long Insert(Order order)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"INSERT INTO orders (customerId, orderDate, amount) 
            VALUES ($customerId, $orderDate, $amount);
            SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$customerId", order.customerId);
            command.Parameters.AddWithValue("$orderDate", order.orderDate.ToString("o"));
            command.Parameters.AddWithValue("$amount", order.amount);

            long lastId = (long)command.ExecuteScalar();
            connection.Close();

            return lastId;
        }

        public List<Order> GetAllOrders()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM orders";

            SqliteDataReader reader = command.ExecuteReader();
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

        public List<Order> GetAllById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM orders WHERE customerId = {id}";

            SqliteDataReader reader = command.ExecuteReader();
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

        public Order GetOrderById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM orders WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            Order order = new Order();
            if (reader.Read())
            {
                order = GetOrder(reader);
            }
            connection.Close();
            return order;

        }

        public static Order GetOrder(SqliteDataReader reader)
        {
            Order order = new Order();
            order.id = int.Parse(reader.GetString(0));
            order.customerId = long.Parse(reader.GetString(1));
            order.orderDate = DateTime.Parse(reader.GetString(2));
            order.amount = int.Parse(reader.GetString(3));
            return order;
        }

        public List<long> GetProductIds(Order order)
        {
            connection.Open();
            List<long> productIds = new List<long>();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT productId FROM products_orders WHERE orderId={order.id}";
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                productIds.Add(reader.GetInt64(0));
            }
            connection.Close();
            return productIds;
        }


        public int DeleteById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM orders WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            if (nChanged == 0)
            {
                return 0;
            }

            return 1;
        }
        public bool Update(long id, Order order)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"UPDATE orders SET customerId = $customerId, orderDate = $orderDate, amount = $amount WHERE id = $id";
            command.Parameters.AddWithValue("$id", order.id);
            command.Parameters.AddWithValue("$customerId", order.customerId);
            command.Parameters.AddWithValue("$orderDate", order.orderDate);
            command.Parameters.AddWithValue("$amount", order.amount);
            int rowChange = command.ExecuteNonQuery();
            connection.Close();
            if (rowChange == 0)
            {
                return false;
            }

            return true;
        }


        public bool AddProductConection(long productId, long orderId)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"INSERT INTO products_orders (orderId, productId) 
            VALUES ($orderId, $productId);
            SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$orderId", orderId);
            command.Parameters.AddWithValue("$productId", productId);

            long lastId = (long)command.ExecuteScalar();
            connection.Close();
            if (lastId == 0)
            {
                return false;
            }
            return true;
        }
        public bool DeleteProductConection(long orderId)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"DELETE FROM products_orders WHERE orderId = $orderId";
            command.Parameters.AddWithValue("$orderId", orderId);
            object a = command.ExecuteScalar();
            long lastId = a == null ? 0 : (long)a;
            connection.Close();
            if (lastId == 0)
            {
                return false;
            }
            return true;
        }


        public List<long> GetAllOrderProductsId(long orderId)
        {
            List<long> prodIdList = new List<long>();
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM products_orders WHERE orderId = $orderId";
            command.Parameters.AddWithValue("$orderId", orderId);
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                long productId = long.Parse(reader.GetString(2));
                prodIdList.Add(productId);
            }
            connection.Close();
            return prodIdList;
        }

        public List<Order> GetAllUserOrdersById(List<long> orderId)
        {
            List<Order> ordersList = new List<Order>();
            connection.Open();
            foreach (long id in orderId)
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM orders WHERE id = $id";
                command.Parameters.AddWithValue("$id", id);
                SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Order order = GetOrder(reader);
                    ordersList.Add(order);
                }
            }

            connection.Close();
            return ordersList;
        }

    }
}
