using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace EntitiesManager
{
    public class UserRep
    {
        private SqliteConnection connection;
        public UserRep(SqliteConnection connection)
        {
            this.connection = connection;
        }

        public bool IsExistUsers()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users";
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

        public bool Insert(User user)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText =
            @"INSERT INTO users (username, fullname, status, pass) 
            VALUES ($username, $fullname, $status, $pass);
            SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$username", user.username);
            command.Parameters.AddWithValue("$fullname", user.fullname);
            command.Parameters.AddWithValue("$status", user.status);
            command.Parameters.AddWithValue("$pass", user.pass);

            long lastId = (long)command.ExecuteScalar();
            connection.Close();

            return lastId != 0;
        }

        public List<User> GetAllUsers()
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users";

            SqliteDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();
            while (reader.Read())
            {
                User user = GetUser(reader);
                users.Add(user);
            }
            reader.Close();
            connection.Close();
            return users;
        }
        public long CheckUserName(string name)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users";

            SqliteDataReader reader = command.ExecuteReader();
            ;
            while (reader.Read())
            {
                if (name == reader.GetString(1))
                {
                    long lastId = long.Parse(reader.GetString(0));
                    reader.Close();
                    connection.Close();
                    return lastId;
                }
            }

            reader.Close();
            connection.Close();
            return 0;
        }

        public User GetUserById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            SqliteDataReader reader = command.ExecuteReader();
            User user = new User();
            if (reader.Read())
            {
                user = GetUser(reader);
            }
            reader.Close();
            connection.Close();
            return user;
        }

        private User GetUser(SqliteDataReader reader)
        {
            User user = new User();
            user.id = int.Parse(reader.GetString(0));
            user.username = reader.GetString(1);
            user.fullname = reader.GetString(2);
            user.status = reader.GetString(3);

            return user;
        }

        public bool DeleteById(long id)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();

            if (nChanged == 1)
            {
                return true;
            }
            return false;
        }

        public bool Update(long id, User user)
        {
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"UPDATE users SET username = $username, fullname = $fullname, status = $status WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$fullname", user.fullname);
            command.Parameters.AddWithValue("$username", user.username);
            command.Parameters.AddWithValue("$status", user.status);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            if (nChanged == 1)
            {
                return true;
            }
            return false;
        }

        public List<Order> ReadAllUsersOrders(User user)
        {
            if (user == null)
            {
                throw new Exception("Not exixsting user.");
            }

            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", user.id);
            SqliteDataReader reader = command.ExecuteReader();
            List<Order> usersOreders = new List<Order>();
            while (reader.Read())
            {
                Order order = OrderRep.GetOrder(reader);

                usersOreders.Add(order);
            }
            reader.Close();
            connection.Close();
            return usersOreders;
        }

        public List<long> GetAllUserOrdersId(long userId)
        {
            List<long> ordersId = new List<long>();
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM orders WHERE customerId = $customerId";
            command.Parameters.AddWithValue("$customerId", userId);
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                long orderId = long.Parse(reader.GetString(0));
                ordersId.Add(orderId);
            }
            connection.Close();
            return ordersId;
        }

        public bool CheckUserInDb(string username, string pass2)
        {
            string pass = HashPassword.HashCode(pass2);
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT count(rowid) username, pass
            FROM users
            WHERE username = $username AND pass = $pass";
            command.Parameters.AddWithValue("$username", username);
            command.Parameters.AddWithValue("$pass", pass);
            long index = (long)command.ExecuteScalar();
            connection.Close();
            if (index == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}