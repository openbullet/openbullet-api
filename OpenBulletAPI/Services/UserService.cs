using LiteDB;
using Microsoft.Extensions.Configuration;
using OpenBulletAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenBulletAPI.Services
{
    public class UserService
    {
        private readonly LiteCollection<User> _users;
        private readonly string _secretKey;

        public User GetUser(string key)
        {
            return _users.Find(user => user.Key == key).FirstOrDefault();
        }

        public UserService(IConfiguration configuration)
        {
            using (var db = new LiteDatabase(configuration.GetConnectionString("db")))
            {
                _users = db.GetCollection<User>("Users");
            }

            _secretKey = configuration.GetConnectionString("secretKey");
        }

        public List<User> Get(string secretKey)
        {
            if (IsAdmin(secretKey))
            {
                return _users.FindAll().ToList();
            }
            else
            {
                return new List<User>();
            }
        }

        public User Get(string secretKey, string key)
        {
            if (IsAdmin(secretKey))
            {
                return GetUser(key);
            }
            else
            {
                return null;
            }
        }

        public User Create(string secretKey, User user)
        {
            if (IsAdmin(secretKey))
            {
                _users.Insert(user);
                return user;
            }
            else
            {
                return null;
            }
        }

        public void Update(string secretKey, User user)
        {
            if (IsAdmin(secretKey))
            {
                _users.Update(user);
            }
        }

        public void Remove(string secretKey, User user)
        {
            if (IsAdmin(secretKey))
            {
                _users.Delete(c => c.Key == user.Key);
            }
        }

        public void Remove(string secretKey, string key)
        {
            if (IsAdmin(secretKey))
            {
                _users.Delete(user => user.Key == key);
            }
        }

        private bool IsAdmin(string secretKey)
        {
            return secretKey == _secretKey;
        }
    }
}
