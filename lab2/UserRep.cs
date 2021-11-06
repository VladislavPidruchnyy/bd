using System.Collections.Generic;
using Npgsql;
using System;


public class UserRep
    {
        private NpgsqlConnection connection;
        public UserRep(NpgsqlConnection connection)
        {
            this.connection = connection;
        }

        public long Insert(User user)
        {
            connection.Open();

            var sql =
           @"INSERT INTO users (username, fullname, status, password, createdAt) 
            VALUES (@username, @fullname, @status, @password, @createdAt);";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@username", user.username);
            command.Parameters.AddWithValue("@fullname", user.fullname);
            command.Parameters.AddWithValue("@status", user.status);
            command.Parameters.AddWithValue("@password", user.password);
            command.Parameters.AddWithValue("@createdAt", DateTime.Now);
            long lastId = command.ExecuteNonQuery();
            connection.Close();

            return lastId;
        }


        public List<User> GetAllUsers()
        {
            connection.Open();

            var sql = @"SELECT * FROM users";
            using var command = new NpgsqlCommand(sql, connection);
            NpgsqlDataReader reader = command.ExecuteReader();
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

        public User GetUserById(long id)
        {
            connection.Close();
            connection.Open();

            var sql = @"SELECT * FROM users WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            User user = new User();
            if (reader.Read())
            {
                user = GetUser(reader);
            }
            reader.Close();
            connection.Close();
            return user;
        }
        public long GetCount()
        {
            connection.Open();


            var sql = @"SELECT COUNT(*) FROM users";
            using var command = new NpgsqlCommand(sql, connection);

            long count = (long)command.ExecuteScalar();
            return count;
        }

        private User GetUser(NpgsqlDataReader reader)
        {
            User user = new User();
            user.id = reader.GetInt32(0);
            user.username = reader.GetString(1);
            user.fullname = reader.GetString(2);
            user.status = reader.GetString(3);
            user.password = reader.GetString(4);
            user.createdAt = reader.GetDateTime(5);

            return user;
        }

        public bool DeleteById(long id)
        {
            connection.Open();

            var sql = @"DELETE FROM users WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
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

            var sql = @"UPDATE users SET username = @username, fullname = @fullname, status = @status, createdAt = @createdAt WHERE id = @id";
            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@fullname", user.fullname);
            command.Parameters.AddWithValue("@username", user.username);
            command.Parameters.AddWithValue("@status", user.status);
            command.Parameters.AddWithValue("@createdAt", DateTime.Now);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            if (nChanged == 1)
            {
                return true;
            }
            return false;
        }


    }