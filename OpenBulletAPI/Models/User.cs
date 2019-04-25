using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenBulletAPI.Models
{
    public class User
    {
        [BsonId]
        public string Key { get; set; }

        [BsonField("Groups")]
        public string[] Groups { get; set; }
    }
}
