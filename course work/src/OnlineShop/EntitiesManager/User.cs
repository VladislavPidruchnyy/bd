using System.Collections.Generic;

namespace EntitiesManager
{
    public class User
    {
        public long id;
        public string username;
        public string fullname;
        public string status;
        public string pass;
        public List<Order> orders;

        public User()
        {
            this.id = 0;
            this.username = "";
            this.fullname = "";
            this.status = "";
            this.pass = "";
        }

        public override string ToString()
        {
            return $"{id} {username} {fullname} {status} {pass}";
        }
    }
}
