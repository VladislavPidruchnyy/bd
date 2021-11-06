using System;
using System.Collections.Generic;

public class User
    {
        public long id;
        public string username;
        public string fullname;
        public string status;
        public string password;
        public DateTime createdAt;

        public List<Order> orders;

        public User()
        {
            this.id = 0;
            this.username = "";
            this.fullname = "";
            this.status = "";
            this.password = "";
            this.createdAt = default;
        }

        public User(string username, string fullname, string status, string password, DateTime createdAt)
        {
            this.username = username;
            this.fullname = fullname;
            this.status = status;
            this.password = password;
            this.createdAt = createdAt;
        }

        public override string ToString()
        {
            return $"{id} {username} {fullname} {status} {createdAt.ToString()}";
        }

    }