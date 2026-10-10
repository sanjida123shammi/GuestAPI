using GuestAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GuestAPI.Repository
{
    public class UserRepo : IDisposable
    {
        private readonly GuestDbContext _db = new GuestDbContext();
        public void Dispose()
        {
            _db.Dispose();
        }

        public User ValidateUser(string username, string password)
        {
            User user = _db.Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.Password == password);
            return user;
        }
    }
}